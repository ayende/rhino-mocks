namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler.Ast

class Allow_setMacro(AbstractPropertyMacro):
		
	override def CreateMockMemberInvokeBuilder() as IMockMemberInvokeBuilder:
		return MockPropertySetInvokeBuilder(Property)
		
	override def CreateRhinoInvokeBuilder() as IRhinoMockInvokeBuilder:
		return AllowInvokeBuilder()

	override SupportsConstraints:
		get:
			return true
			
	override SupportsRaise:
		get:
			return true
