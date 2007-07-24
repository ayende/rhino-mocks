using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Wendy
    {
        private ISearchPatternBuilder _searchPatternBuilder;
        private MockRepository _mocks;
        private ImageFinder _imageFinder;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _searchPatternBuilder = _mocks.DynamicMock<ISearchPatternBuilder>();
            _imageFinder = new ImageFinder(_searchPatternBuilder);
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException),"ISearchPatternBuilder.CreateFromExtensions([\"png\", \"gif\", \"jpg\", \"bmp\"]); Expected #1, Actual #0.")]
        public void SendingNullParamsValueShouldNotThrowNullReferenceException()
        {
            Expect.Call(_searchPatternBuilder.CreateFromExtensions(ImageFinder.ImageExtensions))
                .Return(null);
            _mocks.ReplayAll();
            _imageFinder.FindImagePath();
            Verify(_searchPatternBuilder);
        }

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),
		   "ISearchPatternBuilder.CreateFromExtensions([]); Expected #0, Actual #1.")]
		public void VerifyShouldFailIfDynamicMockWasCalledWithRepeatNever()
		{
			_searchPatternBuilder.CreateFromExtensions();
			LastCall.Repeat.Never();
			_mocks.ReplayAll();
			try
			{
				_searchPatternBuilder.CreateFromExtensions();
			}
			catch 
			{
				
			}
			Verify(_searchPatternBuilder);
		}

        private void Verify(ISearchPatternBuilder builder)
        {
            _mocks.Verify(builder);
        }

        public string FindImagePath(string directoryToSearch)
        {
            _searchPatternBuilder.CreateFromExtensions(null);
            return null;
        }

        public interface ISearchPatternBuilder
        {
            string CreateFromExtensions(params string[] extensions);
        }

        public class ImageFinder
        {
            private readonly ISearchPatternBuilder builder;
            public static string[] ImageExtensions = { "png", "gif", "jpg", "bmp" };

            public ImageFinder(ISearchPatternBuilder builder)
            {
                this.builder = builder;
            }

            public void FindImagePath()
            {
                builder.CreateFromExtensions(null);
            }
        }
    }
}
