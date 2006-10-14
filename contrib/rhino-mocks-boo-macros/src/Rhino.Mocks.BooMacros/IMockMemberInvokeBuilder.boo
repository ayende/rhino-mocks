namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

interface IMockMemberInvokeBuilder:
	def Build(context as CompilerContext, block as Block, parentMethod as Method) as Expression
	def GetMockInstance(mockCall as Expression) as Expression
	def GetParameterNames(mockCall as Expression) as (string)
