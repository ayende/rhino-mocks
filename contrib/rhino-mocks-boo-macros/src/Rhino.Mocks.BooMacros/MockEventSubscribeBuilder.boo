namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem

class MockEventSubscribeBuilder(IMockMemberInvokeBuilder):

	private _event as MemberReferenceExpression
	
	def constructor(evt as MemberReferenceExpression):
		_event = evt
	
	def Build(context as CompilerContext, block as Block, parentMethod as Method) as Expression:
		add = BinaryExpression(
			BinaryOperatorType.InPlaceAddition,
			_event,
			ast { null }
		)
		block.Insert(0, add)
	
	def GetMockInstance(mockCall as Expression) as Expression:
		return (mockCall as MemberReferenceExpression).Target
	
	def GetParameterNames(mockCall as Expression) as (string):
		return array(string, 0)
		
	

