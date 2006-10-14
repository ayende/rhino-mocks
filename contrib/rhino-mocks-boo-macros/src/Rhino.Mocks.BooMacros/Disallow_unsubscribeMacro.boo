namespace Rhino.Mocks.BooMacros

import System

class Disallow_unsubscribeMacro(AbstractEventMacro):
	
	override def CreateMockMemberInvokeBuilder():
		return MockEventUnsubscribeBuilder(Event)
	
	override def CreateRhinoInvokeBuilder():
		return DisallowInvokeBuilder()
