using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace D.Core.Inline
{
    interface IStatementNode
    {
        string Statement { get; }
    }

    public class DScript
    {
        public static DScript Body
            => GetBody();
        private DScript()
        {
        }
        private static DScript GetBody()
        {
            if (instance == null)
                lock (locker)
                    if (instance == null)
                        instance = new DScript();
            return instance;
        }
        private static volatile DScript instance;
        private static object locker = new object();
    }

    public sealed class DInlineUsing : IStatementNode
    {
        public DInlineUsing(string @namespace)
            => this.Namespace = @namespace;
        
        public string Namespace { get; private set; }
        public string Statement => $"using {this.Namespace};";
        public override string ToString() 
            => this.Statement;
    }

    public sealed class DInlineClass
    {
        public DInlineClass(string name)
        {

        }
    }
    public sealed class DInlineStruct
    {
        public DInlineStruct(string name)
        {

        }
    }

    public sealed class DInlineMethod //: IStatementNode
    {
        public DInlineMethod(string name,Delegate operation)
        {
            operation.Method.GetMethodBody();
        }
    }
}
