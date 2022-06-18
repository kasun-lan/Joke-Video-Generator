using System;
using System.Collections.Generic;
using System.Text;

namespace Joke_Animation_Video_Generator
{
    public class Section
    {
        public string Text { get; set; }
        public string AudioFilePath { get; set; }
        public int DelayBetweenWords { get; set; }
        public Dictionary<string, string> keyValuePairs { get; set; }
    }
}
