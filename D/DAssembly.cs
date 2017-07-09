namespace D.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public sealed class DAssembly : Module, IEnumerable
    {
        private readonly Assembly assembly;
        private readonly List<DClass> classes;

        internal DAssembly(Assembly a)
        {
            this.assembly = a;
            this.classes = new List<DClass>(a.GetTypes().Select(d => this.@static(d)));
        }

        public override Assembly Assembly
                    => this.assembly;

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public IEnumerator<dynamic> GetEnumerator()
            => this.classes.Cast<dynamic>().GetEnumerator();

        public dynamic New(string name,params object[] args)
            => this.classes.FirstOrDefault(x => x.GetType().Name.Equals(name)).New(args);

        public dynamic Static(string name)
            => this.classes.FirstOrDefault(x => x.GetType().Name.Equals(name));

        private DClass @static(Type type)
        {
            var flags = BindingFlags.Public | BindingFlags.Static;
            var d = new DClass(type);

            foreach (var field in type.GetFields(flags))
                d[field.Name] = field.GetValue(type);
                //d[field.Name] = field.GetValue(null);

            foreach (var method in type.GetMethods(flags))
                d[method.Name] = new DMethod(a => (method as MethodBase).Invoke(type, a));
                //d[method.Name] = new DMethod(a => (method as MethodBase).Invoke(null, a));

            return d;
        }

    }
}