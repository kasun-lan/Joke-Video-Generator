using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms
{
    public class Section
    {
        public string Text { get; set; }
        public string AudioFilePath { get; set; }
        public int DelayBetweenWords { get; set; }
        public Dictionary<string, string> keyValuePairs { get; set; }
    }
}
