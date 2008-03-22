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
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;

using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class MultiMocks
    {
        #region CanCreateAStrictMultiMockFromTwoInterfaces
        [Test]
        public void CanCreateAStrictMultiMockFromTwoInterfacesNonGeneric()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = (IDemo)mocks.CreateMultiMock(typeof(IDemo), typeof(IDisposable));
            CanCreateAStrictMultiMockFromTwoInterfacesCommon(mocks, demo);
        }

        [Test]
        public void CanCreateAStrictMultiMockFromTwoInterfacesGeneric()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = mocks.CreateMultiMock<IDemo>(typeof(IDisposable));
            CanCreateAStrictMultiMockFromTwoInterfacesCommon(mocks, demo);
        }

        private static void CanCreateAStrictMultiMockFromTwoInterfacesCommon(MockRepository mocks, IDemo demo)
        {
            demo.ReturnIntNoArgs();
            LastCall.Return(1);
            IDisposable disposable = demo as IDisposable;

            Assert.IsNotNull(disposable);
            disposable.Dispose();

            mocks.ReplayAll();

            Assert.AreEqual(1, demo.ReturnIntNoArgs());
            disposable.Dispose();

            mocks.VerifyAll();
        }
        #endregion

        #region ClearStrictCollectionAndDisposesIt
        [Test]
        public void ClearStrictCollectionAndDisposesItNonGeneric()
        {
            MockRepository mocks = new MockRepository();
            CollectionBase collection = (CollectionBase)mocks.CreateMultiMock(typeof(CollectionBase),
                                                                              typeof(IDisposable));
            ClearStrictCollectionAndDisposesItCommon(mocks, collection);
        }

        [Test]
        public void ClearStrictCollectionAndDisposesItGeneric()
        {
            MockRepository mocks = new MockRepository();
            CollectionBase collection = mocks.CreateMultiMock<CollectionBase>(typeof(IDisposable));
            ClearStrictCollectionAndDisposesItCommon(mocks, collection);
        }

        private static void ClearStrictCollectionAndDisposesItCommon(MockRepository mocks, CollectionBase collection)
        {
            collection.Clear();
            ((IDisposable)collection).Dispose();

            mocks.ReplayAll();
            CleanCollection(collection);
            mocks.VerifyAll();
        }

        private static void CleanCollection(CollectionBase collection)
        {
            collection.Clear();
            IDisposable disposable = collection as IDisposable;
            if(disposable!=null)
                disposable.Dispose();
        }
        #endregion

        #region CanCreateAStrictMultiMockFromClassAndTwoInterfacesNonGeneric
        [Test]
        public void CanCreateAStrictMultiMockFromClassAndTwoInterfacesNonGeneric()
        {
            MockRepository mocks = new MockRepository();
            XmlReader reader = (XmlReader)mocks.CreateMultiMock(typeof(XmlReader), typeof(ICloneable), typeof(IHasXmlNode));

            CanCreateAStrictMultiMockFromClassAndTwoInterfacesCommon(mocks, reader);
        }

        [Test]
        public void CanCreateAStrictMultiMockFromClassAndTwoInterfacesGeneric()
        {
            MockRepository mocks = new MockRepository();
            XmlReader reader = mocks.CreateMultiMock<XmlReader>(typeof(ICloneable), typeof(IHasXmlNode));

            CanCreateAStrictMultiMockFromClassAndTwoInterfacesCommon(mocks, reader);
        }

        private static void CanCreateAStrictMultiMockFromClassAndTwoInterfacesCommon(MockRepository mocks, XmlReader reader)
        {
            Expect.Call( reader.AttributeCount ).Return( 3 );

            ICloneable cloneable = reader as ICloneable;
            Assert.IsNotNull( cloneable );

            Expect.Call( cloneable.Clone() ).Return( reader );

            IHasXmlNode hasXmlNode = reader as IHasXmlNode;
            Assert.IsNotNull( hasXmlNode );

            XmlNode node = new XmlDocument();
            Expect.Call( hasXmlNode.GetNode() ).Return( node );

            mocks.ReplayAll();

            Assert.AreEqual( 3, reader.AttributeCount );
            Assert.AreEqual( node, hasXmlNode.GetNode() );

            Assert.AreSame( cloneable, cloneable.Clone() );

            mocks.VerifyAll();
        }
        #endregion

        #region CanCreateAStrictMultiMockWithConstructorArgs
        [Test]
        public void CanCreateAStrictMultiMockWithConstructorArgsNonGeneric()
        {
            MockRepository mocks = new MockRepository();

            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = (IFormatProvider)mocks.CreateMock(typeof(IFormatProvider));

            StringWriter mockedWriter = (StringWriter)mocks.CreateMultiMock(
                typeof(StringWriter),
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );

            CommonConstructorArgsTest(mocks, stringBuilder, formatProvider, mockedWriter, MockType.Strict);
        }

        [Test]
        public void CanCreateAStrictMultiMockWithConstructorArgsGeneric()
        {
            MockRepository mocks = new MockRepository();

            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = mocks.CreateMock<IFormatProvider>();

            StringWriter mockedWriter = mocks.CreateMultiMock<StringWriter>(
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );

            CommonConstructorArgsTest(mocks, stringBuilder, formatProvider, mockedWriter, MockType.Strict);
        }

        #endregion

        #region CanCreateADynamicMultiMockFromTwoInterfacesNonGeneric
        [Test]
        public void CanCreateADynamicMultiMockFromTwoInterfacesNonGeneric()
        {
            MockRepository mocks = new MockRepository();
            object o = mocks.DynamicMultiMock(typeof(IDemo), typeof(IEditableObject));

            IDemo demo = o as IDemo;
            IEditableObject editable = o as IEditableObject;

            CanCreateADynamicMultiMockFromTwoInterfacesCommon(mocks, demo, editable);
        }

        [Test]
        public void CanCreateADynamicMultiMockFromTwoInterfacesGeneric()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = mocks.DynamicMultiMock<IDemo>(typeof(IEditableObject));
            IEditableObject editable = demo as IEditableObject;

            CanCreateADynamicMultiMockFromTwoInterfacesCommon(mocks, demo, editable);
        }

        private static void CanCreateADynamicMultiMockFromTwoInterfacesCommon(MockRepository mocks, IDemo demo, IEditableObject editable)
        {
            Assert.IsNotNull(demo, "IDemo null");
            Assert.IsNotNull(editable, "IEditableObject null");

            // Set expectation on one member on each interface

            Expect.Call(demo.ReadOnly).Return("foo");
            editable.BeginEdit();

            mocks.ReplayAll();

            // Drive two members on each interface to check dynamic nature

            Assert.AreEqual("foo", demo.ReadOnly);
            demo.VoidNoArgs();

            editable.BeginEdit();
            editable.EndEdit();

            mocks.VerifyAll();
        }
        #endregion

        #region CanCreateADynamicMultiMockWithConstructorArgs
        [Test(Description="Tests that we can dynamic multi-mock a class with constructor arguments")]
        public void CanCreateADynamicMultiMockWithConstructorArgsNonGeneric()
        {
            MockRepository mocks = new MockRepository();

            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = (IFormatProvider)mocks.CreateMock(typeof(IFormatProvider));

            StringWriter mockedWriter = (StringWriter)mocks.DynamicMultiMock(
                typeof(StringWriter),
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );
            CommonConstructorArgsTest(mocks, stringBuilder, formatProvider, mockedWriter, MockType.Dynamic);
        }

        [Test(Description = "Tests that we can dynamic generic multi-mock a class with constructor arguments")]
        public void CanCreateADynamicMultiMockWithConstructorArgsGeneric()
        {
            MockRepository mocks = new MockRepository();

            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = mocks.CreateMock<IFormatProvider>();

            StringWriter mockedWriter = mocks.DynamicMultiMock<StringWriter>(
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );
            CommonConstructorArgsTest(mocks, stringBuilder, formatProvider, mockedWriter, MockType.Dynamic);
        }

        #endregion

        #region CanCreateAPartialMultiMockFromClassAndTwoInterfacesNonGeneric
        [Test]
        public void CanCreateAPartialMultiMockFromClassAndTwoInterfacesNonGeneric()
        {
            MockRepository mocks = new MockRepository();
            XmlReader reader = (XmlReader)mocks.PartialMultiMock(typeof(XmlReader), typeof(ICloneable), typeof(IHasXmlNode));

            CanCreateAPartialMultiMockFromClassAndTwoInterfacesCommon(mocks, reader);
        }

        [Test]
        public void CanCreateAPartialMultiMockFromClassAndTwoInterfacesGeneric()
        {
            MockRepository mocks = new MockRepository();
            XmlReader reader = mocks.PartialMultiMock<XmlReader>(typeof(ICloneable), typeof(IHasXmlNode));

            CanCreateAPartialMultiMockFromClassAndTwoInterfacesCommon(mocks, reader);
        }

        private static void CanCreateAPartialMultiMockFromClassAndTwoInterfacesCommon(MockRepository mocks, XmlReader reader)
        {
            Expect.Call(reader.AttributeCount).Return(3);

            ICloneable cloneable = reader as ICloneable;
            Assert.IsNotNull(cloneable);

            Expect.Call(cloneable.Clone()).Return(reader);

            IHasXmlNode hasXmlNode = reader as IHasXmlNode;
            Assert.IsNotNull(hasXmlNode);

            XmlNode node = new XmlDocument();
            Expect.Call(hasXmlNode.GetNode()).Return(node);

            mocks.ReplayAll();

            Assert.AreEqual(3, reader.AttributeCount);
            Assert.AreEqual(node, hasXmlNode.GetNode());

            Assert.AreSame(cloneable, cloneable.Clone());

            mocks.VerifyAll();
        }
        #endregion

        #region CanConstructAPartialMultiMockWithConstructorArgs
        [Test(Description = "Tests that we can partial multi-mock a class with constructor arguments")]
        public void CanCreateAPartialMultiMockWithConstructorArgsNonGeneric()
        {
            MockRepository mocks = new MockRepository();

            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = (IFormatProvider)mocks.CreateMock(typeof(IFormatProvider));

            StringWriter mockedWriter = (StringWriter)mocks.PartialMultiMock(
                typeof(StringWriter),
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );

            CommonConstructorArgsTest(mocks, stringBuilder, formatProvider, mockedWriter, MockType.Partial);
        }

        [Test(Description = "Tests that we can partial generic multi-mock a class with constructor arguments")]
        public void CanCreateAPartialMultiMockWithConstructorArgsGeneric()
        {
            MockRepository mocks = new MockRepository();

            StringBuilder stringBuilder = new StringBuilder();
            IFormatProvider formatProvider = mocks.CreateMock<IFormatProvider>();

            StringWriter mockedWriter = mocks.PartialMultiMock<StringWriter>(
                new Type[] { typeof(IDataErrorInfo) },
                stringBuilder,
                formatProvider
            );
            
            CommonConstructorArgsTest(mocks, stringBuilder, formatProvider, mockedWriter, MockType.Partial);
        }
        #endregion

        #region Check cannot create multi mocks using extra classes
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotMultiMockUsingClassesAsExtras()
        {
            MockRepository mocks = new MockRepository();
            mocks.CreateMultiMock(typeof(XmlReader), typeof(XmlWriter));
        }
        #endregion

        #region RepeatedInterfaceMultiMocks
        public interface IMulti
        {
            void OriginalMethod1();
            void OriginalMethod2();
        }
        public class MultiClass : IMulti
        {
            // NON-virtual method
            public void OriginalMethod1() { }
            // VIRTUAL method
            public virtual void OriginalMethod2() { }
        }
        public interface ISpecialMulti : IMulti
        {
            void ExtraMethod();
        }

        [Test(Description="Tests that MultiMocks can mock class hierarchies where interfaces are repeated")]
        public void RepeatedInterfaceMultiMocks()
        {
            MockRepository mocks = new MockRepository();
            object o = mocks.CreateMultiMock(typeof(MultiClass), typeof(ISpecialMulti));

            Assert.IsTrue(o is MultiClass, "Object should be MultiClass");
            Assert.IsTrue(o is IMulti, "Object should be IMulti");
            Assert.IsTrue(o is ISpecialMulti, "Object should be ISpecialMulti");
        }
        #endregion

        #region CommonConstructorArgsTest
        private enum MockType { Strict, Dynamic, Partial }

        // Helper class to provide a common set of tests for constructor-args based
        // multi-mocks testing.  Exercises a mocked StringWriter (which should also be an IDataErrorInfo)
        // constructed with a mocked IFormatProvider.  The test checks the semantics
        // of the mocked StringWriter to compare it with the expected semantics.
        private static void CommonConstructorArgsTest(MockRepository mocks, StringBuilder stringBuilder, IFormatProvider formatProvider, StringWriter mockedWriter, MockType mockType)
        {
            string stringToWrite = "The original string";
            string stringToWriteLine = "Extra bit";

            IDataErrorInfo errorInfo = mockedWriter as IDataErrorInfo;
            Assert.IsNotNull(errorInfo);

            // Configure expectations for mocked writer
            SetupResult.For(mockedWriter.FormatProvider).CallOriginalMethod(OriginalCallOptions.CreateExpectation);
            mockedWriter.Write((string)null);
						LastCall.IgnoreArguments().CallOriginalMethod(OriginalCallOptions.CreateExpectation);

            mockedWriter.Flush();
						LastCall.Repeat.Any().CallOriginalMethod(OriginalCallOptions.CreateExpectation);

            mockedWriter.Close();

            // Configure expectations for object through interface
            Expect.Call(errorInfo.Error).Return(null);
            Expect.Call(errorInfo.Error).Return("error!!!");

            mocks.ReplayAll();

            // Ensure that arguments arrived okay
            // Is the format provider correct
            Assert.AreSame(formatProvider, mockedWriter.FormatProvider, "FormatProvider");
            // Does writing to the writer forward to our stringbuilder from the constructor?
            mockedWriter.Write(stringToWrite);
            mockedWriter.Flush();

            // Let's see what mode our mock is running in.
            // We have not configured WriteLine at all, so:
            //  a) if we're running as a strict mock, it'll fail
            //  b) if we're running as a dynamic mock, it'll no-op
            //  c) if we're running as a partial mock, it'll work
            try
            {
                mockedWriter.WriteLine(stringToWriteLine);
            }
            catch (ExpectationViolationException)
            {
                // We're operating strictly.
                Assert.AreEqual(MockType.Strict, mockType);
            }

            string expectedStringBuilderContents = null;
            switch (mockType)
            {
                case MockType.Dynamic:
                case MockType.Strict:
                    // The writeline won't have done anything
                    expectedStringBuilderContents = stringToWrite;
                    break;
                case MockType.Partial:
                    // The writeline will have worked
                    expectedStringBuilderContents = stringToWrite + stringToWriteLine + Environment.NewLine;
                    break;
            }

            Assert.AreEqual(expectedStringBuilderContents, stringBuilder.ToString());

            // Satisfy expectations.
            mockedWriter.Close();
            Assert.IsNull(errorInfo.Error, "Error should be null");
            Assert.AreEqual("error!!!", errorInfo.Error, "Should have gotten an error");

        	if(MockType.Strict != mockType)
				mocks.VerifyAll();
        }
        #endregion
    }
}
