#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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
