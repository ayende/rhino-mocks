namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler.Ast

class RaiseFinder(DepthFirstVisitor):
	[property(RaiseExpression)]
	private _raiseExpression as Expression
	
	override def OnRaiseStatement(node as RaiseStatement):
		_raiseExpression = node.Exception
