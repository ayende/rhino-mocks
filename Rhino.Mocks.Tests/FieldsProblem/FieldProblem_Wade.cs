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


using System.Runtime.InteropServices;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Wade
	{
		[Test]
		public void CanMockClassWithCoClass()
		{
			MockRepository mocks = new MockRepository();
			clsDBRecordSet contacts = mocks.CreateMock<clsDBRecordSet>();
		}
	}

	[CoClass(typeof (clsDBRecordSetClass))]
	[Guid("1D606603-02F0-4443-8A54-9AE4EDB5EEFA")]
	public interface clsDBRecordSet : _clsDBRecordSet
	{
	}

	[ComImport]
	[Guid("1F606603-02F0-4443-8A54-9AE4EDB5EEFA")]
	public class clsDBRecordSetClass
	{
	}

	[Guid("1D606603-02F0-4443-8A54-9AE4EDB5EEFA")]
	[TypeLibType(4304)]
	public interface _clsDBRecordSet
	{
		[DispId(1745027080)]
		bool BOF { get; }

		[DispId(1745027077)]
		object BookMark { get; set; }

		[DispId(1745027079)]
		clsDBRecordSet Clone { get; }

		[DispId(1745027081)]
		bool EOF { get; }

		[DispId(1745027073)]
		string Filter { set; }

		[DispId(1745027075)]
		string GetString { get; }

		[DispId(1745027072)]
		int Maxrecords { get; set; }

		[DispId(1745027078)]
		int RecordCount { get; }

		[DispId(1745027076)]
		string SQL { get; set; }

		[DispId(1610809360)]
		void AddNew();

		[DispId(1610809370)]
		void CloseRS();


		void Edit();

		[DispId(1610809372)]
		int FindAndReplace(string v_sFieldName, string v_sReplace, string v_sFind);

		[DispId(1610809369)]
		object GetRows(int numrows);


		[DispId(1610809361)]
		void MoveFirst();

		[DispId(1610809362)]
		void MoveLast();

		[DispId(1610809364)]
		void MoveNext();

		[DispId(1610809363)]
		void MovePrevious();


		[DispId(1610809371)]
		void ReOpenRS();

		[DispId(1610809367)]
		void Requery();


		[DispId(1610809373)]
		bool SaveAsXML(string v_sFile);

		[DispId(1610809366)]
		void Update();
	}
}