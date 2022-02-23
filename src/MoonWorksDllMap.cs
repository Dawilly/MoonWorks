﻿#region License

/* MoonWorks - Game Development Framework
 * Copyright 2021 Evan Hemsley
 */

/* Derived from code by Ethan Lee (Copyright 2009-2021).
 * Released under the Microsoft Public License.
 * See fna.LICENSE for details.
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Xml;
#endregion

namespace MoonWorks
{
	internal static class MoonWorksDllMap
	{
		#region Private Static Variables

		private static Dictionary<string, string> mapDictionary
			= new Dictionary<string, string>();

		#endregion

		#region Private Static Methods

		private static string GetPlatformName()
		{
			if (OperatingSystem.IsWindows())
			{
				return "windows";
			}
			else if (OperatingSystem.IsMacOS())
			{
				return "osx";
			}
			else if (OperatingSystem.IsLinux())
			{
				return "linux";
			}
			else if (OperatingSystem.IsFreeBSD())
			{
				return "freebsd";
			}
			else
			{
				// Maybe this platform statically links?
				return "unknown";
			}
		}

		#endregion

		#region DllImportResolver Callback Method

		private static IntPtr MapAndLoad(
			string libraryName,
			Assembly assembly,
			DllImportSearchPath? dllImportSearchPath
		)
		{
			string mappedName;
			if (!mapDictionary.TryGetValue(libraryName, out mappedName))
			{
				mappedName = libraryName;
			}
			return NativeLibrary.Load(mappedName, assembly, dllImportSearchPath);
		}

		#endregion

		#region Module Initializer

		[ModuleInitializer]
		public static void Init()
		{
			// Get the platform and architecture
			string os = GetPlatformName();
			string cpu = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
			string wordsize = (IntPtr.Size * 8).ToString();

			// Get the path to the assembly
			Assembly assembly = Assembly.GetExecutingAssembly();
			string assemblyPath = "";
			if (assembly.Location != null)
			{
				assemblyPath = Path.GetDirectoryName(assembly.Location);
			}

			// Locate the config file
			string xmlPath = Path.Combine(
				assemblyPath,
				assembly.GetName().Name + ".dll.config"
			);
			if (!File.Exists(xmlPath))
			{
				// Let's hope for the best...
				return;
			}

			// Load the XML
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlPath);

			// The NativeLibrary API cannot remap function names. :(
			if (xmlDoc.GetElementsByTagName("dllentry").Count > 0)
			{
				string msg = "Function remapping is not supported by .NET Core. Ignoring dllentry elements...";
				Console.WriteLine(msg);

				// Log it in the debugger for non-console apps.
				if (Debugger.IsAttached)
				{
					Debug.WriteLine(msg);
				}
			}

			// Parse the XML into a mapping dictionary
			foreach (XmlNode node in xmlDoc.GetElementsByTagName("dllmap"))
			{
				XmlAttribute attribute;

				// Check the OS
				attribute = node.Attributes["os"];
				if (attribute != null)
				{
					bool containsOS = attribute.Value.Contains(os);
					bool invert = attribute.Value.StartsWith("!");
					if ((!containsOS && !invert) || (containsOS && invert))
					{
						continue;
					}
				}

				// Check the CPU
				attribute = node.Attributes["cpu"];
				if (attribute != null)
				{
					bool containsCPU = attribute.Value.Contains(cpu);
					bool invert = attribute.Value.StartsWith("!");
					if ((!containsCPU && !invert) || (containsCPU && invert))
					{
						continue;
					}
				}

				// Check the word size
				attribute = node.Attributes["wordsize"];
				if (attribute != null)
				{
					bool containsWordsize = attribute.Value.Contains(wordsize);
					bool invert = attribute.Value.StartsWith("!");
					if ((!containsWordsize && !invert) || (containsWordsize && invert))
					{
						continue;
					}
				}

				// Check for the existence of 'dll' and 'target' attributes
				XmlAttribute dllAttribute = node.Attributes["dll"];
				XmlAttribute targetAttribute = node.Attributes["target"];
				if (dllAttribute == null || targetAttribute == null)
				{
					continue;
				}

				// Get the actual library names
				string oldLib = dllAttribute.Value;
				string newLib = targetAttribute.Value;
				if (string.IsNullOrWhiteSpace(oldLib) || string.IsNullOrWhiteSpace(newLib))
				{
					continue;
				}

				// Don't allow duplicates
				if (mapDictionary.ContainsKey(oldLib))
				{
					continue;
				}

				mapDictionary.Add(oldLib, newLib);
			}

			// Set the resolver callback for our native assemblies
			NativeLibrary.SetDllImportResolver(typeof(SDL2.SDL).Assembly, MapAndLoad);
			NativeLibrary.SetDllImportResolver(typeof(RefreshCS.Refresh).Assembly, MapAndLoad);
			NativeLibrary.SetDllImportResolver(typeof(FAudio).Assembly, MapAndLoad);
		}

		#endregion
	}
}
