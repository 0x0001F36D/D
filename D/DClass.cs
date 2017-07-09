namespace D.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public delegate dynamic DMethod(params dynamic[] args);
    
    public sealed class DClass : DynamicObject, IReadOnlyDictionary<string, object>
    {
        private readonly Type type;

        private IDictionary<string, object> members;

        internal DClass(Type type)
        {
            this.type = type;
            this.members = new Dictionary<string, object>();
        }

        public int Count
            => this.members.Count;

        public IEnumerable<string> Keys
            => this.members.Keys;

        public IEnumerable<object> Values
            => this.members.Values;
        
        public dynamic this[string key]
        {
            get
                => this.members.TryGetValue(key, out var v) ? v : null;
            set
                => this.members[key] = value;
        }

        public static implicit operator DClass(Type type)
            => new DClass(type);

        public bool ContainsKey(string key)
            => this.members.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => this.members.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.members.GetEnumerator();

        public new Type GetType()
            => this.type;

        public dynamic New(params object[] args)
        {
            var obj = this.type.GetConstructor(args.Select(t => t.GetType()).ToArray()).Invoke(args);
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var d = new DClass(type);

            foreach (var field in type.GetFields(flags))
                d[field.Name] = field.GetValue(obj);

            foreach (var method in type.GetMethods(flags).Where(t => !t.DeclaringType.Equals(typeof(object))))
                d[method.Name] = new DMethod(a => (method as MethodBase).Invoke(obj, a));

            return d;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
            => this.members.TryGetValue(binder.Name, out result);

        public bool TryGetValue(string key, out object value)
            => this.members.TryGetValue(key, out value);

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.members[binder.Name] = value;
            return true;
        }
        
    }
    
}