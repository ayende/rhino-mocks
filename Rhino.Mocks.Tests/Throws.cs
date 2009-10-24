namespace Rhino.Mocks.Tests
{
	using System;
	using Xunit;

	public class Throws
	{
		public static void Exception<TException>(Delegates.Action action)
			where TException  : Exception
		{
			try
			{
				action();
				Assert.False(true, "Should have thrown exception");
			}
			catch(TException)
			{
			}
		}

		public static void Exception<TException>(string message, Delegates.Action action)
			where TException : Exception
		{
			try
			{
				action();
				Assert.False(true, "Should have thrown exception");
			}
			catch (TException e)
			{
				Assert.Equal(message, e.Message);
			}
		}
	}
}