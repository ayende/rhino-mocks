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
using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Tests.Utilities
{
	[TestFixture]
	public class MethodCallTests
	{
		[Test]
		public void MethodCallToString()
		{
			string actual = MethodCallUtil.StringPresentation(null, GetMethodInfo("StartsWith", ""), new object[] {"abcd"});
			Assert.AreEqual("String.StartsWith(\"abcd\");", actual);
		}

		[Test]
		public void MethodCallToStringWithSeveralArguments()
		{
			string actual = MethodCallUtil.StringPresentation(null,GetMethodInfo("IndexOf", "abcd", 4), new object[] {"abcd", 4});
			Assert.AreEqual("String.IndexOf(\"abcd\", 4);", actual);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: method")]
		public void MethodCallCtorWontAcceptNullMethod()
		{
			MethodCallUtil.StringPresentation(null,null, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: args")]
		public void MethodCallCtorWontAcceptNullArgs()
		{
            MethodInfo method = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
			MethodCallUtil.StringPresentation(null,method, null);
		}

		[Test]
		public void MethodCallWithArgumentsMissing()
		{
            MethodInfo method = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
            Assert.AreEqual("String.StartsWith(missing parameter);", MethodCallUtil.StringPresentation(null,method, new object[0]));

		}

		#region Implementation

		private static Type[] TypesFromArgs(object[] args)
		{
			Type[] types = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				types[i] = args[i].GetType();
			}
			return types;
		}

		public static MethodInfo GetMethodInfo(string name, params object[] args)
		{
			Type[] types = TypesFromArgs(args);
			MethodInfo method = typeof (string).GetMethod(name, types);
			return method;
		}

		#endregion
	}
}