using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Kevin
	{
		//This is related to an initialization mechanism in use for the CodeGenerationServices on Castle.Contrib
		//I am not sure if this is related to invalid construction of the object. Resharper complains that invoking virtual methods from ctors is a bad idea. 
		//It surely seems that it is preventing me from partial mocking a class that has some complex construction. 
		
		[Test]
		public void Virtual_protected_method_called_from_ctor_is_not_called_during_partial_mock_construction()
		{
			string mockedResult = "mocked result";

			MockRepository mockRepository = new MockRepository();
			ConcreteProtectedMethodCalledFromCtor concreteProtectedMethodCalledFromCtor = mockRepository.PartialMock<ConcreteProtectedMethodCalledFromCtor>();

			Expect.Call(concreteProtectedMethodCalledFromCtor.SimplyHereSoThereIsSomethingToMock()).Return(mockedResult);

			mockRepository.ReplayAll();

			string result = concreteProtectedMethodCalledFromCtor.SimplyHereSoThereIsSomethingToMock();

			mockRepository.VerifyAll();

			Assert.AreEqual(mockedResult, result);
			Assert.AreEqual(true, concreteProtectedMethodCalledFromCtor.WasAbstractMethodCalledFromCtor);
		}

		[Test]
		public void Virtual_public_method_called_from_ctor_is_not_called_during_partial_mock_construction()
		{
			string mockedResult = "mocked result";

			MockRepository mockRepository = new MockRepository();
			ConcretePublicMethodCalledFromCtor concretePublicMethodCalledFromCtor = mockRepository.PartialMock<ConcretePublicMethodCalledFromCtor>();

			Expect.Call(concretePublicMethodCalledFromCtor.SimplyHereSoThereIsSomethingToMock()).Return(mockedResult);

			mockRepository.ReplayAll();

			string result = concretePublicMethodCalledFromCtor.SimplyHereSoThereIsSomethingToMock();

			mockRepository.VerifyAll();

			Assert.AreEqual(mockedResult, result);
			Assert.AreEqual(true, concretePublicMethodCalledFromCtor.WasAbstractMethodCalledFromCtor);
		}
	}

	//Abstract Protected

	public class ConcreteProtectedMethodCalledFromCtor : ProtectedExampleCalledFromCtor
	{
		protected override void AbstractCalledFromCtor()
		{
			_wasAbstractMethodCalledFromCtor = true;
		}
	}

	public abstract class ProtectedExampleCalledFromCtor
	{
		protected bool _wasAbstractMethodCalledFromCtor = false;

		public bool WasAbstractMethodCalledFromCtor
		{
			get { return _wasAbstractMethodCalledFromCtor; }
		}

		public ProtectedExampleCalledFromCtor()
		{
			AbstractCalledFromCtor();
		}

		protected abstract void AbstractCalledFromCtor();

		public virtual string SimplyHereSoThereIsSomethingToMock()
		{
			return "If this value is returned the method was not mocked";
		}
	}

	//Abstract Public
	public class ConcretePublicMethodCalledFromCtor : PublicExampleCalledFromCtor
	{
	    public override void AbstractCalledFromCtor()
	    {
	        _wasAbstractMethodCalledFromCtor = true;
	    }
	}

	public abstract class PublicExampleCalledFromCtor
	{
	    protected bool _wasAbstractMethodCalledFromCtor = false;

	    public bool WasAbstractMethodCalledFromCtor
	    {
	        get { return _wasAbstractMethodCalledFromCtor; }
	    }

	    public PublicExampleCalledFromCtor()
	    {
	        AbstractCalledFromCtor();
	    }

	    public abstract void AbstractCalledFromCtor();

	    public virtual string SimplyHereSoThereIsSomethingToMock()
	    {
	        return "If this value is returned the method was not mocked";
	    }
	}

}
