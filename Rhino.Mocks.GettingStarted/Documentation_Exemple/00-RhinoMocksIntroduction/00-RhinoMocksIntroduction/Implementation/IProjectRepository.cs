namespace RhinoMocksIntroduction
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public interface IProjectRepository
    {
        string GetProjectByName(string newProjectName);
        void SaveProject(IProject prj);
    }
}
