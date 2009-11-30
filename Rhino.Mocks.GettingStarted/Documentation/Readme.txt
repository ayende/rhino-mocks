This solution contains samples for RhinoMock taken from the documentation :
http://www.ayende.com/wiki/Rhino+Mocks+Documentation.ashx

Lot of exemples writes in the documentation are taken from real project.
For each one, I wrote the minimum implementation to compile and pass the test

The library used to compile the project are in the directory lib. Currently, it's :
- nunit 2.5.2
- RhinoMock 3.6

Remarks :

01-RhinoMocksIntroduction:
  RhinoMocksIntroductionTest.cs
  line 34 : You need to prefix the second param by ExpectedMessage=, required by Nunit 2.5.2
  
04-RhinoMocksOrderedUnordered
  RhinoMocksOrderedUnorderedTest.cs
  line 73 : Dispose is a method without return. Implementation change to be conform.
  line 59 : Whatever the implementation, I don't succeed to pass this test. I tried by deleting the Unordered calling and these methods calling, it works fine.
    Exception triggered : RhinoMocksIntroduction.RhinoMocksOrderedUnorderedTest.MovingFundsUsingTransactions:
      Rhino.Mocks.Exceptions.ExpectationViolationException : IDatabaseManager.Dispose(); Expected #0, Actual #1.

06-RhinoMocksEvents
  SubscribeToEventTest.cs
  line 44 : I changed the implementation of test VerifyingThatEventWasAttached_AAA. I am not very sure it's correct. If someone could verify.

07-RhinoMocksIEventRaiser :
  line 46 : We needs 2 params to raise an event

08-RhinoMocksProperties
  RhinoMocksPropertiesTest.cs
  Line 50 : Capacity property doesn't implement in IList, I used ArrayList as a Mocking Class
  
Fabien Arcellier