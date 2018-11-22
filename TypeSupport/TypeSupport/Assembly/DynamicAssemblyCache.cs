﻿using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace TypeSupport.Assembly
{
    /// <summary>
	/// Cache for storing dynamically build assemblies
	/// </summary>
	public static class DynamicAssemblyCache
    {
        internal static ConcurrentDictionary<string, AssemblyManager> Assemblies = new ConcurrentDictionary<string, AssemblyManager>();

        /// <summary>
        /// Get an assembly from the assembly cache. If it does not exist, a new one will be created for you.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static ModuleBuilder Get(string assemblyName)
        {
            AssemblyManager manager = null;

            if (Assemblies.ContainsKey(assemblyName))
            {
                // grab the assembly from the cache
                manager = Assemblies[assemblyName];
            }
            else
            {
                // Create the new assembly
                try
                {
                    // Create the assembly and cache it
                    var name = new AssemblyName(assemblyName);
                    var domain = System.Threading.Thread.GetDomain();
                    var assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                    var module = assembly.DefineDynamicModule(name.Name);
                    var assemblyManager = new AssemblyManager(name, assembly, module, domain);

                    // add the assembly to the cache
                    Assemblies.TryAdd(assemblyName, assemblyManager);
                    manager = assemblyManager;
                }
                catch (ApplicationException ex)
                {
                    throw ex;
                }
            }
            return manager.Module;
        }
    }
}
