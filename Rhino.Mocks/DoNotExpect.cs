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

namespace Rhino.Mocks
{
    /// <summary>
    /// Allows expectations to be set on methods that should never be called.
    /// For methods with void return value, you need to use LastCall or
    /// DoNotExpect.Call() with a delegate.
    /// </summary>
    public static class DoNotExpect
    {
        /// <summary>
        /// Sets LastCall.Repeat.Never() on /any/ proxy on /any/ repository on the current thread.
        /// This method if not safe for multi threading scenarios.
        /// </summary>
        public static void Call(object ignored)
        {
            LastCall.Repeat.Never();
        }

        /// <summary>
        /// Accepts a delegate that will execute inside the method which
        /// LastCall.Repeat.Never() will be applied to.
        /// It is expected to be used with anonymous delegates / lambda expressions and only one
        /// method should be called.
        /// </summary>
        /// <example>
        /// IService mockSrv = mocks.CreateMock(typeof(IService)) as IService;
        /// DoNotExpect.Call(delegate{ mockSrv.Stop(); });
        /// ...
        /// </example>
        public static void Call(Expect.Action actionToExecute)
        {
            if (actionToExecute == null)
                throw new ArgumentNullException("actionToExecute", "The action to execute cannot be null");
            actionToExecute();
            LastCall.Repeat.Never();
        }
    }
}
