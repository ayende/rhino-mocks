namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler.Ast

class RepeatFinder(DepthFirstVisitor):
	[getter(RepeatType)]
	private _repeatType as RepeatType = RepeatType.None
	[getter(RepeatAmountMin)]
	private _repeatAmountMin as long
	[getter(RepeatAmountMax)]
	private _repeatAmountMax as long
	[getter(ErrorMessage)]
	private _errorMessage as string
	
	override def OnMethodInvocationExpression(node as MethodInvocationExpression):
		return unless node.Target isa ReferenceExpression
		return unless (node.Target as ReferenceExpression).Name == "repeat"
		
		if node.Arguments.Count < 1:
			Error("repeat must be called with 1 or 2 arguments.")
			return
		if node.Arguments.Count > 2:
			Error("repeat must be called with 1 or 2 arguments.")
			return
			
		min = node.Arguments[0]
		max as Expression
		
		unless min isa IntegerLiteralExpression:
			Error("repeat min value must be an integer greater than 0.")
			return
		
		if node.Arguments.Count == 2:
			max = node.Arguments[1]
			unless max isa IntegerLiteralExpression:
				Error("repeat max value must be an integer greater than 0.")
				return
		
		_repeatAmountMin = (min as IntegerLiteralExpression).Value
		unless _repeatAmountMin >= 0:
			Error("Range min must be greater than -1.")
			return
			
		if node.Arguments.Count == 2:
			_repeatAmountMax = (max as IntegerLiteralExpression).Value 
			unless _repeatAmountMax >= 0:
				Error("Range max must be greater than -1.")
				return
		
			unless _repeatAmountMax > _repeatAmountMin:
				Error("repeat min value must be less than max value.")
				return
		else:
			_repeatAmountMax = -1
		
		if _repeatAmountMax == -1:
			_repeatType = RepeatType.SingleValue
		else:
			_repeatType = RepeatType.Range
			
	private def Error(message as string):
		_repeatAmountMin = -1
		_repeatAmountMax = -1
		_repeatType = RepeatType.Error
		_errorMessage = message
