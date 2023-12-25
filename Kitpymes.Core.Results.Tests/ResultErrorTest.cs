using Kitpymes.Core.Results.Tests.Fakes;
using Kitpymes.Core.Shared.Extensions;
using Kitpymes.Core.Shared.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Kitpymes.Core.Results.Tests
{
    [TestClass]
    public class ResultErrorTest
    {
        [TestMethod]
        public void ResultError_WithTitle_WithStatusCode_WithDetails_WithExceptionType_WithMessages_WithErrors()
        {
            var stringField = FakeTypes.ReferenceTypes.ClassTypes.String_Null;
            var classField = FakeTypes.ReferenceTypes.ClassTypes.Class_Null;

            var classFieldMessageExpected = Messages.FieldNullOrEmpty(nameof(classField));
            var titleExpected = Guid.NewGuid().ToString();
            var messageExpected = Guid.NewGuid().ToString();
            var statusCodeExpected = HttpStatusCode.BadRequest;
            var exceptionTypeExpected = nameof(ArrayTypeMismatchException);
            object detailsExpected = new
            {
                Code = new Random().Next(),
                Link = Guid.NewGuid().ToString(),
                Otro = Guid.NewGuid().ToString(),
            };

            var errors = new List<(string fieldName, string message)>();

            if (stringField.ToIsNullOrEmpty())
            {
                errors.Add((nameof(stringField), Messages.FieldNullOrEmpty(nameof(stringField))));
            }

            if (!stringField.ToIsEmail())
            {
                errors.Add((nameof(stringField), Messages.FieldInvalidFormat(nameof(stringField))));
            }

            if (classField.ToIsNullOrEmpty())
            {
                errors.Add((nameof(classField), classFieldMessageExpected));
            }

            var messages = new List<string>(errors.Select(x => x.message));

            ApiResult? actual = null;

            if (errors.Count != 0)
            {
                actual = ApiResult.Error(options => options
                        .WithTitle(titleExpected)
                        .WithStatusCode(statusCodeExpected)
                        .WithDetails(detailsExpected)
                        .WithExceptionType(exceptionTypeExpected)
                        .WithMessage(messageExpected)
                        .WithMessages(messages)
                        .WithErrors(errors));
            }

            if(actual is null)
            {
                return;
            }

            var actualJson = actual.ToSerialize();

            Assert.IsFalse(actual.Success);

            Assert.IsTrue(actual.Errors.ToContainsKeyValue(nameof(stringField), Messages.FieldInvalidFormat(nameof(stringField))));
            Assert.IsTrue(actual.Errors.ToContainsKeyValue(nameof(classField), classFieldMessageExpected));

            Assert.IsTrue(actual.Message?.Contains(Messages.FieldInvalidFormat(nameof(stringField))));
            Assert.IsTrue(actual.Message?.Contains(classFieldMessageExpected));

            Assert.IsTrue(actualJson.Contains(Messages.FieldInvalidFormat(nameof(stringField)), StringComparison.CurrentCulture));
            Assert.IsTrue(actualJson.Contains(classFieldMessageExpected, StringComparison.CurrentCulture));

            Assert.AreEqual(actual.Title, titleExpected);
            Assert.IsTrue(actualJson.Contains(titleExpected, StringComparison.CurrentCulture));

            Assert.AreEqual(actual.Status, statusCodeExpected.ToValue());
            Assert.IsTrue(actualJson.Contains(statusCodeExpected.ToValue().ToString(), StringComparison.CurrentCulture));

            Assert.AreEqual(actual.Details, detailsExpected);

            Assert.AreEqual(actual.Exception, exceptionTypeExpected);
            Assert.IsTrue(actualJson.Contains(exceptionTypeExpected, StringComparison.CurrentCulture));
        }
    }
}