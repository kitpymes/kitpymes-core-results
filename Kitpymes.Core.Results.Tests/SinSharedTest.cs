using Kitpymes.Core.Results.Settings;
using Kitpymes.Core.Shared;
using Kitpymes.Core.Shared.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Kitpymes.Core.Results.Tests
{
    /// <summary>
    /// Desde la 1.2.0, Results no depende de Kitpymes.Core.Shared: lo que usaba de Shared es interno.
    /// Estos tests comprueban, a través de la API pública, que cada cosa copiada se comporta como en la 1.1.1,
    /// salvo ToAddOrUpdate, que desde la 1.2.1 suma los mensajes de un campo repetido en vez de sustituirlos.
    /// El proyecto de tests sí referencia Shared 2.0.1, la versión que usaba la 1.1.1, para comparar con el original.
    /// </summary>
    [TestClass]
    public class SinSharedTest
    {
        private const string TituloOkEs = "El proceso se ejecuto correctamente";
        private const string TituloOkEn = "The process was executed successfully";
        private const string TituloValidacionEs = "Se produjeron uno o más errores de validación";
        private const string TituloValidacionEn = "One or more validation errors occurred";
        private const string MensajeSinErrores = "Messages and Errors are null or not contains elements.";

        #region Títulos (SharedResource)

        [TestMethod]
        [DataRow("es-ES", TituloOkEs)]
        [DataRow("es", TituloOkEs)]
        [DataRow("en-US", TituloOkEn)]
        [DataRow("en", TituloOkEn)]
        [DataRow("fr-FR", TituloOkEs)]
        [DataRow("", TituloOkEs)]
        public void Ok_Passing_Cultura_Returns_MismoTituloQueShared(string cultura, string esperado)
        {
            ConCultura(cultura, () =>
            {
                Assert.AreEqual(esperado, ApiResult.Ok().Title);
                Assert.AreEqual(SharedResource.MsgProcessRanSuccessfully, ApiResult.Ok().Title);
                Assert.AreEqual(esperado, ApiResult.Ok(x => x.WithData(1)).Title);
            });
        }

        [TestMethod]
        [DataRow("es-ES", TituloValidacionEs)]
        [DataRow("en-US", TituloValidacionEn)]
        [DataRow("en", TituloValidacionEn)]
        [DataRow("fr-FR", TituloValidacionEs)]
        [DataRow("", TituloValidacionEs)]
        public void BadRequest_Passing_Cultura_Returns_MismoTituloQueShared(string cultura, string esperado)
        {
            ConCultura(cultura, () =>
            {
                var resultados = new[]
                {
                    ApiResult.BadRequest("m"),
                    ApiResult.BadRequest("m", new { A = 1 }),
                    ApiResult.BadRequest(new[] { "m" }),
                    ApiResult.BadRequest(new Dictionary<string, IEnumerable<string>> { ["Campo"] = new[] { "m" } }),
                    ApiResult.BadRequest(new[] { ("Campo", "m") }),
                };

                foreach (var actual in resultados)
                {
                    Assert.AreEqual(esperado, actual.Title);
                    Assert.AreEqual(SharedResource.MsgValidationsError, actual.Title);
                }
            });
        }

        #endregion Títulos (SharedResource)

        #region ToConfigureOrDefault

        [TestMethod]
        public void Ok_Passing_OpcionesNull_Returns_ResultadoPorDefecto()
        {
            ConCultura("es-ES", () =>
            {
                var actual = ApiResult.Ok(null!);

                Assert.IsTrue(actual.Success);
                Assert.AreEqual((int)HttpStatusCode.OK, actual.Status);
                Assert.AreEqual(TituloOkEs, actual.Title);
                Assert.IsNull(actual.Data);
            });
        }

        [TestMethod]
        public void Error_Passing_OpcionesNull_Throws_ArgumentException()
        {
            var exception = Assert.ThrowsExactly<ArgumentException>(() => ApiResult.Error(null!));

            Assert.AreEqual(MensajeSinErrores, exception.Message);
        }

        #endregion ToConfigureOrDefault

        #region Verify.ToThrowIf y ToIsNullOrAny

        [TestMethod]
        public void Error_Passing_SinMensajesNiErrores_Throws_ArgumentException_ConElMensajeDeLa111()
        {
            var exception = Assert.ThrowsExactly<ArgumentException>(() => ApiResult.Error(x => x.WithTitle("Título").WithStatusCode(HttpStatusCode.BadRequest)));

            Assert.AreEqual(MensajeSinErrores, exception.Message);
            Assert.IsNull(exception.ParamName);
        }

        [TestMethod]
        public void Error_Passing_ListasVacias_Throws_ArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => ApiResult.Error(x => x
                .WithMessages(new List<string>())
                .WithErrors(new Dictionary<string, IEnumerable<string>>())
                .WithErrors(Array.Empty<(string, string)>())));
        }

        [TestMethod]
        public void Error_Passing_SoloMensajes_Returns_Error()
        {
            var actual = ApiResult.Error(x => x.WithMessage("m"));

            Assert.IsFalse(actual.Success);
            Assert.AreEqual("m", actual.Message);
            Assert.IsNull(actual.Errors);
        }

        [TestMethod]
        public void Error_Passing_SoloErrores_Returns_Error()
        {
            var actual = ApiResult.Error(x => x.WithErrors(new[] { ("Campo", "m") }));

            Assert.IsFalse(actual.Success);
            Assert.IsNull(actual.Message);
            CollectionAssert.AreEqual(new[] { "m" }, actual.Errors!["Campo"].ToList());
        }

        [TestMethod]
        public void WithMessages_Passing_NullOVacia_Returns_SinMensajes()
        {
            Assert.IsNull(new ResultOptionsError().WithMessages(null!).ResultSettings.Messages);
            Assert.IsNull(new ResultOptionsError().WithMessages(Array.Empty<string>()).ResultSettings.Messages);
            Assert.IsNull(new ResultOptionsError().WithMessages(Perezosa<string>()).ResultSettings.Messages);
        }

        [TestMethod]
        public void WithMessages_Passing_ColeccionPerezosa_Returns_Mensajes()
        {
            var actual = new ResultOptionsError().WithMessages(Perezosa("a", "b")).ResultSettings.Messages;

            CollectionAssert.AreEqual(new[] { "a", "b" }, actual!.ToList());
        }

        [TestMethod]
        public void WithErrors_Passing_NullOVacio_Returns_SinErrores()
        {
            Assert.IsNull(new ResultOptionsError().WithErrors((IDictionary<string, IEnumerable<string>>?)null).ResultSettings.Errors);
            Assert.IsNull(new ResultOptionsError().WithErrors(new Dictionary<string, IEnumerable<string>>()).ResultSettings.Errors);
            Assert.IsNull(new ResultOptionsError().WithErrors((IEnumerable<(string, string)>)null!).ResultSettings.Errors);
            Assert.IsNull(new ResultOptionsError().WithErrors(Array.Empty<(string, string)>()).ResultSettings.Errors);
            Assert.IsNull(new ResultOptionsError().WithErrors(Perezosa<(string, string)>()).ResultSettings.Errors);
        }

        [TestMethod]
        public void WithErrors_Passing_TuplasPerezosas_Returns_Errores()
        {
            var actual = new ResultOptionsError().WithErrors(Perezosa(("Campo", "m"))).ResultSettings.Errors;

            CollectionAssert.AreEqual(new[] { "m" }, actual!["Campo"].ToList());
        }

        #endregion Verify.ToThrowIf y ToIsNullOrAny

        #region ToIsNullOrEmpty

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\t\n")]
        public void WithMessage_Passing_NullVacioOEspacios_Returns_SeIgnora(string? message)
        {
            var actual = new ResultOptionsError().WithMessage(message!).WithMessage("m").ResultSettings.Messages;

            CollectionAssert.AreEqual(new[] { "m" }, actual!.ToList());
        }

        [TestMethod]
        public void Error_Passing_SoloMensajesEnBlanco_Throws_ArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => ApiResult.Error(x => x.WithMessage(" ").WithMessages(new[] { string.Empty })));
        }

        #endregion ToIsNullOrEmpty

        #region ToString(", ")

        [TestMethod]
        public void BadRequest_Passing_VariosMensajes_Returns_MensajesUnidosConComa()
        {
            Assert.AreEqual("a, b, c", ApiResult.BadRequest(new[] { "a", "b", "a", "c" }).Message);
            Assert.AreEqual("a", ApiResult.BadRequest("a").Message);
        }

        #endregion ToString(", ")

        #region ToAddOrUpdate

        [TestMethod]
        public void WithErrors_Passing_Tuplas_Returns_CamposEnOrdenConSusMensajesSinRepetir()
        {
            var tuplas = new[]
            {
                ("Email", "m1"),
                ("Nombre", "n1"),
                ("Email", "m1"),
                ("Email", "m2"),
                ("Nombre", "n1"),
                ("Email", "m3"),
            };

            var actual = new ResultOptionsError().WithErrors(tuplas).ResultSettings.Errors!;

            CollectionAssert.AreEqual(new[] { "Email", "Nombre" }, actual.Keys.ToList());
            CollectionAssert.AreEqual(new[] { "m1", "m2", "m3" }, actual["Email"].ToList());
            CollectionAssert.AreEqual(new[] { "n1" }, actual["Nombre"].ToList());
        }

        [TestMethod]
        public void WithErrors_Passing_CampoRepetidoConOtroMensaje_Returns_LosDosMensajes()
        {
            // Hasta la 1.2.0 (heredado de ToAddOrUpdate de Shared 2.0.1) se quedaba sólo con "m2".
            var actual = new ResultOptionsError().WithErrors(new[] { ("Email", "m1"), ("Email", "m2") }).ResultSettings.Errors!;

            CollectionAssert.AreEqual(new[] { "m1", "m2" }, actual["Email"].ToList());
        }

        [TestMethod]
        public void WithErrors_Passing_TuplasYDiccionario_Returns_MensajesSumados()
        {
            var actual = new ResultOptionsError()
                .WithErrors(new[] { ("Email", "m1"), ("Email", "m2") })
                .WithErrors(new Dictionary<string, IEnumerable<string>> { ["Email"] = new[] { "m2", "m3" } })
                .WithErrors(new[] { ("Email", "m4") })
                .ResultSettings.Errors!;

            CollectionAssert.AreEqual(new[] { "m1", "m2", "m3", "m4" }, actual["Email"].ToList());
        }

        [TestMethod]
        public void BadRequest_Passing_TuplasConCampoRepetido_Returns_TodosLosMensajes()
        {
            var actual = ApiResult.BadRequest(new[] { ("Email", "m1"), ("Email", "m2") });

            CollectionAssert.AreEqual(new[] { "m1", "m2" }, actual.Errors!["Email"].ToList());
        }

        [TestMethod]
        public void WithErrors_Passing_CampoRepetidoConElMismoMensaje_Returns_UnSoloMensaje()
        {
            var actual = new ResultOptionsError().WithErrors(new[] { ("Email", "m1"), ("Email", "m1") }).ResultSettings.Errors!;

            CollectionAssert.AreEqual(new[] { "m1" }, actual["Email"].ToList());
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void WithErrors_Passing_CampoNullVacioOEspacios_Throws_ArgumentException(string? campo)
        {
            var exception = Assert.ThrowsExactly<ArgumentException>(() => new ResultOptionsError().WithErrors(new[] { (campo!, "m") }));

            Assert.AreEqual("El campo key es requerido", exception.Message);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void WithErrors_Passing_MensajeNullVacioOEspacios_Throws_ArgumentException(string? mensaje)
        {
            var exception = Assert.ThrowsExactly<ArgumentException>(() => new ResultOptionsError().WithErrors(new[] { ("Campo", mensaje!) }));

            Assert.AreEqual("El campo value es requerido", exception.Message);
        }

        [TestMethod]
        [DataRow("es-ES")]
        [DataRow("en-US")]
        [DataRow("fr-FR")]
        public void WithErrors_Passing_CampoOMensajeVacio_Returns_MismoMensajeQueShared(string cultura)
        {
            ConCultura(cultura, () =>
            {
                IDictionary<string, IEnumerable<string>> original = new Dictionary<string, IEnumerable<string>>();

                var sharedKey = Assert.ThrowsExactly<Kitpymes.Core.Shared.Util.SharedException>(() => original.ToAddOrUpdate(string.Empty, "m"));
                var sharedValue = Assert.ThrowsExactly<Kitpymes.Core.Shared.Util.SharedException>(() => original.ToAddOrUpdate("Campo", string.Empty));

                var key = Assert.ThrowsExactly<ArgumentException>(() => new ResultOptionsError().WithErrors(new[] { (string.Empty, "m") }));
                var value = Assert.ThrowsExactly<ArgumentException>(() => new ResultOptionsError().WithErrors(new[] { ("Campo", string.Empty) }));

                Assert.AreEqual(sharedKey.Message, key.Message);
                Assert.AreEqual(sharedValue.Message, value.Message);
            });
        }

        #endregion ToAddOrUpdate

        #region Guid.ToSecuencial

        [TestMethod]
        public void Error_Passing_Mensaje_Returns_TraceIdSecuencialConLaHoraActual()
        {
            var antes = DateTime.UtcNow.Ticks;

            var actual = ApiResult.Error(x => x.WithMessage("m"));

            var despues = DateTime.UtcNow.Ticks;

            Assert.IsTrue(Guid.TryParse(actual.TraceId, out var traceId));

            var bytes = traceId.ToByteArray();

            // Shared colocaba los 8 bytes del contador (ticks + 1, little endian) en este orden.
            var contador = new byte[8];
            contador[1] = bytes[8];
            contador[0] = bytes[9];
            contador[7] = bytes[10];
            contador[6] = bytes[11];
            contador[5] = bytes[12];
            contador[4] = bytes[13];
            contador[3] = bytes[14];
            contador[2] = bytes[15];

            var ticks = BitConverter.ToInt64(contador, 0);

            Assert.IsTrue(ticks >= antes + 1 && ticks <= despues + 1, $"{antes} <= {ticks} - 1 <= {despues}");
        }

        [TestMethod]
        public void Error_Passing_DosLlamadas_Returns_TraceIdDistintos()
        {
            var ids = Enumerable.Range(0, 50).Select(_ => ApiResult.Error(x => x.WithMessage("m")).TraceId).ToList();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
            Assert.IsTrue(ids.All(x => Guid.TryParse(x, out var g) && g != Guid.Empty));
        }

        #endregion Guid.ToSecuencial

        #region Dependencias del ensamblado

        [TestMethod]
        public void Ensamblado_Passing_Referencias_Returns_SinSharedNiAspNetCore()
        {
            var referencias = typeof(ApiResult).Assembly.GetReferencedAssemblies().Select(x => x.Name!).ToList();

            Assert.IsFalse(referencias.Any(x => x.StartsWith("Kitpymes.Core.Shared", StringComparison.Ordinal)), string.Join(", ", referencias));
            Assert.IsFalse(referencias.Any(x => x.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal)), string.Join(", ", referencias));
        }

        #endregion Dependencias del ensamblado

        private static void ConCultura(string cultura, Action action)
        {
            var anterior = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentUICulture = cultura.Length == 0 ? CultureInfo.InvariantCulture : new CultureInfo(cultura);

                action();
            }
            finally
            {
                CultureInfo.CurrentUICulture = anterior;
            }
        }

        private static IEnumerable<T> Perezosa<T>(params T[] items)
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }
}
