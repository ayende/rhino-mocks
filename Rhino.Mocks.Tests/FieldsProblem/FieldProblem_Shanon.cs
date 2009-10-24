using Xunit;
using RhinoMocksCPPInterfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_Shanon
	{
		[Fact(Skip =  @"Updating the Castle and NH assmeblies causes this to fail.
		
		Message:Method 'StartLiveOnSlot' in type 'IHaveMethodWithModOptsProxye59cf24cdfbc4797af58984e3c4fdf3f' from assembly 'DynamicProxyGenAssembly2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=a621a9e7e5c32e69' does not have an implementation.
Source:mscorlib
TypeName:IHaveMethodWithModOptsProxye59cf24cdfbc4797af58984e3c4fdf3f
TargetSite:System.Type _TermCreateClass(Int32, System.Reflection.Module)
HelpLink:null
StackTrace:

   at System.Reflection.Emit.TypeBuilder._TermCreateClass(Int32 handle, Module module)
   at System.Reflection.Emit.TypeBuilder.CreateTypeNoLock()
   at System.Reflection.Emit.TypeBuilder.CreateType()
   at Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter.BuildType()
   at Castle.DynamicProxy.Generators.InterfaceProxyWithTargetGenerator.GenerateCode(Type proxyTargetType, Type[] interfaces, ProxyGenerationOptions options)
   at Castle.DynamicProxy.DefaultProxyBuilder.CreateInterfaceProxyTypeWithoutTarget(Type theInterface, Type[] interfaces, ProxyGenerationOptions options)
   at Castle.DynamicProxy.ProxyGenerator.CreateInterfaceProxyTypeWithoutTarget(Type theInterface, Type[] interfaces, ProxyGenerationOptions options)
   at Castle.DynamicProxy.ProxyGenerator.CreateInterfaceProxyWithoutTarget(Type theInterface, Type[] interfaces, ProxyGenerationOptions options, IInterceptor[] interceptors)
   at Castle.DynamicProxy.ProxyGenerator.CreateInterfaceProxyWithoutTarget(Type theInterface, Type[] interfaces, IInterceptor[] interceptors)
   at Rhino.Mocks.MockRepository.MockInterface(CreateMockState mockStateFactory, Type type, Type[] extras)
   at Rhino.Mocks.MockRepository.CreateMockObject(Type type, CreateMockState factory, Type[] extras, Object[] argumentsForConstructor)
   at Rhino.Mocks.MockRepository.StrictMock[T](Object[] argumentsForConstructor)
   at Rhino.Mocks.Tests.FieldsProblem.FieldProblem_Shanon.CanMockInterfaceWithMethodsHavingModOpt() in c:\Documents and Settings\jmeckley\My Documents\Visual Studio 2005\Projects\Rhino-Tools\trunk\rhino-mocks\Rhino.Mocks.Tests\FieldsProblem\FieldProblem_Shanon.cs:line 13")]
		public void CanMockInterfaceWithMethodsHavingModOpt()
		{
			MockRepository mocks = new MockRepository();
			IHaveMethodWithModOpts mock = mocks.StrictMock<IHaveMethodWithModOpts>();
			Assert.NotNull(mock);
		}
	}
}
