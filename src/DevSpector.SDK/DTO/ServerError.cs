using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DevSpector.SDK.DTO
{
    public class ServerError
    {
        public ServerError(string error, List<string> description)
        {
            Error = error;
            Description = description;
        }

        public string Error { get; }

        public List<string> Description { get; }

        public string GetCommaSeparatedDescription()
        {
            if (Description == null) return "";
            if (Description.Count == 0) return "";

            var builder = new StringBuilder();

            for (int i = 0; i < Description.Count; i++)
                if (i < Description.Count - 1)
                    builder.Append(Description[i] + ", ");
                else
                    builder.Append(Description[i]);

            return builder.ToString();
        }
    }
}
