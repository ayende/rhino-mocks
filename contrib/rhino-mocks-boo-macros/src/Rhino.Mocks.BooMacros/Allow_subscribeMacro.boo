namespace Rhino.Mocks.BooMacros

import System

class Allow_subscribeMacro(AbstractEventMacro):
	
	override def CreateMockMemberInvokeBuilder():
		return MockEventSubscribeBuilder(Event)
	
	override def CreateRhinoInvokeBuilder():
		return AllowInvokeBuilder()
