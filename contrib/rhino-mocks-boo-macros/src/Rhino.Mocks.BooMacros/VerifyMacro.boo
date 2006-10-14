namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class VerifyMacro(AbstractAstMacro):
	
	override def Expand(macro as MacroStatement):
		assert macro.Arguments.Count == 1
		assert macro.Arguments[0] isa ReferenceExpression
		mocks = macro.Arguments[0] as ReferenceExpression
		
		block = Block()
		block.Add(macro.Block)
		block.Add(MethodInvocationExpression(MemberReferenceExpression(mocks, "VerifyAll")))
		
		return block

