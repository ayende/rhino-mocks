using System;
using MbUnit.Framework;

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

    [TestFixture]
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

        [SetUp]
        public virtual void SetUp()
        {
            _demo = MockRepository.GenerateStub<IDemo>();
            SetValuesOnMock();
        }

        [Test]
        public void Should_mock_value_int_property()
        {
            Assert.AreEqual(_newInt, _demo.SomeInt);
        }

        [Test]
        public void Should_mock_value_date_property()
        {
            Assert.AreEqual(_newDate, _demo.SomeDate);
        }

        [Test]
        public void Should_mock_nullable_value_date_property()
        {
            Assert.AreEqual(_newNullableDate, _demo.SomeNullableDate);
        }

        [Test]
        public void Should_mock_nulled_value_date_property()
        {
            Assert.AreEqual(_newNulledDate, _demo.SomeNulledDate);
        }

        [Test]
        public void Should_mock_reference_property()
        {
            Assert.AreEqual(_newObject, _demo.SomeObject);
        }
    }
#if DOTNET35
    [TestFixture]
    public class
        When_mocking_properties_with_RhinoMocks_stub_and_setting_expectations_afterward :
            When_mocking_properties_with_RhinoMocks_stub
    {
        [SetUp]
        public override void SetUp()
        {
            _demo = MockRepository.GenerateStub<IDemo>();
            SetValuesOnMock();

            _demo.Expect(d => d.TimeToGoHome())
                .Repeat.Any()
                .Return(false);
        }
    }

    [TestFixture]
    public class
        When_mocking_properties_with_RhinoMocks_stub_and_setting_expectations_beforehand :
            When_mocking_properties_with_RhinoMocks_stub
    {
        [SetUp]
        public override void SetUp()
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