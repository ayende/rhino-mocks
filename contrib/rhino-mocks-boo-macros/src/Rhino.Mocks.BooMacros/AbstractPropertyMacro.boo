namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler.Ast

abstract class AbstractPropertyMacro(AbstractRhinoMockMacro):
	
	private _property as MemberReferenceExpression
	
	protected Property:
		get:
			return _property
	
	override def ValidateMacro(macro as MacroStatement):
		super.ValidateMacro(macro)
		assert macro.Arguments[0] isa MemberReferenceExpression, "Must call with a property accessor."
		
	override def PreExpand(macro as MacroStatement):
		_property = macro.Arguments[0] as MemberReferenceExpression
