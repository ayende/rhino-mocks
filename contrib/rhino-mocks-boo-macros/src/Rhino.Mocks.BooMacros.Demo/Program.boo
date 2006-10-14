namespace Rhino.Mocks.BooMacros.Demo

import Rhino.Mocks
import Rhino.Mocks.BooMacros

interface IFoo:
	def Bar()
	def Cat() as int
	def Dog(x as int, y as string, z as int) as single
	event Completed as System.EventHandler
	Name as string:
		get
		set
				
def AllowMethod():
	record mocks:
		foo as IFoo = mocks.CreateMock(IFoo)
		allow foo.Bar()
	
	verify mocks:
		foo.Bar()

def AllowMethodWithImplicitReturn():
	record mocks:
		foo as IFoo = mocks.CreateMock(IFoo)
		allow foo.Cat()
	
	verify mocks:
		print "Cat value = ${foo.Cat()}"

def AllowMethodWithExplicitReturn():
	mocks = MockRepository()
	foo as IFoo = mocks.CreateMock(IFoo)
	allow foo.Cat():
		return 42
	mocks.ReplayAll()
	
	print "Cat value = ${foo.Cat()}"
	
	mocks.VerifyAll()
	
def AllowMethodWithConstraints():
	mocks = MockRepository()
	foo as IFoo = mocks.CreateMock(IFoo)
	allow foo.Dog(x as int, y as string, int):
		x [ Is.LessThanOrEqual(100) ]
		y [ Text.Contains("foo") ]

	mocks.ReplayAll()
	
	foo.Dog(1, "hello foo test", 1)
	
	try:
		foo.Dog(101, "wrong", 1)
	except ex as Exceptions.ExpectationViolationException:
		print ex.Message
		
	mocks.VerifyAll()
	
def ExpectMethodWithConstraintsAndReturn():
	record mocks:
		foo as IFoo = mocks.CreateMock(IFoo)
		expect foo.Dog(int, x as string, y as int):
			x [ Is.NotNull() ]
			y [ Is.GreaterThan(0) & Is.LessThan(10) ]
			return 42.0
	
	verify mocks:
		foo.Dog(0, "hello", 5)

def ExpectMethodWithRepeat():
	record mocks:
		foo as IFoo = mocks.CreateMock(IFoo)
		expect foo.Dog(int, string, int):
			repeat 3
	try:			
		verify mocks:
			foo.Dog(0, "hello", 5)
			foo.Dog(0, "hello", 5)
			#foo.Dog(0, "hello", 5)
	except ex as Exceptions.ExpectationViolationException:
		print ex.Message

def ExpectEventSubscribe():
	record mocks:
		foo as IFoo = mocks.CreateMock(IFoo)
		expect_subscribe foo.Completed
		
	verify mocks:
		foo.Completed += def:
			pass
			
def DisallowEventSubscribe():
	record mocks:
		foo as IFoo = mocks.CreateMock(IFoo)
		disallow_subscribe foo.Completed
	
	try:
		verify mocks:
			foo.Completed += def:
				pass
	except ex as Exceptions.ExpectationViolationException:
		print ex.Message
		
def PropertyBehavior():
	record mocks:
		foo as IFoo = mocks.CreateMock(IFoo)
		mock_property foo.Name
		
	verify mocks:
		foo.Name = "Bob"
		print foo.Name
	
try:
	AllowMethod()
	AllowMethodWithImplicitReturn()
	AllowMethodWithExplicitReturn()
	AllowMethodWithConstraints()
	ExpectMethodWithRepeat()
	ExpectEventSubscribe()
	DisallowEventSubscribe()
	PropertyBehavior()
except ex as System.Exception:
	print "unhandled exception:"
	print ex.ToString()
	
print "done"
gets()
