using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;

namespace Kitpymes.Core.Results.Tests
{
    [TestClass]
    public class ResultExtensionsTest
    {
        [TestMethod]
        public void ToApiResult_Passing_Success_Returns_Ok200WithoutData()
        {
            var actual = Result.Success().ToApiResult();

            Assert.IsTrue(actual.Success);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.Status);
            Assert.IsNull(actual.Data);
            Assert.IsNull(actual.Code);
        }

        [TestMethod]
        public void ToApiResult_Passing_SuccessWithValue_Returns_Ok200WithData()
        {
            var actual = Result.Success(42).ToApiResult();

            Assert.IsTrue(actual.Success);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.Status);
            Assert.AreEqual(42, actual.Data);
        }

        [TestMethod]
        [DataRow(ErrorType.Validation, HttpStatusCode.BadRequest)]
        [DataRow(ErrorType.NotFound, HttpStatusCode.NotFound)]
        [DataRow(ErrorType.Conflict, HttpStatusCode.Conflict)]
        [DataRow(ErrorType.Unauthorized, HttpStatusCode.Unauthorized)]
        [DataRow(ErrorType.Forbidden, HttpStatusCode.Forbidden)]
        [DataRow(ErrorType.NotContracted, HttpStatusCode.Forbidden)]
        [DataRow(ErrorType.Unexpected, HttpStatusCode.InternalServerError)]
        public void ToStatusCode_Passing_ErrorType_Returns_HttpStatus(ErrorType type, HttpStatusCode expected)
        {
            Assert.AreEqual(expected, type.ToStatusCode());
        }

        [TestMethod]
        public void ToStatusCode_Passing_UnknownType_Throws_ArgumentOutOfRangeException()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ((ErrorType)99).ToStatusCode());
        }

        [TestMethod]
        public void ToApiResult_Passing_EveryErrorType_Returns_StatusCodeMessageAndTitle()
        {
            foreach (var type in Enum.GetValues<ErrorType>())
            {
                var error = type switch
                {
                    ErrorType.Validation => Error.Validation("Users.Motivo", "Mensaje."),
                    ErrorType.NotFound => Error.NotFound("Users.Motivo", "Mensaje."),
                    ErrorType.Conflict => Error.Conflict("Users.Motivo", "Mensaje."),
                    ErrorType.Unauthorized => Error.Unauthorized("Users.Motivo", "Mensaje."),
                    ErrorType.Forbidden => Error.Forbidden("Users.Motivo", "Mensaje."),
                    ErrorType.NotContracted => Error.NotContracted("Users.Motivo", "Mensaje."),
                    _ => Error.Unexpected("Users.Motivo", "Mensaje."),
                };

                var actual = Result.Failure(error).ToApiResult();

                Assert.IsFalse(actual.Success, type.ToString());
                Assert.AreEqual((int)type.ToStatusCode(), actual.Status, type.ToString());
                Assert.AreEqual("Users.Motivo", actual.Code, type.ToString());
                Assert.AreEqual("Mensaje.", actual.Message, type.ToString());
                Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Title), type.ToString());
                Assert.IsNotNull(actual.TraceId, type.ToString());
            }
        }

        [TestMethod]
        public void ToApiResult_Passing_Unexpected_Returns_NoExceptionDetails()
        {
            var actual = Result.Failure(Error.Unexpected("Core.Unexpected", "Error inesperado.")).ToApiResult();

            Assert.IsNull(actual.Exception);
            Assert.IsNull(actual.Details);
        }

        [TestMethod]
        public void ToApiResult_Passing_ValidationWithFields_Returns_FieldErrorsAndErrorsByField()
        {
            var error = Error.Validation("Users.DatosInvalidos", "Datos inválidos.", new[]
            {
                new FieldError("Email", "Validations.Requerido", "El email es obligatorio."),
                new FieldError("Email", "Validations.Formato", "El email no tiene formato válido."),
                new FieldError("Nombre", "Validations.Requerido", "El nombre es obligatorio."),
            });

            var actual = Result<int>.Failure(error).ToApiResult();

            Assert.AreEqual(3, actual.FieldErrors!.Count);
            Assert.AreEqual("Validations.Formato", actual.FieldErrors[1].Code);
            Assert.AreEqual(2, actual.Errors!.Count);
            CollectionAssert.AreEqual(
                new[] { "El email es obligatorio.", "El email no tiene formato válido." },
                actual.Errors["Email"].ToArray());
            CollectionAssert.AreEqual(new[] { "El nombre es obligatorio." }, actual.Errors["Nombre"].ToArray());
        }

        [TestMethod]
        public void ToApiResult_Passing_ErrorWithoutFields_Returns_NoFieldErrors()
        {
            var actual = Result.Failure(Error.NotFound("Users.NoExiste", "No existe.")).ToApiResult();

            Assert.IsNull(actual.FieldErrors);
            Assert.IsNull(actual.Errors);
        }

        [TestMethod]
        public void ApiResultError_Passing_NoMessagesNorErrors_Throws()
        {
            Assert.Throws<Exception>(() => ApiResult.Error(options => options.WithStatusCode(HttpStatusCode.BadRequest)));
        }

        [TestMethod]
        public void ApiResultBadRequest_Passing_TwoMessages_Returns_MessagesJoinedWithComma()
        {
            var actual = ApiResult.BadRequest(new[] { "Uno", "Dos" });

            Assert.AreEqual("Uno, Dos", actual.Message);
        }

        [TestMethod]
        public void ApiResultError_Passing_WithErrorsTwice_Returns_ErrorsOfBothCalls()
        {
            var actual = ApiResult.Error(options => options
                .WithErrors(new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> { ["Email"] = new[] { "Obligatorio." } })
                .WithErrors(new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> { ["Email"] = new[] { "Formato no válido." }, ["Nombre"] = new[] { "Obligatorio." } }));

            Assert.AreEqual(2, actual.Errors!.Count);
            CollectionAssert.AreEqual(new[] { "Obligatorio.", "Formato no válido." }, actual.Errors["Email"].ToArray());
            CollectionAssert.AreEqual(new[] { "Obligatorio." }, actual.Errors["Nombre"].ToArray());
        }

        [TestMethod]
        public void ApiResultError_Passing_WithErrorsRepeatedMessage_Returns_MessageOnce()
        {
            var actual = ApiResult.Error(options => options
                .WithErrors(new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> { ["Email"] = new[] { "Obligatorio." } })
                .WithErrors(new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> { ["Email"] = new[] { "Obligatorio." } }));

            CollectionAssert.AreEqual(new[] { "Obligatorio." }, actual.Errors!["Email"].ToArray());
        }

        [TestMethod]
        public void ApiResultError_Passing_WithErrorsIgnoreCaseDictionary_Returns_KeysMergedIgnoringCase()
        {
            var first = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>(StringComparer.OrdinalIgnoreCase) { ["Email"] = new[] { "Obligatorio." } };
            var second = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> { ["email"] = new[] { "Formato no válido." } };

            var actual = ApiResult.Error(options => options.WithErrors(first).WithErrors(second));

            Assert.AreEqual(1, actual.Errors!.Count);
            CollectionAssert.AreEqual(new[] { "Obligatorio.", "Formato no válido." }, actual.Errors["EMAIL"].ToArray());
        }

        [TestMethod]
        public void ApiResultError_Passing_WithErrorsNullMessages_Returns_EmptyMessages()
        {
            var actual = ApiResult.Error(options => options
                .WithMessage("Datos inválidos.")
                .WithErrors(new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> { ["Email"] = null! }));

            Assert.AreEqual(0, actual.Errors!["Email"].Count());
        }

        [TestMethod]
        public void ApiResultError_Passing_WithErrorsAndChangingSourceAfter_Returns_ErrorsUnchanged()
        {
            var source = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> { ["Email"] = new[] { "Obligatorio." } };

            var actual = ApiResult.Error(options => options.WithErrors(source));
            source["Nombre"] = new[] { "Obligatorio." };

            Assert.AreEqual(1, actual.Errors!.Count);
        }

        [TestMethod]
        public void ToApiResult_Passing_Null_Throws_ArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => ((Result)null!).ToApiResult());
        }

        [TestMethod]
        public void ApiResultInterface_Passing_ImplementationWithoutNewMembers_Returns_NullDefaults()
        {
            IApiResultError actual = new OldImplementation();

            Assert.IsNull(actual.Code);
            Assert.IsNull(actual.FieldErrors);
        }

        // Implementación escrita contra la 1.0.1: sigue compilando sin los miembros nuevos.
        private sealed class OldImplementation : IApiResultError
        {
            public bool Success => false;

            public int? Status => 400;

            public string? Title => null;

            public string? TraceId => null;

            public string? Exception => null;

            public string? Message => null;

            public System.Collections.Generic.IDictionary<string, System.Collections.Generic.IEnumerable<string>>? Errors => null;

            public object? Details => null;
        }
    }
}
