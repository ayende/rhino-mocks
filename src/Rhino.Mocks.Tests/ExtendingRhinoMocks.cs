using System;
using System.Collections;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using NUnit.Framework;

namespace Rhino.Mocks.Tests
{

	[TestFixture]
	public class ExtendingRhinoMocksFixture
	{
		[Test]
		public void CanUseCustomMocks()
		{
			CarRepository carRepository = new CarRepository();
			carRepository.Add(new Car("Volvo"));
			MyMockRepository mocks = new MyMockRepository();
			IView view = (IView)mocks.QueryableMock(typeof(IView));
			Presenter presenter = new Presenter(carRepository, view);
			presenter.Render();
			Car car = (Car)mocks.Query(view);
			Assert.AreEqual("Volvo", car.Make);
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
		string name;

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

		public class QueryMockState : IMockState
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

			public IMethodOptions LastMethodOptions
			{
				get { throw new NotImplementedException(); }
			}

			public void SetExceptionToThrowOnVerify(Exception ex)
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
