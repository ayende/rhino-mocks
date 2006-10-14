namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

abstract class AbstractEventMacro(AbstractRhinoMockMacro):
	
	private _event as MemberReferenceExpression
	
	protected Event as MemberReferenceExpression:
		get:
			return _event
			
	override def ValidateMacro(macro as MacroStatement):
		super.ValidateMacro(macro)
		assert macro.Arguments[0] isa MemberReferenceExpression, "Must call with member reference."
		
	override def PreExpand(macro as MacroStatement):
		_event = macro.Arguments[0] as MemberReferenceExpression

			
