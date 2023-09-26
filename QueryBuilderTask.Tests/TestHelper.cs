using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryBuilderTask.Tests
{
    public static class TestHelper
    {
        public static string RemoveWhitespace(string str)
        {
            return Regex.Replace(str, @"\s", string.Empty);
        }
    }
}
