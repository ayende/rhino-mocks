namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class MockPropertyGetInvokeBuilder(IMockMemberInvokeBuilder):
	private _property as MemberReferenceExpression
	
	def constructor(property as MemberReferenceExpression):
		_property = property
		
	def Build(context as CompilerContext, block as Block, parentMethod as Method) as Expression:
		return _property

	def GetMockInstance(mockCall as Expression) as Expression:
		return (mockCall as MemberReferenceExpression).Target

	def GetParameterNames(mockCall as Expression) as (string):
		return array(string, 0)
