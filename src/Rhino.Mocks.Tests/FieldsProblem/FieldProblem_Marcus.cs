using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Hosting;
using NUnit.Framework;
using Rhino.Mocks;


namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_ByMarcus
    {
        MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void RepositoryWithoutMocks_ReturnWithoutException()
        {
            //nothing here, it works all in the setup/teardown		
        }

        [Test]
        public void ConstructorThatThrowsInMock()
        {

            try
            {
                ClassWithThrowingCtor c = mocks.CreateMock(typeof(ClassWithThrowingCtor)) as ClassWithThrowingCtor;
                Assert.IsNotNull(c);
                Assert.Fail("Exception expected");
            }
            catch (Exception e)
            {
                string expectedExceptionStartsWith = @"Exception in constructor: System.Exception: I'm a ctor that throws";
                string actualExceptionStartString = e.Message.Substring(0,expectedExceptionStartsWith.Length);
                Assert.AreEqual(expectedExceptionStartsWith, actualExceptionStartString);

            }
        }

        public class ClassWithThrowingCtor
        {
            public ClassWithThrowingCtor()
            {
                throw new Exception("I'm a ctor that throws");
            }
        }

    }
}
