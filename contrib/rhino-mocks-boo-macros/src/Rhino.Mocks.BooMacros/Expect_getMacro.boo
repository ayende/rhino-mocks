namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class Expect_getMacro(AbstractPropertyMacro):
			
	override def CreateMockMemberInvokeBuilder() as IMockMemberInvokeBuilder:
		return MockPropertyGetInvokeBuilder(Property)

	override def CreateRhinoInvokeBuilder() as IRhinoMockInvokeBuilder:
		return ExpectInvokeBuilder()
			
	override SupportsRaise:
		get:
			return true
			
	override SupportsRepeat:
		get:
			return true
	
	override SupportsReturn:
		get:
			return true
