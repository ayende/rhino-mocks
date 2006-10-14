namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class AllowMacro(AbstractMethodMacro):
		
	override def CreateMockMemberInvokeBuilder() as IMockMemberInvokeBuilder:
		return MockMethodInvokeBuilder(MethodInvocationExpression)
		
	override def CreateRhinoInvokeBuilder() as IRhinoMockInvokeBuilder:
		return AllowInvokeBuilder()

	override SupportsConstraints:
		get:
			return true
			
	override SupportsRaise:
		get:
			return true
	
	override SupportsReturn:
		get:
			return true
