namespace Rhino.Mocks.GettingStarted
{
    public static class Helpers
    {
        public static bool Implements<T>(this object @this)
        {
            return typeof (T).IsAssignableFrom(@this.GetType());
        }
    }
}