using System;
using System.Text;
using NUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class UsingComObject
    {
        public interface IMockTest
        {
            Scripting.FileSystemObject GetFileSystemObject();
        }
        
        [Test]
        public void UsingScriptingFileSystem()
        {
            MockRepository mocks = new MockRepository();
            Type fsoType = Type.GetTypeFromProgID("Scripting.FileSystemObject");
            Scripting.FileSystemObject fso = (Scripting.FileSystemObject)Activator.CreateInstance(fsoType);
            IMockTest test = mocks.CreateMock(typeof(IMockTest)) as IMockTest;
            Expect.Call(test.GetFileSystemObject()).Return(fso);
            mocks.ReplayAll();
            Assert.AreSame(fso, test.GetFileSystemObject());
            mocks.VerifyAll();

        }
    }
}
