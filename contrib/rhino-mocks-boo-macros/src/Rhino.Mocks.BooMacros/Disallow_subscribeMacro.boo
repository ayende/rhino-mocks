namespace Rhino.Mocks.BooMacros

import System

class Disallow_subscribeMacro(AbstractEventMacro):
	
	override def CreateMockMemberInvokeBuilder():
		return MockEventSubscribeBuilder(Event)
	
	override def CreateRhinoInvokeBuilder():
		return DisallowInvokeBuilder()
