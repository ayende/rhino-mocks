using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_Kuchia : IDisposable
    {
        private MockRepository _mocks;
        private IProblem _problem;
        private IDaoFactory _daoFactory;
        private IBLFactory _blFactory;

        [Fact]
        public void Method1_CallWithMocks_Returns10()
        {
            int result = Problem.Method1();
            Mocks.ReplayAll();
            Mocks.VerifyAll();
            Assert.Equal(10, result);

        }

        public MockRepository Mocks
        {
            get
            {
                _mocks = _mocks ?? new MockRepository();
                return _mocks;
            }
        }

        public IDaoFactory DaoFactoryMock
        {
            get
            {
                _daoFactory = _daoFactory ?? Mocks.StrictMock<IDaoFactory>();
                return _daoFactory;
            }
        }


        public IBLFactory BLFactoryMock
        {
            get
            {
                _blFactory = _blFactory ?? Mocks.StrictMock<IBLFactory>();
                return _blFactory;
            }
        }


        public IProblem Problem
        {
            get
            {
                _problem = _problem ?? new Problem(BLFactoryMock, DaoFactoryMock);
                return _problem;
            }

        }

        public void Dispose()
        {
            _problem = null;
            _blFactory = null;
            _daoFactory = null;
            _mocks = null;
        }
    }

    public interface IBLFactory
    {

    }

    public interface IDaoFactory
    {
    }

    public interface IProblem
    {
        int Method1();
    }

    public class Problem : BaseProblem, IProblem
    {
        public Problem(IBLFactory blFactory, IDaoFactory daoFactory)
            : base(blFactory, daoFactory)
        {

        }

        public int Method1()
        {
            return 10;
        }
    }

    public abstract class BaseProblem
    {
        private IBLFactory _blFactory;
        private IDaoFactory _daoFactory;

        public BaseProblem(IBLFactory blFactory, IDaoFactory daoFactory)
        {
            _blFactory = blFactory;
            _daoFactory = daoFactory;
        }
    }
}