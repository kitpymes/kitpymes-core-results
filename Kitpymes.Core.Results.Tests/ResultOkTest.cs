using Kitpymes.Core.Results.Tests.Fakes;
using Kitpymes.Core.Shared;
using Kitpymes.Core.Shared.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace Kitpymes.Core.Results.Tests
{
    [TestClass]
    public class ResultOkTest
    {
        [TestMethod]
        public void OK()
        {
            var actual = ApiResult.Ok();

            Assert.IsTrue(actual.Success);
            Assert.AreEqual(actual.Status, HttpStatusCode.OK.ToValue());
            Assert.AreEqual(actual.Title, SharedResource.MsgProcessRanSuccessfully);
        }

        [TestMethod]
        public void Ok_WithTitle()
        {
            var titleExpected = Guid.NewGuid().ToString();

            var actual = ApiResult.Ok(x => x.WithTitle(titleExpected));

            Assert.IsTrue(actual.Success);
            Assert.AreEqual(actual.Status, HttpStatusCode.OK.ToValue());
            Assert.AreEqual(actual.Title, titleExpected);
        }

        [TestMethod]
        public void Ok_WithData()
        {
            var dataExpected = new FakeUser { Age = 13, Permissions = ["read", "create"], Email = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };

            var actual = ApiResult.Ok(x => x.WithData(dataExpected));
            var actualToJson = actual.ToSerialize();

            Assert.IsTrue(actual.Success);
            Assert.AreEqual(actual.Status, HttpStatusCode.OK.ToValue());
            Assert.AreEqual(actual.Title, SharedResource.MsgProcessRanSuccessfully);
            Assert.AreEqual(actual.Data, dataExpected);
        }

        [TestMethod]
        public void Ok_WithTitle_WithData()
        {
            var titleExpected = Guid.NewGuid().ToString();
            var dataExpected = new FakeUser { Age = 13, Permissions = ["read", "create"], Email = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };

            var actual = ApiResult.Ok(x => x.WithTitle(titleExpected).WithData(dataExpected));
            var actualToJson = actual.ToSerialize();

            Assert.IsTrue(actual.Success);
            Assert.AreEqual(actual.Status, HttpStatusCode.OK.ToValue());
            Assert.AreEqual(actual.Title, titleExpected);
            Assert.AreEqual(actual.Data, dataExpected);
        }
    }
}