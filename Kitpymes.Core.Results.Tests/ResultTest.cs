using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kitpymes.Core.Results.Tests
{
    [TestClass]
    public class ResultTest
    {
        private static readonly Error NotFound = Error.NotFound("Users.NoExiste", "El usuario no existe.");

        [TestMethod]
        public void Success_Passing_Nothing_Returns_SuccessWithoutError()
        {
            var actual = Result.Success();

            Assert.IsTrue(actual.IsSuccess);
            Assert.IsFalse(actual.IsFailure);
            Assert.IsNull(actual.Error);
        }

        [TestMethod]
        public void Success_Passing_Nothing_Returns_SameInstance()
        {
            Assert.AreSame(Result.Success(), Result.Success());
        }

        [TestMethod]
        public void Failure_Passing_Error_Returns_FailureWithThatError()
        {
            var actual = Result.Failure(NotFound);

            Assert.IsFalse(actual.IsSuccess);
            Assert.IsTrue(actual.IsFailure);
            Assert.AreSame(NotFound, actual.Error);
        }

        [TestMethod]
        public void Failure_Passing_Null_Throws_ArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => Result.Failure(null!));
        }

        [TestMethod]
        public void ImplicitConversion_Passing_Error_Returns_Failure()
        {
            Result actual = NotFound;

            Assert.IsTrue(actual.IsFailure);
            Assert.AreSame(NotFound, actual.Error);
        }

        [TestMethod]
        public void SuccessOfT_Passing_Value_Returns_SuccessWithValue()
        {
            var actual = Result.Success(42);

            Assert.IsTrue(actual.IsSuccess);
            Assert.IsFalse(actual.IsFailure);
            Assert.IsNull(actual.Error);
            Assert.AreEqual(42, actual.Value);
        }

        [TestMethod]
        public void SuccessOfT_Passing_ValueFromGenericType_Returns_SameAsNonGenericFactory()
        {
            var actual = Result<string>.Success("a");

            Assert.IsTrue(actual.IsSuccess);
            Assert.AreEqual("a", actual.Value);
        }

        [TestMethod]
        public void FailureOfT_Passing_Error_Returns_FailureWithThatError()
        {
            var actual = Result<int>.Failure(NotFound);

            Assert.IsFalse(actual.IsSuccess);
            Assert.IsTrue(actual.IsFailure);
            Assert.AreSame(NotFound, actual.Error);
        }

        [TestMethod]
        public void FailureOfT_Passing_Null_Throws_ArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => Result<int>.Failure(null!));
        }

        [TestMethod]
        public void ImplicitConversionOfT_Passing_Error_Returns_Failure()
        {
            Result<int> actual = NotFound;

            Assert.IsTrue(actual.IsFailure);
            Assert.AreSame(NotFound, actual.Error);
        }

        [TestMethod]
        public void Value_Passing_Failure_Throws_InvalidOperationException()
        {
            var result = Result<int>.Failure(NotFound);

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => _ = result.Value);

            StringAssert.Contains(exception.Message, "Users.NoExiste");
        }

        [TestMethod]
        public void TryGetValue_Passing_Success_Returns_TrueAndValue()
        {
            var ok = Result.Success("a").TryGetValue(out var value);

            Assert.IsTrue(ok);
            Assert.AreEqual("a", value);
        }

        [TestMethod]
        public void TryGetValue_Passing_Failure_Returns_FalseAndDefault()
        {
            var ok = Result<int>.Failure(NotFound).TryGetValue(out var value);

            Assert.IsFalse(ok);
            Assert.AreEqual(default, value);
        }

        [TestMethod]
        public void ResultOfT_Passing_AsBaseResult_Returns_SameState()
        {
            Result asBase = Result<int>.Failure(NotFound);

            Assert.IsTrue(asBase.IsFailure);
            Assert.AreSame(NotFound, asBase.Error);
        }

        [TestMethod]
        public void FailureFor_Passing_Result_Returns_NonGenericFailure()
        {
            var actual = Result.FailureFor<Result>(NotFound);

            Assert.AreEqual(typeof(Result), actual.GetType());
            Assert.AreSame(NotFound, actual.Error);
        }

        [TestMethod]
        public void FailureFor_Passing_ResultOfValueType_Returns_GenericFailure()
        {
            var actual = Result.FailureFor<Result<int>>(NotFound);

            Assert.IsTrue(actual.IsFailure);
            Assert.AreSame(NotFound, actual.Error);
        }

        [TestMethod]
        public void FailureFor_Passing_ResultOfReferenceType_Returns_GenericFailure()
        {
            var actual = Result.FailureFor<Result<string>>(NotFound);

            Assert.IsTrue(actual.IsFailure);
            Assert.AreSame(NotFound, actual.Error);
        }

        [TestMethod]
        public void FailureFor_Passing_SameTypeTwice_Returns_IndependentFailures()
        {
            var other = Error.Conflict("Users.EmailDuplicado", "El email ya existe.");

            var first = Result.FailureFor<Result<int>>(NotFound);
            var second = Result.FailureFor<Result<int>>(other);

            Assert.AreSame(NotFound, first.Error);
            Assert.AreSame(other, second.Error);
        }

        [TestMethod]
        public void FailureFor_Passing_Null_Throws_ArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => Result.FailureFor<Result<int>>(null!));
        }
    }
}
