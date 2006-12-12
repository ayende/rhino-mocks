using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Impl
{
    /// <summary>
    /// This class is reponsible for taking a delegate and creating a wrapper
    /// interface around it, so it can be mocked.
    /// </summary>
    internal class DelegateTargetInterfaceCreator
    {
        private long counter=0;
        
        /// <summary>
        /// The scope for all the delegate interfaces create by this mock repositroy.
        /// </summary>
        private ModuleScope moduleScope = new ModuleScope();

        private IDictionary delegateTargetInterfaces = new Hashtable();
        
        /// <summary>
        /// Gets a type with an "Invoke" method suitable for use as a target of the
        /// specified delegate type.
        /// </summary>
        /// <param name="delegateType"></param>
        /// <returns></returns>
        public Type GetDelegateTargetInterface(Type delegateType)
        {
            Type type;
            lock (delegateTargetInterfaces)
            {
                type = (Type)delegateTargetInterfaces[delegateType];

                if (type == null)
                {
                    type = CreateCallableInterfaceFromDelegate(delegateType);
                    delegateTargetInterfaces[delegateType] = type;
                }
            }
            return type;
        }
        
        private Type CreateCallableInterfaceFromDelegate(Type delegateType)
        {
            Type type;
            long count = Interlocked.Increment(ref counter);
            TypeBuilder typeBuilder = moduleScope.ObtainDynamicModule().DefineType(
                string.Format("ProxyDelegate_{0}_{1}", delegateType.Name, count),
                TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public);

            MethodInfo invoke = delegateType.GetMethod("Invoke");
            ParameterInfo[] parameters = invoke.GetParameters();

            Type returnType = invoke.ReturnType;
            Type[] parameterTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                parameterTypes[i] = parameters[i].ParameterType;

            typeBuilder.DefineMethod("Invoke", MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.Public,
                                     CallingConventions.HasThis, returnType, parameterTypes);

            type = typeBuilder.CreateType();
            return type;
        }
    }
}
