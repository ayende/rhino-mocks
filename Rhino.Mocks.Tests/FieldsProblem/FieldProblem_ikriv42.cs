using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_ikriv42
    {
        public class D : MarshalByRefObject
        {
            public D(IConvertible dep1, IDisposable dep2, ICloneable dep3)
            {
            }
        }

       [Fact]
       public void GenerateMock_Works_With_MarshalByRef()
       {
           MockRepository.GenerateMock<D>();
       }

        [Fact]
        public void GenerateStub_Works_With_MarshalByRef()
        {
            MockRepository.GenerateStub<D>();
        }
    }
}