namespace Rhino.Mocks.BooMacros

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem

class MockPropertySetInvokeBuilder(IMockMemberInvokeBuilder):
	private _property as MemberReferenceExpression
	
	def constructor(property as MemberReferenceExpression):
		_property = property
		
	def Build(context as CompilerContext, block as Block, parentMethod as Method) as Expression:
		tss = context.TypeSystemServices
		nrs = context.NameResolutionService
		cb = context.CodeBuilder
		
		# Wait until we can establish the type of the property
		# so that we can create a default value for it
		context.Parameters.Pipeline.AfterStep += def(sender, e as CompilerStepEventArgs):
			return unless e.Step isa Steps.ProcessMethodBodies
			
			# Build a properly bound property setter
			type = _property.Target.ExpressionType
			prop = nrs.ResolveProperty(type, _property.Name)
			value = CreateDefaultValue(prop.Type, tss, parentMethod)
			setter = cb.CreatePropertySet(_property.Target, prop, value)
			
			block.Insert(0, setter)
			
		return null
	
	def GetMockInstance(mockCall as Expression) as Expression:
		return (mockCall as MemberReferenceExpression).Target

	def GetParameterNames(mockCall as Expression) as (string):
		return ("value", )
