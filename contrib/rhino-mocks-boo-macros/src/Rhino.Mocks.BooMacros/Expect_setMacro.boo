namespace Rhino.Mocks.BooMacros

import System

class Expect_setMacro(AbstractPropertyMacro):
		
	override def CreateMockMemberInvokeBuilder() as IMockMemberInvokeBuilder:
		return MockPropertySetInvokeBuilder(Property)
		
	override def CreateRhinoInvokeBuilder() as IRhinoMockInvokeBuilder:
		return ExpectInvokeBuilder()

	override SupportsConstraints:
		get:
			return true
			
	override SupportsRaise:
		get:
			return true
			
	override SupportsRepeat:
		get:
			return true
	

