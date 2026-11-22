using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kitpymes.Core.Results.Tests
{
    [TestClass]
    public class ErrorTest
    {
        [TestMethod]
        public void Factories_Passing_CodeAndMessage_Returns_ErrorWithItsType()
        {
            var cases = new (Func<string, string, Error> factory, ErrorType expected)[]
            {
                ((c, m) => Error.Validation(c, m), ErrorType.Validation),
                (Error.NotFound, ErrorType.NotFound),
                (Error.Conflict, ErrorType.Conflict),
                (Error.Unauthorized, ErrorType.Unauthorized),
                (Error.Forbidden, ErrorType.Forbidden),
                (Error.NotContracted, ErrorType.NotContracted),
                (Error.Unexpected, ErrorType.Unexpected),
            };

            foreach (var (factory, expected) in cases)
            {
                var actual = factory("Users.Motivo", "Mensaje.");

                Assert.AreEqual(expected, actual.Type);
                Assert.AreEqual("Users.Motivo", actual.Code);
                Assert.AreEqual("Mensaje.", actual.Message);
                Assert.AreEqual(0, actual.Fields.Count);
            }
        }

        [TestMethod]
        public void Validation_Passing_Fields_Returns_ErrorWithFieldsInOrder()
        {
            var fields = new List<FieldError>
            {
                new ("Email", "Validations.Requerido", "El email es obligatorio."),
                new ("Nombre", "Validations.LongitudMaxima", "El nombre es demasiado largo."),
            };

            var actual = Error.Validation("Users.DatosInvalidos", "Datos inválidos.", fields);

            Assert.AreEqual(2, actual.Fields.Count);
            Assert.AreEqual("Email", actual.Fields[0].Field);
            Assert.AreEqual("Validations.LongitudMaxima", actual.Fields[1].Code);
        }

        [TestMethod]
        public void Validation_Passing_FieldsAndChangingSourceAfter_Returns_ErrorUnchanged()
        {
            var fields = new List<FieldError> { new ("Email", "Validations.Requerido", "Obligatorio.") };

            var actual = Error.Validation("Users.DatosInvalidos", "Datos inválidos.", fields);
            fields.Add(new ("Nombre", "Validations.Requerido", "Obligatorio."));

            Assert.AreEqual(1, actual.Fields.Count);
        }

        [TestMethod]
        public void Validation_Passing_FieldsWithNull_Throws_ArgumentException()
        {
            var fields = new FieldError[] { null! };

            Assert.ThrowsExactly<ArgumentException>(() => Error.Validation("Users.DatosInvalidos", "Datos inválidos.", fields));
        }

        [TestMethod]
        [DataRow("Users.EmailDuplicado")]
        [DataRow("Core.Unexpected")]
        [DataRow("Validations.Requerido")]
        [DataRow("Users.Roles.NoAsignado")]
        [DataRow("Module2.Motivo3")]
        public void NotFound_Passing_ValidCode_Returns_Error(string code)
        {
            Assert.AreEqual(code, Error.NotFound(code, "Mensaje.").Code);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Users")]
        [DataRow("Users.")]
        [DataRow(".Motivo")]
        [DataRow("users.EmailDuplicado")]
        [DataRow("Users.emailDuplicado")]
        [DataRow("Users.Email Duplicado")]
        [DataRow("Users.Email-Duplicado")]
        [DataRow("Users.ÉmailDuplicado")]
        [DataRow("Users..Motivo")]
        [DataRow("2Users.Motivo")]
        public void NotFound_Passing_InvalidCode_Throws_ArgumentException(string? code)
        {
            Assert.ThrowsExactly<ArgumentException>(() => Error.NotFound(code!, "Mensaje."));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void NotFound_Passing_EmptyMessage_Throws_ArgumentException(string? message)
        {
            Assert.ThrowsExactly<ArgumentException>(() => Error.NotFound("Users.NoExiste", message!));
        }

        [TestMethod]
        public void ToString_Passing_Error_Returns_TypeCodeAndMessage()
        {
            var actual = Error.Conflict("Users.EmailDuplicado", "El email ya existe.").ToString();

            Assert.AreEqual("Conflict Users.EmailDuplicado: El email ya existe.", actual);
        }

        [TestMethod]
        public void FieldError_Passing_Values_Returns_SameValues()
        {
            var actual = new FieldError("Email", "Validations.Requerido", "Obligatorio.");

            Assert.AreEqual("Email", actual.Field);
            Assert.AreEqual("Validations.Requerido", actual.Code);
            Assert.AreEqual("Obligatorio.", actual.Message);
            Assert.AreEqual("Email: Validations.Requerido - Obligatorio.", actual.ToString());
        }

        [TestMethod]
        [DataRow(null, "Validations.Requerido", "Obligatorio.", "field")]
        [DataRow(" ", "Validations.Requerido", "Obligatorio.", "field")]
        [DataRow("Email", "", "Obligatorio.", "code")]
        [DataRow("Email", "requerido", "Obligatorio.", "code")]
        [DataRow("Email", "Validations.Campo Requerido", "Obligatorio.", "code")]
        [DataRow("Email", "Validations.Requerido", null, "message")]
        public void FieldError_Passing_EmptyValue_Throws_ArgumentExceptionWithItsName(string? field, string? code, string? message, string expectedParam)
        {
            var exception = Assert.ThrowsExactly<ArgumentException>(() => new FieldError(field!, code!, message!));

            Assert.AreEqual(expectedParam, exception.ParamName);
        }

        [TestMethod]
        public void ErrorType_Passing_AllValues_Returns_SevenStableValues()
        {
            var values = Enum.GetValues<ErrorType>().Select(x => (int)x).ToArray();

            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6 }, values);
        }
    }
}
