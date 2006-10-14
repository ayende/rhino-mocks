namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

interface IRhinoMockInvokeBuilder:
	
	def Build(mockInstance as Expression, mockCall as Expression, context as CompilerContext) as MethodInvocationExpression


