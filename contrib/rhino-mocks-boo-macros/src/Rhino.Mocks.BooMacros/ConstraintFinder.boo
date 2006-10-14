namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class ConstraintFinder(DepthFirstVisitor):
	
	private _constraints = []
	
	override def OnSlicingExpression(node as SlicingExpression):
		super.OnSlicingExpression(node)
		
		constraint = node.Indices[0].Begin
		if node.Indices.Count > 1:
			for i in range(1, node.Indices.Count):
				a = ast { Rhino.Mocks.Constraints.And() }
				a.Arguments.Add(constraint)
				a.Arguments.Add(node.Indices[i].Begin)
				constraint = a
		
		name = node.Target.ToCodeString()
		_constraints.Add(ParameterConstraint(name, constraint))

	Constraints as (ParameterConstraint):
		get:
			return _constraints.ToArray(ParameterConstraint)
