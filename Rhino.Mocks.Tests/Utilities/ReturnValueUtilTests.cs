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

using System.Collections;
using Xunit;
using Rhino.Mocks.Utilities;
using System.Collections.Generic;

namespace Rhino.Mocks.Tests.Utilities
{
	
	public class ReturnValueUtilTests
	{
		[Fact]
		public void DefaultReturnValue()
		{
			Assert.Null(ReturnValueUtil.DefaultValue(typeof(void), null));
			Assert.Null(ReturnValueUtil.DefaultValue(typeof (string),null));
			Assert.Equal(0, ReturnValueUtil.DefaultValue(typeof (int),null));
			Assert.Equal((short) 0, ReturnValueUtil.DefaultValue(typeof (short),null));
			Assert.Equal((char) 0, ReturnValueUtil.DefaultValue(typeof (char),null));
			Assert.Equal(0L, ReturnValueUtil.DefaultValue(typeof (long),null));
			Assert.Equal(0f, ReturnValueUtil.DefaultValue(typeof (float),null));
			Assert.Equal(0d, ReturnValueUtil.DefaultValue(typeof (double),null));
			Assert.Equal(TestEnum.DefaultValue, ReturnValueUtil.DefaultValue(typeof (TestEnum),null));
		}

		[Fact]
		public void DefaultReturnValue_WhenTheReturnTypeIsACollectionInterface_ReturnAnEmptyCollection()
		{
			Assert.NotNull(ReturnValueUtil.DefaultValue(typeof (IEnumerable), null) as IEnumerable);

			var defaultValueForCollections = ReturnValueUtil.DefaultValue(typeof (ICollection), null) as ICollection;
			Assert.NotNull(defaultValueForCollections);
			Assert.Equal(0, defaultValueForCollections.Count);

			var defaultValueForLists = ReturnValueUtil.DefaultValue(typeof (IList), null) as IList;
			Assert.NotNull(defaultValueForLists);
			Assert.Equal(0, defaultValueForLists.Count);

			var defaultValueForDictionaries = ReturnValueUtil.DefaultValue(typeof (IDictionary), null) as IDictionary;
			Assert.NotNull(defaultValueForDictionaries);
			Assert.Equal(0, defaultValueForDictionaries.Keys.Count);
			Assert.Equal(0, defaultValueForDictionaries.Values.Count);
		}

		[Fact]
		public void DefaultReturnValue_WhenTheReturnTypeIsAGenericCollectionInterface_ReturnAnEmptyCollection()
		{
			var defValForGenericEnumerable = ReturnValueUtil.DefaultValue(typeof (IEnumerable<IFoo>), null);
			Assert.NotNull(defValForGenericEnumerable);

			var defValForAGenericCollection = ReturnValueUtil.DefaultValue(typeof (ICollection<IFoo>), null) as ICollection<IFoo>;
			Assert.NotNull(defValForAGenericCollection);
			Assert.Equal(0, defValForAGenericCollection.Count);

			var defValForAGenericList = ReturnValueUtil.DefaultValue(typeof (IList<IFoo>), null) as IList<IFoo>;
			Assert.NotNull(defValForAGenericList);
			Assert.Equal(0, defValForAGenericList.Count);

			var defValForAGenericDictionary =
				ReturnValueUtil.DefaultValue(typeof (IDictionary<IFoo, TestEnum>), null) as IDictionary<IFoo, TestEnum>;
			Assert.NotNull(defValForAGenericDictionary);
			Assert.Equal(0, defValForAGenericDictionary.Keys.Count);
			Assert.Equal(0, defValForAGenericDictionary.Values.Count);
		}

		[Fact]
		public void DefaultReturnValue_WhenTheReturnTypeImplementsAdditionalsInterfacesToIEnumerable_ReturnNull()
		{
			object defVal = ReturnValueUtil.DefaultValue(typeof (IFoo), null);

			Assert.Null(defVal);
		}

		private enum TestEnum
		{
			DefaultValue,
			NonDefaultValue
		}

		public interface IFoo : IEnumerable
		{
			string Bar { get; set; }
		}
	}
}