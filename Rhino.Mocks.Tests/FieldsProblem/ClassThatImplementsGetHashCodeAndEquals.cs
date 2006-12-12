using System;
using System.Data;
using System.Text;
using NUnit.Framework;

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
				EmployeeInfo info = (EmployeeInfo)mocks.CreateMock(typeof(EmployeeInfo), "ID001");

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
