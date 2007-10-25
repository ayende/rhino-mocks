using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_SamW
    {
        [Test]
        public void UsingArrayAndOutParam()
        {
            MockRepository mockRepository = new MockRepository();
            ITest test = mockRepository.CreateMock<ITest>();
            string b;
            test.ArrayWithOut(new string[] { "data" }, out b);

            LastCall.Return("SuccessWithOut1").OutRef("SuccessWithOut2");

            mockRepository.ReplayAll();
            Console.WriteLine(test.ArrayWithOut(new string[] { "data" }, out b));
            Console.WriteLine(b);
        }


        public interface ITest
        {
            string ArrayWithOut(string[] a, out string b);
        }


    }
}
