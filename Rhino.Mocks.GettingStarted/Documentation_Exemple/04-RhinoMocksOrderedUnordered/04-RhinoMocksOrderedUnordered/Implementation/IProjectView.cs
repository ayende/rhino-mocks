namespace RhinoMocksIntroduction
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IProjectView
    {
        string Title { get; set; }
        bool HasChanges { get; set; }
        string Ask(string arg1, string arg2);
    }
}
