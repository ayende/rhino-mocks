using System;
using System.Text;

using NUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class OrderedTests
    {
        private delegate void ConfigureMockDelegate(MockRepository mocks, I1 mockObject);

        #region TestOrderedTimesMin0
        [Test(Description="Test that a Times min of 0 works correctly inside an ordered recorder")]
        public void TestOrderedTimesMin0()
        {
            ConfigureMockDelegate deleg = new ConfigureMockDelegate(ConfigureOrderedTimesMin0);

            // Let's check all the valid possibilities
            AssertOrderValidity(deleg, true, "");
            AssertOrderValidity(deleg, true, "A");
            AssertOrderValidity(deleg, true, "C");
            AssertOrderValidity(deleg, true, "B");
            AssertOrderValidity(deleg, true, "AB");
            AssertOrderValidity(deleg, true, "AC");
            AssertOrderValidity(deleg, true, "BC");
            AssertOrderValidity(deleg, true, "ABC");

            // Let's check some invalid ones
            AssertOrderValidity(deleg, false, "CB");
            AssertOrderValidity(deleg, false, "CA");
            AssertOrderValidity(deleg, false, "BA");
            AssertOrderValidity(deleg, false, "AA");
            AssertOrderValidity(deleg, false, "ABB");
        }

        private static void ConfigureOrderedTimesMin0(MockRepository mocks, I1 mockObject)
        {
            using (mocks.Ordered())
            {
                mockObject.A();
                LastCall.Repeat.Times(0, 1);
                mockObject.B();
                LastCall.Repeat.Times(0, 1);
                mockObject.C();
                LastCall.Repeat.Times(0, 1);
            }
        }
        #endregion

        #region TestOrderedTimesMinNonZeroMaxHigherThanMin
        [Test(Description="Test that a Times non-zero min with a higher max works correctly inside an ordered recorder")]
        public void TestOrderedTimesMinNonZeroMaxHigherThanMin()
        {
            ConfigureMockDelegate deleg = new ConfigureMockDelegate(ConfigureOrderedTimesMinNonZeroMaxHigherThanMin);

            // Let's check all the valid possibilities
            AssertOrderValidity(deleg, true, "ABC");
            AssertOrderValidity(deleg, true, "ABBC");
            AssertOrderValidity(deleg, true, "ABBBC");

            // Let's check some invalid ones
            AssertOrderValidity(deleg, false, "BC"); // Missing A
            AssertOrderValidity(deleg, false, "AC"); // Too few Bs
            AssertOrderValidity(deleg, false, "ABBBBC"); // Too many Bs
        }

        private static void ConfigureOrderedTimesMinNonZeroMaxHigherThanMin(MockRepository mocks, I1 mockObject)
        {
            using (mocks.Ordered())
            {
                mockObject.A();
                mockObject.B();
                LastCall.Repeat.Times(1, 3);
                mockObject.C();
            }
        }
        #endregion

        #region TestOrderedAtLeastOnce
        [Test(Description = "Test that 'at least once' works correctly inside an ordered recorder")]
        public void TestOrderedAtLeastOnce()
        {
            ConfigureMockDelegate deleg = new ConfigureMockDelegate(ConfigureOrderedAtLeastOnce);

            // Let's check some of the valid possibilities
            AssertOrderValidity(deleg, true, "ABC");
            AssertOrderValidity(deleg, true, "ABBC");
            AssertOrderValidity(deleg, true, "ABBBBBBBBBBBBBBBBBBBBC");

            // Let's check some invalid ones
            AssertOrderValidity(deleg, false, "BC"); // Missing A
            AssertOrderValidity(deleg, false, "AC"); // Too few Bs
        }

        private static void ConfigureOrderedAtLeastOnce(MockRepository mocks, I1 mockObject)
        {
            using (mocks.Ordered())
            {
                mockObject.A();
                mockObject.B();
                LastCall.Repeat.AtLeastOnce();
                mockObject.C();
            }
        }
        #endregion

        #region TestOrderedTimesMin0WithNestedUnordered
        [Test(Description = "Test that a Times min of 0 works correctly when followed by an nested unordered section")]
        public void TestOrderedTimesMin0WithNestedUnordered()
        {
            ConfigureMockDelegate deleg = new ConfigureMockDelegate(ConfigureOrderedTimesMin0WithNestedUnordered);

            // Let's check all the valid possibilities
            AssertOrderValidity(deleg, true, "BC");
            AssertOrderValidity(deleg, true, "CB");
            AssertOrderValidity(deleg, true, "ABC");
            AssertOrderValidity(deleg, true, "ACB");
            AssertOrderValidity(deleg, true, "BCA");
            AssertOrderValidity(deleg, true, "CBA");
            AssertOrderValidity(deleg, true, "ABCA");
            AssertOrderValidity(deleg, true, "ACBA");

            // Let's check some invalid ones
            AssertOrderValidity(deleg, false, "");
            AssertOrderValidity(deleg, false, "AA");
            AssertOrderValidity(deleg, false, "AABC");
            AssertOrderValidity(deleg, false, "AACB");
            AssertOrderValidity(deleg, false, "ABCAA");
        }

        private static void ConfigureOrderedTimesMin0WithNestedUnordered(MockRepository mocks, I1 mockObject)
        {
            using (mocks.Ordered())
            {
                mockObject.A();
                LastCall.Repeat.Times(0, 1);
                using (mocks.Unordered())
                {
                    mockObject.B();
                    mockObject.C();
                }
                mockObject.A();
                LastCall.Repeat.Times(0, 1);
            }
        }
        #endregion

        #region Helper method for performing easy configuration and replay testing
        /// <summary>
        /// Sets up a mock, and runs through methods as specified by characters in <paramref name="methodOrder"/>.
        /// </summary>
        /// <param name="mockConfigurer">A delegate to use to configure the mock.</param>
        /// <param name="isValid">Whether the order should be valid or not.</param>
        /// <param name="methodOrder">The method order (e.g. "ABC", "ABBC").</param>
        private static void AssertOrderValidity( ConfigureMockDelegate mockConfigurer, bool isValid, 
                                                 string methodOrder )
        {
            MockRepository mocks = new MockRepository();

            I1 i = mocks.CreateMock(typeof(I1)) as I1;

            mockConfigurer(mocks, i);

            mocks.ReplayAll();

            try
            {
                foreach (char c in methodOrder)
                {
                    switch (c)
                    {
                        case 'A':
                            i.A();
                            break;
                        case 'B':
                            i.B();
                            break;
                        case 'C':
                            i.C();
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
                mocks.VerifyAll();
            }
            catch (Exceptions.ExpectationViolationException ex)
            {
                if (isValid)
                {
                    Assert.Fail("Order {0} was supposed to be ok, but got error: {1}", methodOrder, ex.Message);
                }
                else
                {
                    return;
                }
            }

            if (!isValid)
            {
                Assert.Fail("Order {0} was supposed to fail, but did not.", methodOrder);
            }
        }
        #endregion
    }

    public interface I1
    {
        void A();
        void B();
        void C();
    }
}
