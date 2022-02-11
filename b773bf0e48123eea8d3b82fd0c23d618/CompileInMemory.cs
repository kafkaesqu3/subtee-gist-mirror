using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Reflection;
namespace InMemoryCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] codeArr = new string[1];
            string code = @"
using System;
namespace HelloWorld
{
public class HelloWorldClass
{
public static void Main()
{
Console.WriteLine(""Hello World!"");
Console.ReadLine();
        }
    }
}";
            codeArr[0] = code;
            compileInMemory(codeArr);
        }
        public static void compileInMemory(string[] code)
        {
            CompilerParameters compilerParameters = new CompilerParameters();
            string currentDirectory = Directory.GetCurrentDirectory();
            compilerParameters.GenerateInMemory = true;
            compilerParameters.TreatWarningsAsErrors = false;
            compilerParameters.GenerateExecutable = false;
            compilerParameters.CompilerOptions = "/optimize";
            string[] value = new string[]
            {
                "System.dll",
           //     "System.Core.dll",
                "mscorlib.dll",
                "System.Management.Automation.dll"
            };
            compilerParameters.ReferencedAssemblies.AddRange(value);
            CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
            CompilerResults compilerResults = cSharpCodeProvider.CompileAssemblyFromSource(compilerParameters, code);
            if (compilerResults.Errors.HasErrors) {
                string text = "Compile error: ";
                foreach (CompilerError compilerError in compilerResults.Errors) {
                    text = text + "\r\n" + compilerError.ToString();
                }
                throw new Exception(text);
            }
            Module module = compilerResults.CompiledAssembly.GetModules()[0];
            Type type = null;
            MethodInfo methodInfo = null;
            if (module != null)
            {
                type = module.GetType("HelloWorld.HelloWorldClass");
            }
            if (type != null) {
                methodInfo = type.GetMethod("Main");
            }
            if(methodInfo != null)
            {
                methodInfo.Invoke(null, null);
            }
        }
    }
}