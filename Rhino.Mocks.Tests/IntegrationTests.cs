using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests
{
    public class BirdVeterinary
    {
        private Cage cage;

        public BirdVeterinary()
        {
            this.cage = new Cage( /*cage information*/);
        }

        public void Mate(ISongBird male, ISongBird female)
        {
            female.MoveToCage(cage);
            male.MoveToCage(cage);
            male.Eat("seeds", 250);
            female.Eat("seeds", 250);
            male.Mate(female);
            female.Mate(male);
        }
    }

    public abstract class ProcessorBase
    {
        public int Register;
        
        public virtual int Inc()
        {
            Register = Add(1);
            return Register;
        }

        public abstract int Add(int i);
    }
    
    public class Cage
    {
        ///lots of info about cage
        ///... ,,, ...
        ///,,, ... ,,,

    }

    public interface ISongBird
    {
        void Eat(string type, int quantity);
        string Sing();
        void Mate(ISongBird songBird);
        void MoveToCage(Cage cage);
    }

    [TestFixture]
    public class IntegrationTests
    {
        public delegate bool CageDelegate(Cage cage);

        public Cage recordedCage;

[Test]
public void UsingPartialMocks()
{
    MockRepository mocks = new MockRepository();
    ProcessorBase proc = (ProcessorBase) mocks.PartialMock(typeof (ProcessorBase));
    Expect.Call(proc.Add(1)).Return(1);
    Expect.Call(proc.Add(1)).Return(2);
    
    mocks.ReplayAll();
    
    proc.Inc();
    Assert.AreEqual(1, proc.Register);
    proc.Inc();
    Assert.AreEqual(2, proc.Register);
    
    mocks.VerifyAll();
}
        
        [Test]
        public void ExampleUsingCallbacks()
        {
            MockRepository mocks = new MockRepository();

            ISongBird maleBird = (ISongBird)mocks.CreateMock(typeof(ISongBird)),
                femaleBird = (ISongBird)mocks.CreateMock(typeof(ISongBird));

            using (mocks.Ordered())
            {
                using (mocks.Unordered())
                {
                    maleBird.MoveToCage(null);
                    LastCall.On(maleBird).Callback(new CageDelegate(IsSameCage));
                    femaleBird.MoveToCage(null);
                    LastCall.On(femaleBird).Callback(new CageDelegate(IsSameCage));
                }
                maleBird.Eat("seeds", 250);
                femaleBird.Eat("seeds", 250);
                using (mocks.Unordered())
                {
                    maleBird.Mate(femaleBird);
                    femaleBird.Mate(maleBird);
                }
            }
            mocks.ReplayAll();

            BirdVeterinary vet = new BirdVeterinary();
            vet.Mate(maleBird, femaleBird);
            mocks.VerifyAll();
        }

        [Test]
        public void ExampleUsingParameterMatchingAndConstraints()
        {
            MockRepository mocks = new MockRepository();
            ISongBird bird = (ISongBird)mocks.CreateMock(typeof(ISongBird));
            bird.Eat("seeds", 500); //verifying expected values
            bird.Sing();
            LastCall.On(bird).Return("Chirp, Chirp");
            bird.Sing();
            string exceptionMessage = "No food, no song";
            LastCall.On(bird).Throw(new Exception(exceptionMessage));
            mocks.ReplayAll();

            bird.Eat("seeds", 500);
            Assert.AreEqual("Chirp, Chirp", bird.Sing());
            try
            {
                bird.Sing();
                Assert.Fail("Exception expected");
            }
            catch (Exception e)
            {
                Assert.AreEqual(exceptionMessage, e.Message);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void UnorderedExecutionOfOrderedSequence()
        {
            MockRepository mocks = new MockRepository();
            ISongBird maleBird = (ISongBird)mocks.CreateMock(typeof(ISongBird)),
                femaleBird = (ISongBird)mocks.CreateMock(typeof(ISongBird));

            using (mocks.Ordered())
            {
                maleBird.Eat("seeds", 250);
                femaleBird.Eat("seeds", 250);
            }

            using (mocks.Ordered())
            {
                maleBird.Mate(femaleBird);
                femaleBird.Mate(maleBird);
            }
            mocks.ReplayAll();

            maleBird.Mate(femaleBird);
            femaleBird.Mate(maleBird);

            maleBird.Eat("seeds", 250);
            femaleBird.Eat("seeds", 250);
            mocks.VerifyAll();
        }

        [Test]
        public void OrderedExecutionOfUnorderedSequence()
        {
            MockRepository mocks = new MockRepository();
            ISongBird maleBird = (ISongBird)mocks.CreateMock(typeof(ISongBird)),
                femaleBird = (ISongBird)mocks.CreateMock(typeof(ISongBird));

            using (mocks.Ordered())
            {
                using (mocks.Unordered())
                {
                    maleBird.Eat("seeds", 250);
                    femaleBird.Eat("seeds", 250);
                }

                using (mocks.Unordered())
                {
                    maleBird.Mate(femaleBird);
                    femaleBird.Mate(maleBird);
                }
            }
            mocks.ReplayAll();

            femaleBird.Eat("seeds", 250);
            maleBird.Eat("seeds", 250);

            femaleBird.Mate(maleBird);
            maleBird.Mate(femaleBird);

            mocks.VerifyAll();
        }

        [Test]
        public void SetupResultWithNestedOrdering()
        {
            MockRepository mocks = new MockRepository();
            ISongBird maleBird = (ISongBird)mocks.CreateMock(typeof(ISongBird)),
                femaleBird = (ISongBird)mocks.CreateMock(typeof(ISongBird));

            SetupResult.On(maleBird).Call(maleBird.Sing()).Return("");
            using (mocks.Ordered())
            {
                using (mocks.Unordered())
                {
                    maleBird.Eat("seeds", 250);
                    femaleBird.Eat("seeds", 250);
                }

                using (mocks.Unordered())
                {
                    maleBird.Mate(femaleBird);
                    femaleBird.Mate(maleBird);
                }
            }
            mocks.ReplayAll();

            maleBird.Sing();
            femaleBird.Eat("seeds", 250);
            maleBird.Sing();
            maleBird.Eat("seeds", 250);

            maleBird.Sing();
            femaleBird.Mate(maleBird);
            maleBird.Sing();
            maleBird.Mate(femaleBird);
            maleBird.Sing();
            mocks.VerifyAll();
        }

        private bool IsSameCage(Cage cageFromCallback)
        {
            //Can do any sort of valiation here
            if (this.recordedCage == null)
            {
                this.recordedCage = cageFromCallback;
                return true;
            }
            return this.recordedCage == cageFromCallback;
        }
    }
}