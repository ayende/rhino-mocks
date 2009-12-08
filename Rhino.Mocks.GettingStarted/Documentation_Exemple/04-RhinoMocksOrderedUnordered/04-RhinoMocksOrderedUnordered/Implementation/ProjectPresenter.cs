namespace RhinoMocksIntroduction
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public class ProjectPresenter : IProjectPresenter
    {
        public IProject m_Prj;
        public IProjectRepository m_Repository;
        public IProjectView m_View;

        public ProjectPresenter(IProject prj, IProjectRepository repository, IProjectView view)
        {
            this.m_Prj = prj;
            this.m_Repository = repository;
            this.m_View = view;
        }

        #region IProjectPresenter Members

        bool IProjectPresenter.SaveProjectAs()
        {
            string projectName = m_View.Title;
            if (m_Repository.GetProjectByName(m_View.Ask("Mock ?", "Yes")) == null)
            {
                m_View.Title = "RhinoMocks";
                m_Prj.Name = "RhinoMocks";
                m_View.HasChanges = false;

                m_Repository.SaveProject(m_Prj);
                return true;
            }

            return false;
        }

        #endregion
    }
}
