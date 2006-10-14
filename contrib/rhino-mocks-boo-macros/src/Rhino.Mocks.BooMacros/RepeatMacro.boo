namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class RepeatMacro(AbstractAstMacro):

	override def Expand(macro as MacroStatement):
		assert macro.Arguments.Count >= 1
		assert macro.Arguments.Count <= 2
		mie as MethodInvocationExpression = ast { repeat() }
		mie.Arguments = macro.Arguments
		return ExpressionStatement(mie)
		
