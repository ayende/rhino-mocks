namespace Rhino.Mocks.BooMacros

import System

class Allow_unsubscribeMacro(AbstractEventMacro):
	
	override def CreateMockMemberInvokeBuilder():
		return MockEventUnsubscribeBuilder(Event)
	
	override def CreateRhinoInvokeBuilder():
		return AllowInvokeBuilder()
