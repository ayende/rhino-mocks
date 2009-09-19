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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Raise events for all subscribers for an event
	/// </summary>
	public class EventRaiser : IEventRaiser
	{
		string eventName;
		IMockedObject proxy;

		///<summary>
		/// Create an event raiser for the specified event on this instance.
		///</summary>
		public static IEventRaiser Create(object instance, string eventName)
		{
			IMockedObject proxy = instance as IMockedObject;
			if (proxy == null)
				throw new ArgumentException("Parameter must be a mocked object", "instance");
			return new EventRaiser(proxy, eventName);
		}

		/// <summary>
		/// Creates a new instance of <c>EventRaiser</c>
		/// </summary>
		public EventRaiser(IMockedObject proxy, string eventName)
		{
			this.eventName = eventName;
			this.proxy = proxy;
		}

		#region IEventRaiser Members

		/// <summary>
		/// Raise the event
		/// </summary>
		public void Raise(params object[] args)
		{
			Delegate subscribed = proxy.GetEventSubscribers(eventName);
			if (subscribed != null)
			{
				AssertMatchingParameters(subscribed.Method, args);
			    try
			    {
			        subscribed.DynamicInvoke(args);
			    }
			    catch (TargetInvocationException e)
			    {
			        PreserveStackTrace(e.InnerException);
			        throw e.InnerException;
			    }
			}
		}

        private static void PreserveStackTrace(Exception exception)
        {
            MethodInfo preserveStackTrace = typeof(Exception).GetMethod("InternalPreserveStackTrace",
              BindingFlags.Instance | BindingFlags.NonPublic);
            preserveStackTrace.Invoke(exception, null);
        }

		private static void AssertMatchingParameters(MethodInfo method, object[] args)
		{
			ParameterInfo[] parameterInfos = method.GetParameters();
			int paramsCount = parameterInfos.Length;
			if(args== null || args.Length != paramsCount)
			{
				int actualCount;
				if(args==null)
					actualCount = 0;
				else 
					actualCount = args.Length;
				string msg = string.Format("You have called the event raiser with the wrong number of parameters. Expected {0} but was {1}", paramsCount, actualCount);
				throw new InvalidOperationException(msg);
			}
			List<string> errors = new List<string>();
			for (int i = 0; i < parameterInfos.Length; i++)
			{
				if ((args[i] == null && parameterInfos[i].ParameterType.IsValueType) ||
					(args[i] != null && parameterInfos[i].ParameterType.IsInstanceOfType(args[i])==false))
				{
					string type = "null";
					if(args[i]!=null)
						type = args[i].GetType().FullName;
					errors.Add("Parameter #" + (i+1) + " is " + type + " but should be " +
														parameterInfos[i].ParameterType);
				}
			}
			if(errors.Count>0)
			{
				throw new InvalidOperationException(string.Join(Environment.NewLine, errors.ToArray()));
			}
		}

		/// <summary>
		/// The most common signature for events
		/// Here to allow intellisense to make better guesses about how 
		/// it should suggest parameters.
		/// </summary>
		public void Raise(object sender, EventArgs e)
		{
			Raise(new object[] { sender, e });
		}

		#endregion
	}
}
