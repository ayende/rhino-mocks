namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler.Ast

abstract class AbstractMethodMacro(AbstractRhinoMockMacro):
	
	private _mie as MethodInvocationExpression
	
	protected MethodInvocationExpression:
		get:
			return _mie

	override def ValidateMacro(macro as MacroStatement):
		super.ValidateMacro(macro)
		assert macro.Arguments[0] isa MethodInvocationExpression, "Must call with method invocation."
		
	override def PreExpand(macro as MacroStatement):
		_mie = macro.Arguments[0] as MethodInvocationExpression
		
