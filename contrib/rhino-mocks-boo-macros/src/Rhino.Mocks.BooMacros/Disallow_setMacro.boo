namespace Rhino.Mocks.BooMacros

import System

class Disallow_setMacro(AbstractPropertyMacro):
	
	override def CreateMockMemberInvokeBuilder() as IMockMemberInvokeBuilder:
		return MockPropertySetInvokeBuilder(Property)
		
	override def CreateRhinoInvokeBuilder() as IRhinoMockInvokeBuilder:
		return DisallowInvokeBuilder()

	override SupportsConstraints:
		get:
			return true
			
