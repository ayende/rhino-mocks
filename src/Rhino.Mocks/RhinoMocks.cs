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
        public const string StrongName = "DynamicAssemblyProxyGen, PublicKey=0024000004800000940000000602000000240000525341310004000001000100fb4ff5a7c8bba6feb6a6b75b260cd57c1b8b85b63a45dedcb7081331740c870852af30abd2a74700cce1d7a01aeed019339db202e010ac808396b2922362877c6afc8993281092434a223b9920cac8ba409d138a97b73cd1baad813af450b886e3d7f5a09ee450d415145eb0524778a6bd1ae733fd2b6ceebfd151620534bcb7";
    }
}
