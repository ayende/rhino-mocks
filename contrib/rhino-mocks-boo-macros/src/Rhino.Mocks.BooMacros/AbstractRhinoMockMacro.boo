namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import Rhino.Mocks.Interfaces

abstract class AbstractRhinoMockMacro(AbstractAstMacro):
	
	abstract def CreateMockMemberInvokeBuilder() as IMockMemberInvokeBuilder:
		pass
		
	abstract def CreateRhinoInvokeBuilder() as IRhinoMockInvokeBuilder:
		pass
	
	virtual SupportsConstraints as bool:
		get:
			return false
		
	virtual SupportsRaise as bool:
		get:
			return false
		
	virtual SupportsRepeat as bool:
		get:
			return false
		
	virtual SupportsReturn as bool:
		get:
			return false
	
	virtual def ValidateMacro(macro as MacroStatement):
		assert macro.Arguments.Count == 1, "Macro must have one argument."
	
	virtual def PreExpand(macro as MacroStatement):
		pass
		
	override def Expand(macro as MacroStatement):		
		ValidateMacro(macro)
		PreExpand(macro)
		
		memberInvokeBuilder = CreateMockMemberInvokeBuilder()
		rhinoInvokeBuilder = CreateRhinoInvokeBuilder()
		# Create the block to insert code into
		block = Block()
		# Get the method/property/event being mocked
		mockCall = macro.Arguments[0]
		mockInstance = memberInvokeBuilder.GetMockInstance(mockCall)
		params = memberInvokeBuilder.GetParameterNames(mockCall)
		
		parentMethod = GetParentMethod(macro)
		
		# Build up the Rhino Mocks method chaining
		expr as Expression
		expr = memberInvokeBuilder.Build(Context, block, parentMethod)
		expr = rhinoInvokeBuilder.Build(mockInstance, expr, Context)
		
		if SupportsConstraints:
			expr = AddConstraints(macro, expr, params)
		else:
			expr = MethodInvocationExpression(
				MemberReferenceExpression(expr, "IgnoreArguments")
			)
			
		addedRaise = false
		if SupportsRaise:
			oldExpr = expr
			expr = AddRaise(macro, expr)
			addedRaise = not object.ReferenceEquals(oldExpr, expr)
						
		if SupportsRepeat:
			expr = AddRepeat(macro, expr)

		if SupportsReturn and not addedRaise:
			expr = AddReturn(macro, expr, mockCall, block, parentMethod)
			
		block.Add(expr)
		
		# Handy way to see the block of code when debugging
		#assert false, block.ToCodeString().Replace("\r\n", "; ")
		
		return block

	private def GetParentMethod(macro as MacroStatement) as Method:
		parent as Node = macro.ParentNode
		while (parent is not null) and not (parent isa Method):
			parent = parent.ParentNode
		
		assert parent is not null, "Macro cannot be used outside of a method."
		
		return parent
		
	private def AddConstraints(macro as MacroStatement, expr as Expression, params as (string)):
		c = ConstraintFinder()
		macro.Block.Accept(c)
		if c.Constraints.Length == 0:
			return MethodInvocationExpression(
				MemberReferenceExpression(expr, "IgnoreArguments")
			)
		
		mie = MethodInvocationExpression(
			MemberReferenceExpression(expr, "Constraints")
		)
		# build an array of constraint expressions
		constraints = array(Expression, params.Length)
		for i, name in enumerate(params):
			if not string.IsNullOrEmpty(name):
				for pc in c.Constraints:
					if pc.Name == name:
						constraints[i] = pc.Expression
						break
			else:
				constraints[i] = null
				
		for c in constraints:
			if c is null:
				# No constraint specified so allow anything
				mie.Arguments.Add(ast { Is.Anything() })
			else:
				mie.Arguments.Add(c)

		return mie

	private def AddRaise(macro as MacroStatement, expr as Expression):
		raiseFinder = RaiseFinder()
		macro.Block.Accept(raiseFinder)
		if raiseFinder.RaiseExpression is null:
			return expr
			
		return MethodInvocationExpression(
			MemberReferenceExpression(expr, "Throw"),
			raiseFinder.RaiseExpression
		)
		
	private def AddRepeat(macro as MacroStatement, expr as Expression):
		repeatFinder = RepeatFinder()
		macro.Block.Accept(repeatFinder)
		assert repeatFinder.RepeatType != RepeatType.Error
		
		if repeatFinder.RepeatType == RepeatType.None:
			return expr
			
		mie = MethodInvocationExpression(
			MemberReferenceExpression(
				MemberReferenceExpression(expr, "Repeat"),
				"Times"
			)
		)
		if repeatFinder.RepeatType == RepeatType.SingleValue:
			mie.Arguments.Add(IntegerLiteralExpression(repeatFinder.RepeatAmountMin))
		elif repeatFinder.RepeatType == RepeatType.Range:
			mie.Arguments.Add(IntegerLiteralExpression(repeatFinder.RepeatAmountMin))
			mie.Arguments.Add(IntegerLiteralExpression(repeatFinder.RepeatAmountMax))
		
		return mie
		
	private def AddReturn(macro as MacroStatement, expr as Expression, mockCall as Expression, block as Block, parentMethod as Method):
		returnFinder = ReturnFinder()
		macro.Block.Accept(returnFinder)
		
		if returnFinder.ReturnExpression is not null:
			return MethodInvocationExpression(
				MemberReferenceExpression(expr, "Return"),
				returnFinder.ReturnExpression
			)
		else:
			# Need to wait until we know expression type
			# Create local references because member variables
			# will be null after the ProcessMethodBodies compiler step
			tss = TypeSystemServices
			cb = CodeBuilder
			
			Parameters.Pipeline.AfterStep += def(sender, e as CompilerStepEventArgs):
				return unless e.Step isa Steps.ProcessMethodBodies
				
				type = tss.GetExpressionType(mockCall)
				
				if type != tss.VoidType:	
					# rhinoCall is last expression in the block
					rhinoCall = (block.Statements[-1] as ExpressionStatement).Expression
					returnValue = CreateDefaultValue(type, tss, parentMethod)
					mie = cb.CreateMethodInvocation(
						rhinoCall,
						tss.Map(typeof(IMethodOptions).GetMethod("Return")),
						returnValue
					)
					# Replace the last statement in the block
					block.Statements.RemoveAt(block.Statements.Count - 1)
					block.Add(mie)
					
			return expr
