namespace Rhino.Mocks.BooMacros

import System

class Expect_subscribeMacro(AbstractEventMacro):
	
	override def CreateMockMemberInvokeBuilder():
		return MockEventSubscribeBuilder(Event)
	
	override def CreateRhinoInvokeBuilder():
		return ExpectInvokeBuilder()

	override SupportsRepeat:
		get:
			return true
