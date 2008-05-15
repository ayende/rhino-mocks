#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
            WebRequest request = (WebRequest)mocks.StrictMock(typeof(WebRequest));
            WebResponse response = (WebResponse)mocks.StrictMock(typeof(WebResponse));
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
            IDemo demo = (IDemo)mocks.StrictMock(typeof(IDemo));
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
            webRequestMock = (WebRequest)mockRepository.StrictMock(typeof(WebRequest));
            webResponseMock = (WebResponse)mockRepository.StrictMock(typeof(WebResponse));

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