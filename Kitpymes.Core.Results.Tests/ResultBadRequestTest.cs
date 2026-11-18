using Kitpymes.Core.Results.Tests.Fakes;
using Kitpymes.Core.Shared;
using Kitpymes.Core.Shared.Extensions;
using Kitpymes.Core.Shared.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Kitpymes.Core.Results.Tests
{
    [TestClass]
    public class ResultBadRequestTest
    {
        [TestMethod]
        public void BadRequest_WithMessage()
        {
            var messageExpected = Guid.NewGuid().ToString();

            var actual = ApiResult.BadRequest(messageExpected);

            Assert.IsFalse(actual.Success);
            Assert.AreEqual(actual.Status, HttpStatusCode.BadRequest.ToValue());
            Assert.AreEqual(actual.Title, SharedResource.MsgValidationsError);
            Assert.AreEqual(actual.Message, messageExpected);
        }

        [TestMethod]
        public void BadRequest_WithMessage_WithDetails()
        {
            var messageExpected = Guid.NewGuid().ToString();
            var detailsExpected = new FakeUser { Age = 13, Permissions = ["read", "create"], Email = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };

            var actual = ApiResult.BadRequest(messageExpected, detailsExpected);
            var actualToJson = actual.ToSerialize();

            Assert.IsFalse(actual.Success);
            Assert.AreEqual(actual.Status, HttpStatusCode.BadRequest.ToValue());
            Assert.AreEqual(actual.Title, SharedResource.MsgValidationsError);
            Assert.AreEqual(actual.Message, messageExpected);
            Assert.AreEqual(actual.Details, detailsExpected);
            Assert.IsTrue(actualToJson.Contains(messageExpected, StringComparison.CurrentCulture));
        }

        [TestMethod]
        public void BadRequest_WithMessages_List()
        {
            var stringField = FakeTypes.ReferenceTypes.ClassTypes.String_Null;
            var classField = FakeTypes.ReferenceTypes.ClassTypes.Class_Null;
            var classFieldMessageExpected = Messages.NullOrEmpty(nameof(classField));

            var messages = new List<string>();

            if (stringField.ToIsNullOrEmpty())
            {
                messages.Add(Messages.NullOrEmpty(nameof(stringField)));
            }

            if (!stringField.ToIsEmail())
            {
                messages.Add(Messages.InvalidFormat(nameof(stringField)));
            }

            if (classField.ToIsNullOrEmpty())
            {
                messages.Add(classFieldMessageExpected);
            }

            var actual = ApiResult.BadRequest(messages);

            Assert.IsFalse(actual.Success);
            Assert.AreEqual(actual.Status, HttpStatusCode.BadRequest.ToValue());
            Assert.AreEqual(actual.Title, SharedResource.MsgValidationsError);
            Assert.IsTrue(actual.Message!.Contains(Messages.NullOrEmpty(nameof(stringField))));
            Assert.IsTrue(actual.Message.Contains(Messages.NullOrEmpty(nameof(stringField))));
            Assert.IsTrue(actual.Message.Contains(classFieldMessageExpected));
        }

        [TestMethod]
        public void BadRequest_WithMessages_List_Tuple()
        {
            var stringField = FakeTypes.ReferenceTypes.ClassTypes.String_Null;
            var classField = FakeTypes.ReferenceTypes.ClassTypes.Class_Null;
            var classFieldMessageExpected = Messages.NullOrEmpty(nameof(classField));

            var errors = new List<(string fieldName, string message)>();

            if (stringField.ToIsNullOrEmpty())
            {
                errors.Add((nameof(stringField), Messages.NullOrEmpty(nameof(stringField))));
            }

            if (!stringField.ToIsEmail())
            {
                errors.Add((nameof(stringField), Messages.InvalidFormat(nameof(stringField))));
            }

            if (classField.ToIsNullOrEmpty())
            {
                errors.Add((nameof(classField), classFieldMessageExpected));
            }

            var actual = ApiResult.BadRequest(errors);

            Assert.IsFalse(actual.Success);
            Assert.AreEqual(actual.Status, HttpStatusCode.BadRequest.ToValue());
            Assert.AreEqual(actual.Title, SharedResource.MsgValidationsError);
            Assert.IsTrue(actual.Errors.ToContainsKeyValue(nameof(stringField), Messages.InvalidFormat(nameof(stringField))));
            Assert.IsTrue(actual.Errors.ToContainsKeyValue(nameof(classField), classFieldMessageExpected));
        }

        [TestMethod]
        public void BadRequest_WithMessages_List_Dictionary()
        {
            var stringField = FakeTypes.ReferenceTypes.ClassTypes.String_Null;
            var classField = FakeTypes.ReferenceTypes.ClassTypes.Class_Null;
            var classFieldMessageExpected = Messages.NullOrEmpty(nameof(classField));

            var errors = new Dictionary<string, IEnumerable<string>>();

            if (stringField.ToIsNullOrEmpty())
            {
                errors.Add(nameof(stringField), new List<string> { Messages.NullOrEmpty(nameof(stringField)) });
            }

            if (!stringField.ToIsEmail())
            {
                errors[nameof(stringField)] = errors[nameof(stringField)].Concat(new string[] { Messages.InvalidFormat(nameof(stringField)) });
            }

            if (classField.ToIsNullOrEmpty())
            {
                errors.Add(nameof(classField), new List<string> { classFieldMessageExpected });
            }

            var actual = ApiResult.BadRequest(errors);

            Assert.IsFalse(actual.Success);
            Assert.AreEqual(actual.Status, HttpStatusCode.BadRequest.ToValue());
            Assert.AreEqual(actual.Title, SharedResource.MsgValidationsError);
            Assert.IsTrue(actual.Errors.ToContainsKeyValue(nameof(stringField), Messages.NullOrEmpty(nameof(stringField))));
            Assert.IsTrue(actual.Errors.ToContainsKeyValue(nameof(stringField), Messages.InvalidFormat(nameof(stringField))));
            Assert.IsTrue(actual.Errors.ToContainsKeyValue(nameof(classField), classFieldMessageExpected));
        }
    }
}