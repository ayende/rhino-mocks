using System;
using System.IO;
using System.Net;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_SpookyET
    {
        MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void MockHttpRequesteRsponse()
        {
            byte[] responseData = Encoding.UTF8.GetBytes("200 OK");
            Stream stream = new MemoryStream(responseData);
            WebRequest request = (WebRequest)mocks.CreateMock(typeof(WebRequest));
            WebResponse response = (WebResponse)mocks.CreateMock(typeof(WebResponse));
            Expect.On(request).Call(request.GetResponse()).Return(response);
            Expect.On(response).Call(response.GetResponseStream()).Return(stream);

            mocks.ReplayAll();

            Stream returnedStream = GetResponseStream(request);

            Assert.AreSame(stream, returnedStream);
            string returnedString = new StreamReader(returnedStream).ReadToEnd();
            Assert.AreEqual("200 OK", returnedString);
        }

        /// <summary>
        /// Notice the ordering: First we've a Return and then IgnoreArguments, that
        /// broke because I didn't copy the returnValueSet in the expectation swapping.
        /// </summary>
        [Test]
        public void UsingReturnAndThenIgnoreArgs()
        {
            IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
            Expect.On(demo).Call(demo.StringArgString(null)).Return("ayende").IgnoreArguments();
            mocks.ReplayAll();
            Assert.AreEqual("ayende", demo.StringArgString("rahien"));
        }

        [Test]
        public void WebRequestWhenDisposing()
        {
            MockRepository mockRepository;
            WebRequest webRequestMock;
            WebResponse webResponseMock;

            mockRepository = new MockRepository();
            webRequestMock = (WebRequest)mockRepository.CreateMock(typeof(WebRequest));
            webResponseMock = (WebResponse)mockRepository.CreateMock(typeof(WebResponse));

            using (mockRepository.Ordered())
            {
                Expect.On(webRequestMock).
                    Call(webRequestMock.GetResponse()).
                    Return(webResponseMock);
                Expect.On(webResponseMock).
                    Call(webResponseMock.GetResponseStream()).
                    Return(new MemoryStream());
            }
            webResponseMock.Close();

            mockRepository.ReplayAll();

            WebResponse response = webRequestMock.GetResponse();
            response.GetResponseStream();
            webResponseMock.Close();
            mockRepository.VerifyAll();
        }

        private Stream GetResponseStream(WebRequest request)
        {
            return request.GetResponse().GetResponseStream();
        }
    }
}