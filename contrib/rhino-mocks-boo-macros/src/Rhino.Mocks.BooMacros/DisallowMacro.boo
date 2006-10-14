namespace Rhino.Mocks.BooMacros

import System

class DisallowMacro(AbstractMethodMacro):
			
	override def CreateMockMemberInvokeBuilder() as IMockMemberInvokeBuilder:
		return MockMethodInvokeBuilder(MethodInvocationExpression)
		
	override def CreateRhinoInvokeBuilder() as IRhinoMockInvokeBuilder:
		return DisallowInvokeBuilder()

	override SupportsConstraints:
		get:
			return true
			
	override SupportsRaise:
		get:
			return false
			
	override SupportsRepeat:
		get:
			return false
	
	override SupportsReturn:
		get:
			return false
