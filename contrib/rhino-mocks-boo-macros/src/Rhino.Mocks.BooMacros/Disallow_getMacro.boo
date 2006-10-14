namespace Rhino.Mocks.BooMacros

import System

class Disallow_getMacro(AbstractPropertyMacro):
			
	override def CreateMockMemberInvokeBuilder() as IMockMemberInvokeBuilder:
		return MockPropertyGetInvokeBuilder(Property)

	override def CreateRhinoInvokeBuilder() as IRhinoMockInvokeBuilder:
		return DisallowInvokeBuilder()

	override SupportsConstraints:
		get:
			return true
			
