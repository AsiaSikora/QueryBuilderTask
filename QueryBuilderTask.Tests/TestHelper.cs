using System.Text.RegularExpressions;

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
