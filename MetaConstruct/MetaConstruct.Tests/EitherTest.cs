using MetaCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MetaContruct.Tests {
    [TestFixture]
    public class EitherTest {
        [Test]
        public void Deconstruct0() {
            Either<bool, string> either = true.AsLeft();
            string Switch() => either switch {
                (bool left, null) => left.ToString(),
            };
            Assert.AreEqual("True", Switch());

            either = "val".AsRight();
            Assert.Throws<SwitchExpressionException>(() => Switch());
        }

        [Test]
        public void Deconstruct1() {
            Either<bool, string> either = true.AsLeft();
            string Switch() => either switch {
                (bool left, null) => left.ToString(),
                (null, string right) => right,
            };
            Assert.AreEqual("True", Switch());

            either = "val".AsRight();
            Assert.AreEqual("val", Switch());
        }

        [Test]
        public void Deconstruct1_() {
            Either<string, bool> either = true.AsRight();
            string Switch() => either switch {
                (null, bool left) => left.ToString(),
                (string right, null) => right,
            };
            Assert.AreEqual("True", Switch());

            either = "val".AsLeft();
            Assert.AreEqual("val", Switch());
        }

        [Test]
        public void Deconstruct2() {
            Either<int, bool> either = true.AsRight();
            string Switch() => either switch {
                (int left, null) => left.ToString(),
                (null, bool right) => right.ToString(),
            };
            Assert.AreEqual("True", Switch());

            either = 13.AsLeft();
            Assert.AreEqual("13", Switch());
        }

        [Test]
        public void Deconstruct3() {
            Either<Exception, string> either = "val".AsRight();
            string Switch() => either switch {
                (null, string right) => right,
                (Exception left, null) => left.GetType().Name,
            };
            Assert.AreEqual("val", Switch());

            either = new Exception().AsLeft();
            Assert.AreEqual("Exception", Switch());
        }

        [Test]
        public void AsLeftTest() {
            Either<bool, string> either = true.AsLeft();
            Assert.True(either.IsLeft());
            Assert.False(either.IsRight());
            Assert.AreEqual("True", either.Match(left: x => x.ToString(), right: _ => throw new InvalidOperationException()));
            bool? result = null;
            either.Match(left: x => result = x, right: _ => throw new InvalidOperationException());
            Assert.True(result!.Value);

            either = false.AsLeft();
            Assert.True(either.IsLeft());
            Assert.False(either.IsRight());
            Assert.AreEqual("False", either.Match(left: x => x.ToString(), right: _ => throw new InvalidOperationException()));
            result = null;
            either.Match(left: x => result = x, right: _ => throw new InvalidOperationException());
            Assert.False(result!.Value);
        }
        [Test]
        public void AsRightTest() {
            Either<bool, string> either = "bla".AsRight();
            Assert.False(either.IsLeft());
            Assert.True(either.IsRight());
            Assert.AreEqual("bla", either.Match(left: _ => throw new InvalidOperationException(), right: x => x));
            string? result = null;
            either.Match(left: _ => throw new InvalidOperationException(), right: x => result = x);
            Assert.AreEqual("bla", result);
        }
        [Test]
        public void AsEitherFromRightTest() {
            var either = true
                ? "bla".AsRight().AsEither<bool>()
                : false.AsLeft();
            Assert.AreEqual("bla", either.Match(left: (bool _) => throw new InvalidOperationException(), right: (string x) => x));
        }
        [Test]
        public void AsEitherFromLeftTest() {
            var either = false
                ? "bla".AsRight()
                : false.AsLeft().AsEither<string>();
            Assert.AreEqual("False", either.Match(left: (bool x) => x.ToString(), right: (string _) => throw new InvalidOperationException()));
        }
        [Test]
        public void EqualsVirtualTest() {
            Assert.False(Equals(Either<bool, bool>.Left(true), Either<bool, bool>.Right(true)));
            Assert.False(Equals(Either<bool, bool>.Left(true), string.Empty));

            Assert.True(Equals(Either<bool, bool>.Left(true), Either<bool, bool>.Left(true)));
            Assert.True(Equals(Either<bool, bool>.Right(true), Either<bool, bool>.Right(true)));

            Assert.False(Equals(Either<bool, bool>.Right(true), Either<bool, bool>.Right(false)));
            Assert.False(Equals(Either<bool, bool>.Left(true), Either<bool, bool>.Left(false)));

            Assert.False(Equals(Either<string, string>.Left("true"), Either<string, string>.Right("true")));

            Assert.True(Equals(Either<string, string>.Left("true"), Either<string, string>.Left("true")));
            Assert.True(Equals(Either<string, string>.Right("true"), Either<string, string>.Right("true")));

            Assert.False(Equals(Either<string, string>.Right("true"), Either<string, string>.Right("false")));
            Assert.False(Equals(Either<string, string>.Left("true"), Either<string, string>.Left("false")));
        }
        [Test]
        public void GetHashCodeTest() {
            Assert.AreNotEqual(Either<bool, bool>.Left(true).GetHashCode(), Either<bool, bool>.Right(true).GetHashCode());
            Assert.AreEqual(Either<bool, bool>.Left(true).GetHashCode(), Either<bool, bool>.Left(true).GetHashCode());
            Assert.AreEqual(Either<bool, bool>.Right(false).GetHashCode(), Either<bool, bool>.Right(false).GetHashCode());
            Assert.AreNotEqual(Either<bool, bool>.Right(false).GetHashCode(), Either<bool, bool>.Right(true).GetHashCode());
            Assert.AreNotEqual(Either<bool, bool>.Left(false).GetHashCode(), Either<bool, bool>.Left(true).GetHashCode());
        }

        [Test]
        public void EqualsEquatableTest() {
            Assert.False(EqualsEquatable(Either<bool, bool>.Left(true), Either<bool, bool>.Right(true)));

            Assert.True(EqualsEquatable(Either<bool, bool>.Left(true), Either<bool, bool>.Left(true)));
            Assert.True(EqualsEquatable(Either<bool, bool>.Right(true), Either<bool, bool>.Right(true)));

            Assert.False(EqualsEquatable(Either<bool, bool>.Right(true), Either<bool, bool>.Right(false)));
            Assert.False(EqualsEquatable(Either<bool, bool>.Left(true), Either<bool, bool>.Left(false)));

            Assert.False(EqualsEquatable(Either<string, string>.Left("true"), Either<string, string>.Right("true")));

            Assert.True(EqualsEquatable(Either<string, string>.Left("true"), Either<string, string>.Left("true")));
            Assert.True(EqualsEquatable(Either<string, string>.Right("true"), Either<string, string>.Right("true")));

            Assert.False(EqualsEquatable(Either<string, string>.Right("true"), Either<string, string>.Right("false")));
            Assert.False(EqualsEquatable(Either<string, string>.Left("true"), Either<string, string>.Left("false")));
        }
        static bool EqualsEquatable<L, R>(Either<L, R> x, Either<L, R> y) {
            Assert.IsInstanceOf<IEquatable<Either<L, R>>>(x);
            Assert.IsInstanceOf<IEquatable<Either<L, R>>>(y);
            return x.Equals(y);
        }
        [Test]
        public void EqualsOperatorTest() {
            Assert.False(Either<bool, bool>.Left(true) == Either<bool, bool>.Right(true));

            Assert.True(Either<bool, bool>.Left(true) == Either<bool, bool>.Left(true));
            Assert.True(Either<bool, bool>.Right(true) == Either<bool, bool>.Right(true));

            Assert.False(Either<bool, bool>.Right(true) == Either<bool, bool>.Right(false));
            Assert.False(Either<bool, bool>.Left(true) == Either<bool, bool>.Left(false));

            Assert.False(Either<string, string>.Left("true") == Either<string, string>.Right("true"));

            Assert.True(Either<string, string>.Left("true") == Either<string, string>.Left("true"));
            Assert.True(Either<string, string>.Right("true") == Either<string, string>.Right("true"));

            Assert.False(Either<string, string>.Right("true") == Either<string, string>.Right("false"));
            Assert.False(Either<string, string>.Left("true") == Either<string, string>.Left("false"));
        }

        [Test]
        public void NotEqualsOperatorTest() {
            Assert.True(Either<bool, bool>.Left(true) != Either<bool, bool>.Right(true));

            Assert.False(Either<bool, bool>.Left(true) != Either<bool, bool>.Left(true));
            Assert.False(Either<bool, bool>.Right(true) != Either<bool, bool>.Right(true));

            Assert.True(Either<bool, bool>.Right(true) != Either<bool, bool>.Right(false));
            Assert.True(Either<bool, bool>.Left(true) != Either<bool, bool>.Left(false));
        }

        [Test]
        public void ToStringTest() {
            Assert.AreEqual("Right: True", Either<bool, bool>.Right(true).ToString());
            Assert.AreEqual("Left: False", Either<bool, bool>.Left(false).ToString());
            Assert.AreEqual("Left: ", Either<string?, bool>.Left(null).ToString());
            Assert.AreEqual("Right: ", Either<bool, string?>.Right(null).ToString());
        }

        [Test]
        public void DefaultValue() {
            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).ToString());

            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).IsLeft());
            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).IsRight());
            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).Match(left: x => default(object), right: x => default(object)));
            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).Match(left: x => { }, right: x => { }));

            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).Equals(Either<bool, bool>.Left(true)));
            Assert.Throws<InvalidOperationException>(() => Either<bool, bool>.Left(true).Equals(default(Either<bool, bool>)));
            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).Equals(Either<bool, bool>.Left(true)));
            Assert.Throws<InvalidOperationException>(() => Either<bool, bool>.Left(true).Equals(default(Either<bool, bool>)));

            Assert.Throws<InvalidOperationException>(() => Equals(default(Either<bool, bool>), Either<bool, bool>.Left(true)));
            Assert.Throws<InvalidOperationException>(() => Equals(Either<bool, bool>.Left(true), default(Either<bool, bool>)));
            Assert.Throws<InvalidOperationException>(() => Equals(default(Either<bool, bool>), Either<bool, bool>.Left(true)));
            Assert.Throws<InvalidOperationException>(() => Equals(Either<bool, bool>.Left(true), default(Either<bool, bool>)));
            Assert.Throws<InvalidOperationException>(() => Equals(default(Either<bool, bool>), string.Empty));

            Assert.Throws<InvalidOperationException>(() => (default(Either<bool, bool>) == Either<bool, bool>.Left(true)).ToString());
            Assert.Throws<InvalidOperationException>(() => (default(Either<bool, bool>) != Either<bool, bool>.Left(true)).ToString());
            Assert.Throws<InvalidOperationException>(() => (Either<bool, bool>.Left(true) == default(Either<bool, bool>)).ToString());
            Assert.Throws<InvalidOperationException>(() => (Either<bool, bool>.Left(true) != default(Either<bool, bool>)).ToString());

            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).GetHashCode());
        }

        [Test]
        public void ToLeft() {
            Assert.True(Either<bool, bool>.Left(true).ToLeft());
            Assert.False(Either<bool, bool>.Left(false).ToLeft());
            Assert.Throws<InvalidOperationException>(() => Either<bool, bool>.Right(false).ToLeft());
            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).ToLeft());
        }

        [Test]
        public void ToRight() {
            Assert.True(Either<bool, bool>.Right(true).ToRight());
            Assert.False(Either<bool, bool>.Right(false).ToRight());
            Assert.Throws<InvalidOperationException>(() => Either<bool, bool>.Left(false).ToRight());
            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).ToRight());
        }

        [Test]
        public void LeftOrDefault() {
            Assert.True(Either<bool, bool>.Left(true).LeftOrDefault());
            Assert.False(Either<bool, bool>.Left(false).LeftOrDefault());
            Assert.False(Either<bool, bool>.Right(true).LeftOrDefault());
            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).LeftOrDefault());
        }
        [Test]
        public void RightOrDefault() {
            Assert.True(Either<bool, bool>.Right(true).RightOrDefault());
            Assert.False(Either<bool, bool>.Right(false).RightOrDefault());
            Assert.False(Either<bool, bool>.Left(true).RightOrDefault());
            Assert.Throws<InvalidOperationException>(() => default(Either<bool, bool>).RightOrDefault());
        }

        [Test]
        public void UnwrapException() {
            Assert.Throws<ObjectDisposedException>(() => Either<ObjectDisposedException, string>.Left(new ObjectDisposedException("")).UnwrapException());
            Assert.AreEqual("bla", Either<ObjectDisposedException, string>.Right("bla").UnwrapException());
        }
    }
}
