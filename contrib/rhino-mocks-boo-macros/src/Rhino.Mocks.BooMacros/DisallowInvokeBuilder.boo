namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Rhino.Mocks

class DisallowInvokeBuilder(IRhinoMockInvokeBuilder):

	def constructor():
		pass
	
	def Build(mockInstance as Expression, mockCall as Expression, context as CompilerContext) as MethodInvocationExpression:
		mie as MethodInvocationExpression
		if mockCall is null:
			mie = ast { LastCall.On() }
			mie.Arguments.Add(mockInstance)
		else:
			mie = ast { Expect.On() }
			mie.Arguments.Add(mockInstance)
			mie = MethodInvocationExpression(
				MemberReferenceExpression(mie, "Call"),
				mockCall
			)
		mie = MethodInvocationExpression(
			MemberReferenceExpression(
				MemberReferenceExpression(mie, "Repeat"),
				"Never"
			)
		)
		return mie

