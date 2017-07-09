using System;
using System.Linq;
using System.CodeDom.Compiler;

namespace D.Core
{
    using System.Collections.Generic;
    using System.IO;

#if roslyn
    using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
#else
    using Microsoft.CSharp;
#endif

    public sealed class DCompiler
    {
        private readonly CompilerParameters parameters;
        private readonly HashSet<string> referencedAssemblies;
        private readonly CodeDomProvider provider;

        public DCompiler()
        {
            this.provider = new CSharpCodeProvider();

            this.parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
            };

            this.referencedAssemblies = new HashSet<string>(new[]
            {
                "System.dll",
                "System.Core.dll",
                "System.Collections.dll"
            });
        }

        public DCompiler AddReference(string referenceFile)
        {
            this.referencedAssemblies.Add(referenceFile);
            return this;
        }

        public DAssembly Compile(DCode code, TextWriter logger = default(TextWriter))
        {
            this.parameters.ReferencedAssemblies.AddRange(this.referencedAssemblies.ToArray());

            try
            {
                var results = this.provider.CompileAssemblyFromSource(this.parameters, code.ToString());

                if (results.Errors.HasErrors)
                {
                    foreach (CompilerError error in results.Errors)
                        logger?.WriteLine(error.ErrorText);
                    return default(DAssembly);
                }
                else
                    return new DAssembly(results.CompiledAssembly);
            }
            catch (Exception e)
            {
                throw new DException(e.Message);
            }
        }
    }
}