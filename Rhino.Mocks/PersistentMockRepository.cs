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
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Castle.DynamicProxy;

namespace Rhino.Mocks
{
	/// <summary>
	/// PersistentMockRepository extends MockRepository with the capability
	/// to generate persistent assemblies for mock objects that change rarely.
	/// </summary>
	public class PersistentMockRepository : MockRepository
	{
		/// <summary>
		/// Return the singleton PersistentMockRepository, creating and loading it 
		/// from a previously saved assembly if present.
		/// </summary>
		public static PersistentMockRepository GetInstance
		{
			get
			{
				if (thePersistentMockRepository == null)
				{
					thePersistentMockRepository = new PersistentMockRepository();
				}

				return thePersistentMockRepository;
			}
		}

		/// <summary>
		/// Save the currently compiled assembly for re-use as an assembly with the 
		/// returned path name (e.g., "{some path}/CastleDynProxy2.dll"). This call will
		/// throw InvalidOperationException if:
		/// the assembly has already been saved, or
		/// the assembly has been loaded from cached,	
		/// </summary>
		public static string SaveAssembly()
		{
			return GetInstance.SaveAssemblyCore();
		}

		/// <summary>
		/// Return true if SaveAssembly has been called previously for this instance
		/// of PersistentMockRepository
		/// </summary>
		public static bool IsAssemblySaved
		{
			get
			{
				return isAssemblySaved;
			}
		}

		/// <summary>
		/// Return true if the cached assembly is valid, based on file timestamp
		/// comparison of referenced assemblies listed in PERSISTENT_MOCK_REPOSITORY_CACHE_FILE.
		/// <remarks>The PERSISTENT_MOCK_REPOSITORY_CACHE_FILE allows us to get information about
		/// the cached assembly without having to load it (unloading is complicated)</remarks>
		/// </summary>
		/// <returns></returns>
		public static bool IsCacheValid()
		{
			bool isCacheValid = false;

			if (File.Exists(CachedAssemblyFilePath)) {
				string repositoryCacheFileName = GetCachedFilePath(PERSISTENT_MOCK_REPOSITORY_CACHE_FILE);
				if (File.Exists(repositoryCacheFileName)) {
					try {
						isCacheValid = true;
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.Load(repositoryCacheFileName);
						XmlNodeList assemblyNodes = xmlDoc.GetElementsByTagName(ELEMENT_ASSEMBLY);
						foreach (XmlNode assemblyNode in assemblyNodes) {
							XPathNavigator xpath = assemblyNode.CreateNavigator();
							string refdAssemblyFilePath = xpath.GetAttribute(ATTRIBUTE_FILEPATH, "");
							string timeStampString = xpath.GetAttribute(ATTRIBUTE_FILETIME, "");
							long expectedTime = Convert.ToInt64(timeStampString);
							long actualTime = File.GetLastWriteTime(refdAssemblyFilePath).ToFileTime();

							if (expectedTime != actualTime) {
								isCacheValid = false;
								break;
							}
						}
					} catch (XmlException) {
						isCacheValid = false;
					}
				}
			}

			return isCacheValid;
		}

		/// <summary>
		/// Invalidate the assembly cache so that it can be recreated at the next 
		/// call to the PersistentMockRepository ctor. 
		/// NOTE: 
		/// Since PersistentMockRepository is itself a cached singleton, this call will
		/// only apply once the repository has been unloaded (e.g., the current AppDomain
		/// unloads).
		/// This step is required because the AppDomain will lock the assembly file
		/// until it is unloaded.
		/// <remarks>This call will delete the PersistentMockRepository xml file
		/// associated with the cached assembly. Deleting the xml file instead of
		/// the assembly itself prevents contention for the cached assembly.</remarks>
		/// </summary>
		/// <returns></returns>
		public static void InvalidateCache()
		{
			string mockRepositoryXmlPath = PersistentMockRepositoryXmlPath;
			if (File.Exists(mockRepositoryXmlPath)) {
				File.Delete(mockRepositoryXmlPath);
			}
		}

		/// <summary>
		/// Return true if this assembly was loaded from the location
		/// specified by a previous call to SaveAssembly. A PersistentMockRepository
		/// that has been loaded cannot be extended to create proxies that are not
		/// contained within the loaded assembly (i.e., a loaded PersistentMockRepostiory
		/// is frozen.)
		/// </summary>
		public bool IsAssemblyLoadedFromCache
		{
			get
			{
				return isAssemblyLoadedFromCache;
			}
		}

		/// <summary>
		/// Return file path to be used for cached assembly
		/// </summary>
		/// <returns></returns>
		public static string CachedAssemblyFilePath
		{
			get
			{
				return GetCachedFilePath(ModuleScope.DEFAULT_FILE_NAME);
			}
		}

		/// <summary>
		/// Returns path to cache metadata xml file which is used to determine
		/// timestamps of referenced assemblies in order to invalidate the cache
		/// when they change.
		/// </summary>
		public static string PersistentMockRepositoryXmlPath
		{
			get
			{
				return GetCachedFilePath(PERSISTENT_MOCK_REPOSITORY_CACHE_FILE);
			}
		}

