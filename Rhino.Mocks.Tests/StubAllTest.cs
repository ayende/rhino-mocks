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
	
	public class StubAllTest
	{
		[Fact]
		public void StaticAccessorForStubAll()
		{
			ICat cat = MockRepository.GenerateStub<ICat>();
			cat.Eyes = 2;
			Assert.Equal(2, cat.Eyes );
		}

		[Fact]
		public void StubAllHasPropertyBehaviorForAllProperties()
		{
			MockRepository mocks = new MockRepository();
			ICat cat = mocks.Stub<ICat>();
			cat.Legs = 4;
			Assert.Equal(4, cat.Legs);

			cat.Name = "Esther";
			Assert.Equal("Esther", cat.Name);

			Assert.Null(cat.Species);
			cat.Species = "Ordinary housecat";
			Assert.Equal("Ordinary housecat", cat.Species);

			cat.IsDeclawed = true;
			Assert.True(cat.IsDeclawed);
		}

		[Fact]
		public void StubAllHasPropertyBehaviorForAllPropertiesWhenStubbingClasses()
		{
			MockRepository mocks = new MockRepository();
			Housecat housecat = mocks.Stub<Housecat>();

			housecat.FurLength = 7;
			Assert.Equal(7, housecat.FurLength);

			housecat.Color = "Black";
			Assert.Equal("Black", housecat.Color);
		}

		[Fact]
		public void StubAllCanRegisterToEventsAndRaiseThem()
		{
			MockRepository mocks = new MockRepository();
			ICat cat = mocks.Stub<ICat>();
			cat.Hungry += null; //Note, no expectation!
			IEventRaiser eventRaiser = LastCall.GetEventRaiser();

			bool raised = false;
			cat.Hungry += delegate
			{
				raised = true;
			};

			eventRaiser.Raise(cat, EventArgs.Empty);
			Assert.True(raised);
		}

		[Fact]
		public void CallingMethodOnStubAllDoesNotCreateExpectations()
		{
			MockRepository mocks = new MockRepository();
			ICat cat = mocks.Stub<ICat>();
			using (mocks.Record())
			{
				cat.Legs = 4;
				cat.Name = "Esther";
				cat.Species = "Ordinary housecat";
				cat.IsDeclawed = true;
				cat.GetMood();
			}
			mocks.VerifyAll();
		}

		[Fact]
		public void DemoStubAllLegsProperty()
		{
			ICat catStub = MockRepository.GenerateStub<ICat>();

			catStub.Legs = 0;
			Assert.Equal(0, catStub.Legs);

			SomeClass instance = new SomeClass(catStub);
			instance.SetLegs(10);
			Assert.Equal(10, catStub.Legs);
		}

		[Fact]
		public void StubAllCanCreateExpectationOnMethod()
		{
			MockRepository mocks = new MockRepository();
			ICat cat = mocks.Stub<ICat>();
			using (mocks.Record())
			{
				cat.Legs = 4;
				cat.Name = "Esther";
				cat.Species = "Ordinary housecat";
				cat.IsDeclawed = true;
				cat.GetMood();
				LastCall.Return("Happy");
			}
			Assert.Equal("Happy", cat.GetMood());
			mocks.VerifyAll();
		}

		[Fact]
		public void StubAllCanHandlePropertiesGettingRegisteredMultipleTimes()
		{
			MockRepository mocks = new MockRepository();
			SpecificFish fish = mocks.Stub<SpecificFish>();

			fish.IsFreshWater = true;
			Assert.True(fish.IsFreshWater);
		}

        [Fact]
        public void StubCanHandlePolymorphicArgConstraints()
        {
            IAquarium aquarium = MockRepository.GenerateStub<IAquarium>();
            aquarium.Stub(x => x.DetermineAge(Arg<MartianFish>.Matches(arg => arg.Planet == "mars"))).Return(100);
            aquarium.Stub(x => x.DetermineAge(Arg<SpecificFish>.Is.TypeOf)).Return(5);
            
            Assert.False(typeof(MartianFish).IsAssignableFrom(typeof(SpecificFish)));
            Assert.Equal(5, aquarium.DetermineAge(new SpecificFish()));
        }

	}

	public interface ICat : IAnimal
	{
		bool IsDeclawed { get; set; }
	}

	public class Feline
	{
		private int _furLength;

		public virtual int FurLength
		{
			get { return _furLength; }
			set { _furLength = value; }
		}
	}

	public class Housecat : Feline
	{
		private String _color;

		public virtual String Color
		{
			get { return _color; }
			set { _color = value; }
		}
	}

   

    public interface IAquarium
    {
        int DetermineAge(IFish fish);
    }

	public interface IFish
	{
		bool IsFreshWater { get; set; }
	}

	public abstract class Fish : IFish
	{
		public abstract bool IsFreshWater { get; set; }
	}

    public class MartianFish : IFish
    {
        public bool IsFreshWater { get; set; }
        public string Planet { get; set; }
    }

	public class SpecificFish : Fish
	{
		private bool _isFreshWater;

		public override bool IsFreshWater
		{
			get { return _isFreshWater; }
			set { _isFreshWater = value; }
		}		
	}

}
