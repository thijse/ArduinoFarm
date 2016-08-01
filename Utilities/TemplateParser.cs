using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utilities
{
    public class Template
    {
        private readonly Dictionary<string, string> _parameters;
        public Template()
        {
            _parameters = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            _parameters.Add(key, value);
        }

        public void Clear()
        {
            _parameters.Clear();
        }

        public string ParseTemplate(string template)
        {
            if (template == null) return "";        
            var parsedTemplate = Regex.Replace(template, @"\{(.+?)\}", m => _parameters[m.Groups[1].Value]);
            return parsedTemplate;
        }

        public static List<string> FindParameters(string template)
        {
            var matches = Regex.Matches(template, @"\{(.+?)\}");            
            return (from Match match in matches select match.Groups[1].Value).ToList();
        }



    }
}