		/// <summary>
		/// Create a new PersistentMockRepository and load the assembly cache if it is valid.
		/// </summary>
		private PersistentMockRepository()
		{
			if (IsCacheValid()) {
				Assembly assembly = Assembly.LoadFile(CachedAssemblyFilePath);
				persistentProxyBuilder.ModuleScope.LoadAssemblyIntoCache(assembly);
				isAssemblyLoadedFromCache = true;
			}
		}

		/// <summary>
		/// Save the currently compiled assembly for re-use as an assembly with the 
		/// returned path name (e.g., "{some path}/CastleDynProxy2.dll"). This call will
		/// throw InvalidOperationException if:
		/// the assembly has already been saved, or
		/// the assembly has been loaded from cached,
		/// </summary>
		private string SaveAssemblyCore()
		{
			if (IsAssemblySaved) {
				throw new InvalidOperationException("Assembly has already been saved");
			}

			if (IsAssemblyLoadedFromCache) {
				throw new InvalidOperationException("Assembly is loaded from cache and cannot be saved. Call InvalidateCache.");
			}

			if (File.Exists(CachedAssemblyFilePath)) {
				File.Delete(CachedAssemblyFilePath);
			}

			string filePath = persistentProxyBuilder.SaveAssembly();

			if (filePath != null) {
				SavePersistentMockRepositoryCache();
				isAssemblySaved = true;
			}

			return filePath;
		}

		/// <summary>
		/// Save referenced assembly file paths and timestamps to PERSISTENT_MOCK_REPOSITORY_CACHE_FILE
		/// as xml.
		/// </summary>
		private static void SavePersistentMockRepositoryCache()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.Encoding = Encoding.UTF8;
	
			string repositoryCacheFileName = GetCachedFilePath(PERSISTENT_MOCK_REPOSITORY_CACHE_FILE);
			XmlWriter xmlWriter = XmlWriter.Create(repositoryCacheFileName, settings);
			Assembly assembly = Assembly.ReflectionOnlyLoadFrom(CachedAssemblyFilePath);
			
			WritePersistentMockRepositoryXml(assembly, xmlWriter);
			
			xmlWriter.Flush();
			xmlWriter.Close();
		}

		private static void WritePersistentMockRepositoryXml(Assembly assembly, XmlWriter xmlWriter)
		{
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();

			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement(ELEMENT_REFERENCES);

			foreach (AssemblyName refdName in referencedAssemblies) {
				Assembly refdAssembly = Assembly.Load(refdName);
				xmlWriter.WriteStartElement(ELEMENT_ASSEMBLY);
				string refdAssemblyFilePath = GetAssemblyPath(refdAssembly);
				xmlWriter.WriteAttributeString(ATTRIBUTE_FILEPATH, refdAssemblyFilePath);
				xmlWriter.WriteAttributeString(ATTRIBUTE_FILETIME, File.GetLastWriteTime(refdAssemblyFilePath).ToFileTime().ToString());
				xmlWriter.WriteEndElement();
			}

			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
		}

		/// <summary>
		/// Return the file path of given file in directory of cached assembly.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private static string GetCachedFilePath(string fileName)
		{
			string thisAssemblyLocation = GetAssemblyPath(typeof(PersistentMockRepository).Assembly);
			string thisAssemblyDirectory = Path.GetDirectoryName(thisAssemblyLocation);
			return thisAssemblyDirectory + Path.DirectorySeparatorChar + fileName;
		}

		/// <summary>
		/// Change assembly CodeBase into something intelligible to the file system.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		private static string GetAssemblyPath(Assembly assembly)
		{
			string thisAssemblyURL = assembly.CodeBase;
			return thisAssemblyURL.Replace("file:///", "");
		}

		/// <summary>
		/// Return Generator based on PersistentProxyBuilder
		/// </summary>
		protected override ProxyGenerator Generator
		{
			get
			{
				if (proxyGenerator == null)
				{
					proxyGenerator = new ProxyGenerator(persistentProxyBuilder);
				}
				return proxyGenerator;
			}
		}


		private static PersistentMockRepository thePersistentMockRepository;
		private static ProxyGenerator proxyGenerator;
		private static readonly string ELEMENT_REFERENCES = "References";
		private static readonly string ELEMENT_ASSEMBLY = "Assembly";
		private static readonly string ATTRIBUTE_FILEPATH = "FilePath";
		private static readonly string ATTRIBUTE_FILETIME = "FileTime";
		private static readonly string PERSISTENT_MOCK_REPOSITORY_CACHE_FILE = "PersistentMockRepository.xml"; 
		private readonly PersistentProxyBuilder persistentProxyBuilder = new PersistentProxyBuilder();
		private static bool isAssemblyLoadedFromCache;
		private static bool isAssemblySaved;
	}
}
