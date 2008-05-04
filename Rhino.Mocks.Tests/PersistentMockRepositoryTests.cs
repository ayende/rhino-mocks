#region license

// Copyright (c) 2005 - 2008 Ayende Rahien (ayende@ayende.com)
// Copyright (c) 2008 Karl Lew (karl_lew@intuit.com)
// Copyright (c) 2008 Raja Ramanathan (raja_ramanathan@intuit.com)
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
using System.Reflection;
using System.Runtime.Remoting;

//NOTE: MbUnit may have difficulty running the following tests in the debugger with Resharper
using MbUnit.Framework;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class PersistentMockRepositoryTests : MarshalByRefObject 
	{
		public enum SequencedTestCase
		{
			DeleteCache,
			SaveAssembly,
			CacheLoad,
			CreateUnsavedMock,
			StaleCache,
			SaveAssemblyTwice,
			SaveCachedAssembly,
		}

		public interface IDemoNotSaved
		{
			string Hello { get; }
		}
	
        [SetUp]
        public void SetUp()
        {
		}

		/// <summary>
		/// The tests MUST be run in sequence because assemblies need to 
		/// be loaded/unloaded/etc. The actual tests are declared Explicit so
		/// that they can be run individually in sequence for debuggging.
		/// This method is the "automated" test runner that executes the
		/// Explicit tests in sequence.
		/// </summary>
		[Test]
		public void TestPersistentMockRepository()
		{
			RunTest(SequencedTestCase.DeleteCache);
			RunTest(SequencedTestCase.SaveAssembly);
			RunTest(SequencedTestCase.CacheLoad);
			RunTest(SequencedTestCase.SaveCachedAssembly);
			RunTest(SequencedTestCase.CreateUnsavedMock);
			RunTest(SequencedTestCase.StaleCache);
			RunTest(SequencedTestCase.SaveAssemblyTwice);
		}

		private static void RunTest(SequencedTestCase sequencedTest)
		{
			Assembly thisAssembly = Assembly.GetExecutingAssembly();
			string thisPath = thisAssembly.CodeBase.Replace("file:///", "");

			AppDomainSetup domainSetup = new AppDomainSetup();
			domainSetup.ApplicationBase = Path.GetDirectoryName(thisPath);

			AppDomain appDomain = AppDomain.CreateDomain("TestPersistentMockRepository", null, domainSetup);
			Console.WriteLine("Executing {0} in separate AppDomain in {1}", sequencedTest, appDomain.BaseDirectory);
			ObjectHandle testsHandle = appDomain.CreateInstanceFrom(thisAssembly.CodeBase, "Rhino.Mocks.Tests.PersistentMockRepositoryTests");
			Assert.IsNotNull(testsHandle);
			PersistentMockRepositoryTests mockTests = testsHandle.Unwrap() as PersistentMockRepositoryTests;
			Assert.IsNotNull(mockTests);
			switch (sequencedTest)
			{
				case SequencedTestCase.DeleteCache:
					mockTests.TestDeleteCache();
					break;
				case SequencedTestCase.SaveAssembly:
					mockTests.TestSaveAssembly();
					break;
				case SequencedTestCase.CacheLoad:
					mockTests.TestCacheLoad();
					break;
				case SequencedTestCase.CreateUnsavedMock:
					mockTests.TestCreateUnsavedMock();
					break;
				case SequencedTestCase.StaleCache:
					mockTests.TestStaleCache();
					break;
				case SequencedTestCase.SaveAssemblyTwice:
					mockTests.TestSaveAssemblyTwice();
					break;
			
			}

			AppDomain.Unload(appDomain);
		}

		/// <summary>
		/// This must be the first unit test executed
		/// </summary>
		public void TestDeleteCache()
		{
			//Remove any existing cache
			bool cacheExists = File.Exists(PersistentMockRepository.CachedAssemblyFilePath);
			if (cacheExists) {
				Console.WriteLine("Deleting cached assembly {0}", PersistentMockRepository.CachedAssemblyFilePath);
				File.Delete(PersistentMockRepository.CachedAssemblyFilePath);
				Assert.IsFalse(File.Exists(PersistentMockRepository.CachedAssemblyFilePath));
			} else {
				Console.WriteLine("Assembly is not yet cached at {0}", PersistentMockRepository.CachedAssemblyFilePath);
			}
		}

		/// <summary>
		/// This must be the second unit test executed. It will create the cache
		/// </summary>
		public void TestSaveAssembly()
		{
			PersistentMockRepository persistMocks = PersistentMockRepository.GetInstance;

			Assert.IsFalse(PersistentMockRepository.IsCacheValid(), "IsCacheValid()");
			Assert.IsFalse(persistMocks.IsAssemblyLoadedFromCache, "IsAssemblyLoadedFromCache");
			long msDemoStart = Environment.TickCount;
			IDemo demo = persistMocks.CreateMock(typeof(IDemo)) as IDemo;
			Assert.IsNotNull(demo);
			long msDemoElapsed = Environment.TickCount - msDemoStart;
			Console.WriteLine("Created demo: {0}ms", msDemoElapsed);
			Assert.Less(DEMO_FAST_CREATION_MAX, msDemoElapsed, "cached mock creation should be faster");
			string cachePath = PersistentMockRepository.SaveAssembly();
			Console.WriteLine("Assembly cached to: {0}", cachePath);
			Assert.IsNotNull(cachePath, "Assembly was not saved");
			Assert.AreEqual(cachePath, PersistentMockRepository.CachedAssemblyFilePath);
			Assert.IsTrue(File.Exists(PersistentMockRepository.CachedAssemblyFilePath), PersistentMockRepository.CachedAssemblyFilePath);

			//Since this is a non-cached assembly, there will be a cost to generating the first mock
			Assert.IsTrue(msDemoElapsed > 10);
		}


		/// <summary>
		/// The third unit test will reload the cache
		/// </summary>
		public void TestCacheLoad()
		{
			PersistentMockRepository persistMocks = PersistentMockRepository.GetInstance;
			
			//Reload the MockRepository assembly
			Assert.IsTrue(persistMocks.IsAssemblyLoadedFromCache);
			long msDemo2Start = Environment.TickCount;
			IDemo demo2 = persistMocks.CreateMock(typeof(IDemo)) as IDemo;
			Assert.IsNotNull(demo2);
			long msDemo2Elapsed = Environment.TickCount - msDemo2Start;
			Console.WriteLine("Created demo2: {0}ms", msDemo2Elapsed);
			// We expect the second creation to be faster,
			// but we can't exactly be sure if that's from the cache or from the loaded assembly
			Assert.IsTrue(msDemo2Elapsed < DEMO_FAST_CREATION_MAX);
		}

		/// <summary>
		/// The third unit test will reload the cache, create a new mock, which will
		/// invalidate the cache (and make this test fail if run twice in a row without the
		/// preceding tests)
		/// </summary>
		public void TestCreateUnsavedMock()
		{
			PersistentMockRepository persistMocks = PersistentMockRepository.GetInstance;

			//Reload the MockRepository assembly
			Assert.IsTrue(persistMocks.IsAssemblyLoadedFromCache);

			long msDemo3Start = Environment.TickCount;
			IDemoNotSaved demo3 = persistMocks.CreateMock<IDemoNotSaved>();
			long msDemo3Elapsed = Environment.TickCount - msDemo3Start;
			Assert.IsNotNull(demo3);
			Console.WriteLine("Created unsaved (i.e., NEW mock): {0}ms", msDemo3Elapsed);
			Assert.IsTrue(msDemo3Elapsed > DEMO_FAST_CREATION_MAX);
		}

		public void TestStaleCache()
		{
			Assert.IsTrue(PersistentMockRepository.IsCacheValid(), "IsCacheValid");
			PersistentMockRepository.InvalidateCache();
			PersistentMockRepository persistMocks = PersistentMockRepository.GetInstance;
			Assert.IsFalse(PersistentMockRepository.IsCacheValid(), "IsCacheValid");
			Assert.IsFalse(persistMocks.IsAssemblyLoadedFromCache);
		}

		public void TestCreateUncachedType()
		{
			PersistentMockRepository persistMocks = PersistentMockRepository.GetInstance;
			TestSaveAssembly();

			Exception caughtException = null;
			try {
				// This will throw an exception because PersistentMockRepository is frozen.
				IDemoNotSaved demo3 = persistMocks.CreateMock<IDemoNotSaved>();
			} catch (Exception e) {
				caughtException = e;
			}
			Assert.IsNotNull(caughtException);
		}

		public void TestSaveAssemblyTwice()
		{
			Exception caughtException = null;
			try
			{
				TestSaveAssembly();

				string result = PersistentMockRepository.SaveAssembly();
				Assert.IsNull(result);
			}
			catch (Exception e)
			{
				caughtException = e;
			}
			Assert.IsTrue(caughtException is InvalidOperationException);
		}

		/// <summary>
		/// SaveAssembly shouldn't be called if repository was loaded from the cache.
		/// </summary>
		public void TestSaveCachedAssembly()
		{
			Assert.IsTrue(PersistentMockRepository.IsCacheValid(), "IsCacheValid");
			Exception caughtException = null;
			try {
				string result = PersistentMockRepository.SaveAssembly();
				Assert.IsNull(result);
			} catch (Exception e) {
				caughtException = e;
			}
			Assert.IsTrue(caughtException is InvalidOperationException);
		}

		private static readonly long DEMO_FAST_CREATION_MAX = 40; // cached creation should be fast
	}
}
