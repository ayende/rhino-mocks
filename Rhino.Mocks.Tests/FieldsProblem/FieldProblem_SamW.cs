using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_SamW
    {
        [Fact]
        public void UsingArrayAndOutParam()
        {
            MockRepository mockRepository = new MockRepository();
            ITest test = mockRepository.StrictMock<ITest>();
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
