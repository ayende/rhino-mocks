using System.Runtime.CompilerServices;
using Rhino.Mocks;

#if dotNet2
[assembly: InternalsVisibleTo(RhinoMocks.StrongName)]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif