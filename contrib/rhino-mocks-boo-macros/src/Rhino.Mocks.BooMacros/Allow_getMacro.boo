namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class Allow_getMacro(AbstractPropertyMacro):
			
	override def CreateMockMemberInvokeBuilder() as IMockMemberInvokeBuilder:
		return MockPropertyGetInvokeBuilder(Property)

	override def CreateRhinoInvokeBuilder() as IRhinoMockInvokeBuilder:
		return AllowInvokeBuilder()
	
	override SupportsReturn:
		get:
			return true

	override SupportsRaise:
		get:
			return true
