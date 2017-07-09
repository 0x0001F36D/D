using D.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Collections;
using System.IO;

namespace D.Test
{
    

    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Main(args.AsEnumerable()).Wait();
            Console.ReadKey();
        }
        static async Task Main(IEnumerable<string> args)
        {
            TestCompiler();
        }

        static void GetInfo<T>(T obj)
        {
            var t = typeof(T);
            foreach (var f in t.GetFields())
            {
                Console.WriteLine("\t" + f.Name + ": " + f.GetValue(obj));
            }
            foreach (var p in t.GetProperties())
            {
                Console.WriteLine("\t" + p.Name + ": " + p.GetValue(obj));
            }
            Console.WriteLine();
        }
        class ILs: IEnumerable<OpCode>
        {
            private readonly List<OpCode> opcodes;
            public ILs()
            {
                this.opcodes = 
                    typeof(OpCodes)
                    .GetFields()
                    .Select(type => type.GetValue(null))
                    .Cast<OpCode>()
                    .ToList();
            }
            /*
            private string GetInfo(OpCode op,char spilter)
            {
                return string.Join(spilter.ToString(), new object[] 
                {
                    op.Value.ToString(),
                    op.Name,
                    op.OpCodeType,
                    op.OperandType,
                    op.StackBehaviourPop,
                    op.StackBehaviourPush,
                    op.Size.ToString(),
                    op.FlowControl,
                    string.Join(" ", BitConverter.GetBytes(op.Value).Reverse().Select(x=>x.ToString("X").PadLeft(2,'0')))
                });
            }
            private string GetHeader(char spilter)
            {
                var op = OpCodes.Nop;
                return string.Join(spilter.ToString(), new object[]
                {

                    nameof(op.Value),
                    nameof(op.Name),
                    nameof(op.OpCodeType),
                    nameof(op.OperandType),
                    nameof(op.StackBehaviourPop),
                    nameof(op.StackBehaviourPush),
                    nameof(op.Size),
                    nameof(op.FlowControl),
                    "Format"
                });
            }
            public string ToCSV(char spliter = ',')
                => this.Aggregate(new StringBuilder().AppendLine(this.GetHeader(spliter)), (sb, op) => sb.AppendLine(this.GetInfo(op, spliter))).ToString();
            */
            
            public IEnumerator<OpCode> GetEnumerator() => ((IEnumerable<OpCode>)this.opcodes).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<OpCode>)this.opcodes).GetEnumerator();
        }

        static void TestCompiler()
        {
            var code = 
                (DCode)
                "   using System;" +
                "	public class Test " +
                "	{" +
                "       private static int scounter = 0;" +
                "       private int counter =0;" +
                "		public Test()" +
                "		{" +
                "			Console.WriteLine(\"Constructor: Test\");" +
                "		}" +
                "		public void Test1()" +
                "		{" +
                "			Console.WriteLine(\"Method: Test1 \"+counter++);" +
                "		}" +
                "		public void Test2(int i)" +
                "		{" +
                "			Console.WriteLine(\"Method: Test2 \"+i);" +
                "		}" +
                "		public static void StaticTest1()" +
                "		{" +
                "			Console.WriteLine(\"Static Method: StaticTest1 \"+scounter++);" +
                "		}" +
                "		public static void StaticTest2(int i)" +
                "		{" +
                "			Console.WriteLine(\"Static Method: StaticTest2 \"+i);" +
                "		}" +
                "		public static int StaticTest3(int i)" +
                "		{" +
                "			Console.WriteLine(\"Static Method: StaticTest2 \"+i);" +
                "			return i+2;" +
                "		}" +
                "   }"+
                "   class DynamicTest"+
                "   {" +
                "       public Test GetTest()" +
                "       {" +
                "           return new Test();" +
                "       }" +
                "       public DynamicTest(string s)" +
                "       {" +
                "           Console.WriteLine(s);" +
                "       }" +
                "   }"
                
                ;

            var compiler = new DCompiler();

            var asm = compiler.Compile(code, Console.Out);

            var t = asm.New("DynamicTest","Hello").GetTest();
            Console.WriteLine(t.GetType());
            t.Test1();

        }
    }
}