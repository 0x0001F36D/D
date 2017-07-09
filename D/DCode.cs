namespace D.Core
{
    using System.Text;

    public sealed class DCode
    {
        private StringBuilder builder;

        private DCode(StringBuilder code)
            => this.builder = code;
        
        public static explicit operator DCode(string code)
            => new DCode(new StringBuilder(code));
        
        public static DCode operator +(DCode code, string s)
        {
            code.builder.AppendLine(s);
            return code;
        }

        public static DCode operator +(DCode code, DCode code2)
        {
            code.builder.AppendLine(code.ToString());
            return code;
        }

        public override string ToString() => this.builder.ToString();
    }
}