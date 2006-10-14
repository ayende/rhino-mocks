namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem

class MockMethodInvokeBuilder(IMockMemberInvokeBuilder):
	private _mie as MethodInvocationExpression
	
	def constructor(mie as MethodInvocationExpression):
		_mie = mie
		
	def Build(context as CompilerContext, block as Block, parentMethod as Method) as Expression:
		args = ExpressionCollection()
		for i in range(_mie.Arguments.Count):
			arg = _mie.Arguments[i]
			type as IType
			if arg isa TryCastExpression:
				tce = arg as TryCastExpression
				name = tce.Type.ToCodeString()
			elif arg isa ReferenceExpression:
				name = arg.ToCodeString()
			else:
				raise ArgumentException("Argument ${arg.ToCodeString()} is invalid. Must be a type reference or parameter declaration.")
				
			e as ITypedEntity = context.NameResolutionService.Resolve(name)
			type = e.Type
			v = CreateDefaultValue(type, context.TypeSystemServices, parentMethod)
			args.Add(v)
			
		_mie.Arguments = args
		
		block.Insert(0, _mie)
		return null
		
	def GetMockInstance(mockCall as Expression) as Expression:
		return ((mockCall as MethodInvocationExpression).Target as MemberReferenceExpression).Target

	def GetParameterNames(mockCall as Expression) as (string):
		params = []
		for a in _mie.Arguments:
			if a isa TryCastExpression:
				tce = a as TryCastExpression
				params.Add(tce.Target.ToCodeString())
			else:
				params.Add(string.Empty)
		return params.ToArray(string)
