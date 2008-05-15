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
using System.Data;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	/// <summary>
	/// Summary description for Bug_45.
	/// </summary>
	public class ClassThatImplementsGetHashCodeAndEquals
	{
        MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }
	    
		[Test]
		public void InitClass()
		{
				EmployeeInfo info = (EmployeeInfo)mocks.StrictMock(typeof(EmployeeInfo), "ID001");

				mocks.ReplayAll();

				Assert.IsNotNull(info);
		}

		[Serializable]
		public class EmployeeInfo 
		{
		
			public EmployeeInfo(string employeeId) 
			{
				if (employeeId == null || employeeId.Length == 0) 
				{
					throw new ArgumentNullException("employeeId");
				}

			}

		
			#region Object Members
   
			/// <summary>
			/// Returns a string representation of this instance.
			/// </summary>
			public override string ToString() 
			{
				return null;
			}
   
			/// <summary>
			/// Gets the hash code for this instance.
			/// </summary>
			public override int GetHashCode() 
			{
				return this.ToString().GetHashCode();
			}
   
			/// <summary>
			/// Determines whether the specified instance is equal to this instance.
			/// </summary>
			public override bool Equals(object obj) 
			{
				EmployeeInfo objToCompare = obj as EmployeeInfo;
      
				if (objToCompare == null) 
				{
					return false;
				}
      
				return (this.GetHashCode() == objToCompare.GetHashCode());
			}
   
			#endregion
		}
	}
}
