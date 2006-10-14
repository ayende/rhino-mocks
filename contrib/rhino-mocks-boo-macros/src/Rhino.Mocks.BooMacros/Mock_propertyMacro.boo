namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Rhino.Mocks

class Mock_propertyMacro(AbstractAstMacro):

	override def Expand(macro as MacroStatement):
		assert macro.Arguments.Count == 1
		assert macro.Arguments[0] isa MemberReferenceExpression
		property = macro.Arguments[0] as MemberReferenceExpression
		mock = property.Target
		
		# Build:
		# SetupResult.On(mock).Call(mock.Property).PropertyBehavior()
		
		code = ast { SetupResult.On() }
		code.Arguments.Add(mock)
		code = MethodInvocationExpression(
			MemberReferenceExpression(
				code,
				"Call"
			),
			property
		)
		code = MethodInvocationExpression(
			MemberReferenceExpression(
				code,
				"PropertyBehavior"
			)
		)
		return ExpressionStatement(code)
