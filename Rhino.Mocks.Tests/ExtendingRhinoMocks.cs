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
using System.Collections;
using System.Reflection;
using Castle.DynamicProxy;
using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
	
	public class ExtendingRhinoMocksFixture
	{
		[Fact]
		public void CanUseCustomMocks()
		{
			CarRepository carRepository = new CarRepository();
			carRepository.Add(new Car("Volvo"));
			MyMockRepository mocks = new MyMockRepository();
			IView view = (IView)mocks.QueryableMock(typeof(IView));
			Presenter presenter = new Presenter(carRepository, view);
			presenter.Render();
			Car car = (Car)mocks.Query(view);
			Assert.Equal("Volvo", car.Make);
		}
	}

	public interface IView
	{
		void SetCar(Car c);
	}

	public class CarRepository : ArrayList
	{
	}

	public class Presenter
	{
		private readonly CarRepository repository;
		private readonly IView view;

		public Presenter(CarRepository repository, IView view)
		{
			this.repository = repository;
			this.view = view;
		}

		public void Render()
		{
			view.SetCar((Car)repository[0]);
		}
	}

	public class Car
	{
		private string name;

		public string Make
		{
			get { return name; }
		}

		public Car(string name)
		{
			this.name = name;
		}
	}

	public class MyMockRepository : MockRepository
	{
		public object QueryableMock(Type type)
		{
            return CreateMockObject(type, new CreateMockState(CreateQueryMockState), new Type[0]);
		}

		private IMockState CreateQueryMockState(IMockedObject mockedObject)
		{
			return new QueryMockState(mockedObject);
		}

		internal class QueryMockState : IMockState
		{
			private readonly IMockedObject mockedObject;
			public object LastCallFirstParam;

			public QueryMockState(IMockedObject mockedObject)
			{
				this.mockedObject = mockedObject;
			}


			public object MethodCall(IInvocation invocation, MethodInfo method, params object[] args)
			{
				if (args.Length != 1)
					throw new NotSupportedException("Expected only a single argument");
				LastCallFirstParam = args[0];
				return null;
			}

			public void Verify()
			{
			}

			public IMockState VerifyState
			{
				get { return this; }
			}

			public IMockState Replay()
			{
				return this;
			}

			public IMockState BackToRecord()
			{
				return this;
			}

			public IMethodOptions<T> GetLastMethodOptions<T>()
			{
				throw new NotImplementedException();
			}

			public IMethodOptions<object> LastMethodOptions
			{
				get { throw new NotImplementedException(); }
			}

			public void SetExceptionToThrowOnVerify(Exception ex)
			{
			}

		    public void NotifyCallOnPropertyBehavior()
		    {
		        
		    }
		}

		public object Query(object mock)
		{
			QueryMockState query = (QueryMockState)base.proxies[mock];
			return query.LastCallFirstParam;
		}
	}
}