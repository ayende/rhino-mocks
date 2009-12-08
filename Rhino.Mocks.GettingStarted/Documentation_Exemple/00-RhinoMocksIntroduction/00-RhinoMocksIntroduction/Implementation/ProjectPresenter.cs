namespace RhinoMocksIntroduction
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public class ProjectPresenter : IProjectPresenter
    {
        public IProject m_Prj;
        public IProjectView m_View;

        public ProjectPresenter(IProject prj, IProjectView view)
        {
            this.m_Prj = prj;
            this.m_View = view;
        }

        #region IProjectPresenter Members

        bool IProjectPresenter.SaveProjectAs()
        {
            string projectName = m_View.Title;
            m_View.Ask(null, null);
            return false;
        }

        #endregion
    }
}
