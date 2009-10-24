using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem.FieldProblem_Nolan2
{
    public interface IDemo
    {
        int SomeInt { get; set; }
        DateTime SomeDate { get; set; }
        DateTime? SomeNullableDate { get; set; }
        DateTime? SomeNulledDate { get; set; }
        object SomeObject { get; set; }

        bool TimeToGoHome();
    }

    
    public class When_mocking_properties_with_RhinoMocks_stub
    {
        protected IDemo _demo;
        protected DateTime _newDate = new DateTime(2008, 12, 2, 22, 17, 0);
        protected int _newInt = 7;

        protected DateTime? _newNullableDate = new DateTime(2008, 12, 2, 22,
                                                            17, 0);

        protected DateTime? _newNulledDate;
        protected Object _newObject = new object();

        protected void SetValuesOnMock()
        {
            _demo.SomeInt = _newInt;
            _demo.SomeDate = _newDate;
            _demo.SomeNullableDate = _newNullableDate;
            _demo.SomeNulledDate = _newNulledDate;
            _demo.SomeObject = _newObject;
        }

		public When_mocking_properties_with_RhinoMocks_stub()
        {
            _demo = MockRepository.GenerateStub<IDemo>();
            SetValuesOnMock();
        }

        [Fact]
        public void Should_mock_value_int_property()
        {
            Assert.Equal(_newInt, _demo.SomeInt);
        }

        [Fact]
        public void Should_mock_value_date_property()
        {
            Assert.Equal(_newDate, _demo.SomeDate);
        }

        [Fact]
        public void Should_mock_nullable_value_date_property()
        {
            Assert.Equal(_newNullableDate, _demo.SomeNullableDate);
        }

        [Fact]
        public void Should_mock_nulled_value_date_property()
        {
            Assert.Equal(_newNulledDate, _demo.SomeNulledDate);
        }

        [Fact]
        public void Should_mock_reference_property()
        {
            Assert.Equal(_newObject, _demo.SomeObject);
        }
    }
#if DOTNET35
    
    public class
        When_mocking_properties_with_RhinoMocks_stub_and_setting_expectations_afterward :
            When_mocking_properties_with_RhinoMocks_stub
    {
		public When_mocking_properties_with_RhinoMocks_stub_and_setting_expectations_afterward()
        {
            _demo = MockRepository.GenerateStub<IDemo>();
            SetValuesOnMock();

            _demo.Expect(d => d.TimeToGoHome())
                .Repeat.Any()
                .Return(false);
        }
    }

    
    public class
        When_mocking_properties_with_RhinoMocks_stub_and_setting_expectations_beforehand :
            When_mocking_properties_with_RhinoMocks_stub
    {
		public When_mocking_properties_with_RhinoMocks_stub_and_setting_expectations_beforehand()
        {
            _demo = MockRepository.GenerateStub<IDemo>();

            _demo.Expect(d => d.TimeToGoHome())
                .Repeat.Any()
                .Return(false);

            SetValuesOnMock();
        }
    }
#endif
	}