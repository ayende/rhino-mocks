namespace Rhino.Mocks.BooMacros

import System

class Expect_unsubscribeMacro(AbstractEventMacro):
	
	override def CreateMockMemberInvokeBuilder():
		return MockEventUnsubscribeBuilder(Event)
	
	override def CreateRhinoInvokeBuilder():
		return ExpectInvokeBuilder()

	override SupportsRepeat:
		get:
			return true
