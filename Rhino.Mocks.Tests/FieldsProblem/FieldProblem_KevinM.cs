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

#if DOTNET35
using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_KevinM
	{
		[Test]
		public void VerifyListPropertyIsSetToList_WithAssert()
		{
			var sHolder =
				MockRepository.GenerateStub<IPropertyHolder>();
			var sManager =
				MockRepository.GenerateStub<IPropertyManager>();

			var mList = new List<string> {"Foo", "Bar", "Baz"};

			// Set up return on stub
			sManager.Stub(x =>
			              x.GetProperty()).Return(mList.AsQueryable());

			new
				PropertyCoordinator(sHolder, sManager).SetListProperty();

			CollectionAssert.AreEqual(sHolder.MyList.ToArray(), mList.ToArray());
		}

		[Test]
		public void
			VerifyListPropertyIsSetToList_WithAssertWasCalled()
		{
			var sHolder =
				MockRepository.GenerateMock<IPropertyHolder>();
			var sManager =
				MockRepository.GenerateStub<IPropertyManager>();

			// Stub list
			var mList = new List<string> {"Foo", "Bar", "Baz"};

			// Set up return on stub
			sManager.Stub(x =>
						  x.GetProperty()).Return(mList.AsQueryable());

			new PropertyCoordinator(sHolder,
			                        sManager).SetListProperty();

			sHolder.AssertWasCalled(h => h.MyList = null, 
				options => options.Constraints(List.Equal(mList)));
		}
	}

	public interface IPropertyHolder
	{
		IList<string> MyList { get; set; }
	}


	public interface IPropertyManager
	{
		IQueryable<string> GetProperty();
	}

	public class PropertyCoordinator
	{
		private readonly IPropertyHolder _holder;
		private readonly IPropertyManager _manager;

		public PropertyCoordinator(IPropertyHolder holder,
		                           IPropertyManager manager)
		{
			_holder = holder;
			_manager = manager;
		}

		public void SetListProperty()
		{
			_holder.MyList = _manager.GetProperty().ToList();
		}
	}
}
#endif