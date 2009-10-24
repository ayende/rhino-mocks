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

using System;
using Xunit;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
	
	public class StubTest
	{
		[Fact]
		public void StaticAccessorForStub()
		{
			IAnimal animal = MockRepository.GenerateStub<IAnimal>();
			animal.Eyes = 2;
			Assert.Equal(2, animal.Eyes );
		}

		[Fact]
		public void StubHasPropertyBehaviorForAllProperties()
		{
			MockRepository mocks = new MockRepository();
			IAnimal animal = mocks.Stub<IAnimal>();
			animal.Legs = 4;
			Assert.Equal(4, animal.Legs);

			animal.Name = "Rose";
			Assert.Equal("Rose", animal.Name);

			Assert.Null(animal.Species);
			animal.Species = "Caucasusian Shepherd";
			Assert.Equal("Caucasusian Shepherd", animal.Species);
		}

		[Fact]
		public void CanRegisterToEventsAndRaiseThem()
		{
			MockRepository mocks = new MockRepository();
			IAnimal animal = mocks.Stub<IAnimal>();
			animal.Hungry += null; //Note, no expectation!
			IEventRaiser eventRaiser = LastCall.GetEventRaiser();

			bool raised = false;
			animal.Hungry += delegate
			{
				raised = true;
			};

			eventRaiser.Raise(animal, EventArgs.Empty);
			Assert.True(raised);
		}

		[Fact]
		public void CallingMethodOnStubsDoesNotCreateExpectations()
		{
			MockRepository mocks = new MockRepository();
			IAnimal animal = mocks.Stub<IAnimal>();
			using (mocks.Record())
			{
				animal.Legs = 4;
				animal.Name = "Rose";
				animal.Species = "Caucasusian Shepherd";
				animal.GetMood();
			}
			mocks.VerifyAll();
		}

		[Fact]
		public void DemoLegsProperty()
		{
			IAnimal animalStub = MockRepository.GenerateStub<IAnimal>();

			animalStub.Legs = 0;
			Assert.Equal(0, animalStub.Legs);

			SomeClass instance = new SomeClass(animalStub);
			instance.SetLegs(10);
			Assert.Equal(10, animalStub.Legs);
		}

		[Fact]
		public void CanCreateExpectationOnMethod()
		{
			MockRepository mocks = new MockRepository();
			IAnimal animal = mocks.Stub<IAnimal>();
			using (mocks.Record())
			{
				animal.Legs = 4;
				animal.Name = "Rose";
				animal.Species = "Caucasusian Shepherd";
				animal.GetMood();
				LastCall.Return("Happy");
			}
			Assert.Equal("Happy", animal.GetMood());
			mocks.VerifyAll();
		}
	}

	public interface IAnimal
	{
		int Legs { get; set; }
		int Eyes { get; set; }
		string Name { get; set; }
		string Species { get; set; }

		event EventHandler Hungry;
		string GetMood();
	}

	public class SomeClass
	{
		private IAnimal animal;

		public SomeClass(IAnimal animal)
		{
			this.animal = animal;
		}

		public void SetLegs(int count)
		{
			animal.Legs = count;
		}
	}
}