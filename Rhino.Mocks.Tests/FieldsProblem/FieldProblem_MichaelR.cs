using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class PropertyWithTypeParameterTest
	{
		[Test]
		public void Execute()
		{
			MockRepository mocks = new MockRepository();
			mocks.CreateMock<ClosedGenericType>();
		}

		public class ClosedGenericType : OpenGenericType<TypeParameterType>
		{
			public override TypeParameterType GenericProperty
			{
				get { return null; }
			}
		}

		public abstract class OpenGenericType<T>
		{
			public abstract T GenericProperty { get; }
		}

		public class TypeParameterType
		{
		}
	}
}