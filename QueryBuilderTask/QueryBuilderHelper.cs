using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace QueryBuilderTask
{
    /// <summary>
    /// QueryBuilderHelper class.
    /// </summary>
    internal class QueryBuilderHelper
    {
        /// <summary>
        /// Create string value depends on JToken type.
        /// </summary>
        /// <param name="token">JToken value.</param>
        /// <param name="timeZone">TimeZone.</param>
        public static string ConvertJTokenToString(JToken token, TimeZoneInfo timeZone)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    return $"'{token.ToString()}'";
                case JTokenType.Integer:
                    return token.ToString();
                case JTokenType.Float:
                    return token.ToString().Replace(",", ".");
                case JTokenType.Date:
                    return JTokenTypeDateToString(token, timeZone);
                default:
                    string errorMessage = $"Cannot convert JTokenType: {token.Type} to string!";
                    throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// Create string DateTime value depends on provided TimeZone.
        /// </summary>
        /// <param name="token">JToken value.</param>
        /// <param name="timeZone">TimeZone.</param>
        private static string JTokenTypeDateToString(JToken token, TimeZoneInfo timeZone)
        {
            DateTime datetimeValue = (DateTime)token;
            DateTime.SpecifyKind(datetimeValue, DateTimeKind.Unspecified);
            TimeSpan offset = timeZone.GetUtcOffset(datetimeValue);
            DateTimeOffset dto = new (datetimeValue, offset);

            string dateTimeStr = dto.ToString("yyyy-MM-dd\\\"T\\\"HH:mm:ss");
            return $"TO_DATE('{dateTimeStr}', 'YYYY-MM-DD\"T\"HH24:MI:SS')";
        }

        /// <summary>
        /// Remove last sign and new line from StringBuilder.
        /// </summary>
        /// <param name="stringBuilder">String builder.</param>
        public static void RemoveLastSignAndNewLine(StringBuilder stringBuilder)
        {
            int howManySignsToRemove = Environment.NewLine.Length + 1;

            stringBuilder.Remove(stringBuilder.Length - howManySignsToRemove, howManySignsToRemove);
        }

        /// <summary>
        /// Create table of strings splited from string.
        /// </summary>
        /// <param name="stringToSplit">String to be split.</param>
        /// <param name="sign">Sign separates strings in stringToBeSplit.</param>
        public static string[] SplitString(string stringToSplit, string sign)
        {
            string[] words = stringToSplit.Split($"{sign}");

            return words;
        }

        public static string RemoveWhitespace(string str)
        {
            return Regex.Replace(str, @"\s", string.Empty);
        }
    }
}
