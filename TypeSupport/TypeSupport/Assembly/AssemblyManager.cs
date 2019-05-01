using System;
using System.Reflection;
using System.Reflection.Emit;

namespace TypeSupport.Assembly
{
    /// <summary>
    /// A holding object for all of the objects required for creating a new assembly
    /// </summary>
    public class AssemblyManager
    {
        public AssemblyName Name { get; set; }
        public AssemblyBuilder Assembly { get; set; }
        public ModuleBuilder Module { get; set; }
        public AppDomain Domain { get; set; }
        public AssemblyManager(AssemblyName name, AssemblyBuilder assembly, ModuleBuilder module, AppDomain domain)
        {
            Name = name;
            Assembly = assembly;
            Module = module;
            Domain = domain;
        }
    }
}
