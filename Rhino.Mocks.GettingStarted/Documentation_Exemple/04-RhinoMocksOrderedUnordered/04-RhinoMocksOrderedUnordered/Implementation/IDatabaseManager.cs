namespace RhinoMocksIntroduction
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IDatabaseManager
    {
        object BeginTransaction();
        void Dispose();
    }
}
