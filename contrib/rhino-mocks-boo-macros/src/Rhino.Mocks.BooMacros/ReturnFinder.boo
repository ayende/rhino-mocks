namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler.Ast

class ReturnFinder(DepthFirstVisitor):
	[property(ReturnExpression)]
	private _returnExpression as Expression
	
	override def OnReturnStatement(node as ReturnStatement):
		_returnExpression = node.Expression
