using System.Text;

namespace Xamrealm.Base
{
    public class Emoji
    {
        private readonly int[] codes;

        public Emoji(int[] codes)
        {
            this.codes = codes;
        }

        public Emoji(int code)
        {
            codes = new[] { code };
        }

        public override string ToString()
        {
            if (codes == null)
                return string.Empty;

            var sb = new StringBuilder(codes.Length);

            foreach (var code in codes)
                sb.Append(char.ConvertFromUtf32(code));

            return sb.ToString();
        }
    }
}
