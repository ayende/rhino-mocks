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
using System.Runtime.InteropServices;

namespace Rhino.Mocks
{
	///<summary>
	/// Adds optional new usage:
	///   using(mockRepository.Record()) {
	///      Expect.Call(mock.Method()).Return(retVal);
	///   }
	///   using(mockRepository.Playback()) {
	///      // Execute code
	///   }
	/// N.B. mockRepository.ReplayAll() and mockRepository.VerifyAll()
	///      calls are taken care of by Record/Playback
	///</summary>
	public partial class MockRepository
	{
		///<summary>
		///</summary>
		///<returns></returns>
		public IDisposable Record()
		{
			return new RecordModeChanger(this);
		}

		///<summary>
		///</summary>
		///<returns></returns>
		public IDisposable Playback()
		{
			return new PlaybackModeChanger(this);
		}
	}

	internal class PlaybackModeChanger : IDisposable
	{
		private readonly MockRepository m_repository;

		public PlaybackModeChanger(MockRepository repository)
		{
			m_repository = repository;
		}

		public void Dispose()
		{
			if (DisposableActionsHelper.ExceptionWasThrownAndDisposableActionShouldNotBeCalled())
				return;
			m_repository.VerifyAll();
		}
	}

	internal static class DisposableActionsHelper
	{
		internal static bool ExceptionWasThrownAndDisposableActionShouldNotBeCalled()
		{
			//If we're running under Mono, then we don't want to call Marshall.GetExceptionCode as it
			// currently is not implemented
			Type t = Type.GetType("Mono.Runtime");
			if (t == null)
			{
				// Probably running the .NET Framework
				if (Marshal.GetExceptionCode() != 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	internal class RecordModeChanger : IDisposable
	{
		private readonly MockRepository m_repository;

		public RecordModeChanger(MockRepository repository)
		{
			m_repository = repository;
		}

		public void Dispose()
		{
			if (DisposableActionsHelper.ExceptionWasThrownAndDisposableActionShouldNotBeCalled())
				return;
			m_repository.ReplayAll();
		}
	}
}