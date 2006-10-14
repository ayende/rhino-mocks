namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler.Ast

class ParameterConstraint:
	[getter(Name)]
	private _name as string
	[getter(Expression)]
	private _expression as Expression

	def constructor(name as string, expression as Expression):
		_name = name
		_expression = expression
