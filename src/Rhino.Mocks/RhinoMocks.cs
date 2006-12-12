using System;
using System.Text;

namespace Rhino.Mocks
{
    /* class: RhinoMocks
     * Used for [assembly: InternalsVisibleTo(RhinoMocks.StrongName)]
     */
    /// <summary>
    /// Used for [assembly: InternalsVisibleTo(RhinoMocks.StrongName)]
    /// </summary>
    public 
#if dotNet2
    static 
#else
	sealed
#endif
	class RhinoMocks
    {
        /// <summary>
        /// Strong name for the Dynamic Proxy assemblies. Used for InternalsVisibleTo specification.
        /// </summary>
		public const string StrongName = "DynamicProxyGenAssembly2";
    }
}
