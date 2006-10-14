namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem

def CreateDefaultValue(type as IType, tss as TypeSystemServices, parentMethod as Method) as Expression:
	if not type.IsValueType:
		return NullLiteralExpression()
	
	localName = tss.CodeBuilder.CreateTempName()
	local = tss.CodeBuilder.DeclareTempLocal(parentMethod, type)
	parentMethod.Locals.Add(local.Local)
	return tss.CodeBuilder.CreateLocalReference(localName, local)

	

