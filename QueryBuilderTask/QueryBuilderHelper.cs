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
    internal class QueryBuilderHelper
    {
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

        private static string JTokenTypeDateToString(JToken token, TimeZoneInfo timeZone)
        {
            DateTime datetimeValue = (DateTime)token;
            DateTime.SpecifyKind(datetimeValue, DateTimeKind.Unspecified);
            TimeSpan offset = timeZone.GetUtcOffset(datetimeValue);
            DateTimeOffset dto = new(datetimeValue, offset);

            string dateTimeStr = dto.ToString("yyyy-MM-dd\\\"T\\\"HH:mm:ss");
            return $"TO_DATE('{dateTimeStr}', 'YYYY-MM-DD\"T\"HH24:MI:SS')";
        }

        public static void RemoveLastSignAndNewLine(StringBuilder columns)
        {
            int howManySignsToRemove = Environment.NewLine.Length + 1;

            columns.Remove(columns.Length - howManySignsToRemove, howManySignsToRemove);
        }
    }
}
