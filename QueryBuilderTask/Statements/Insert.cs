using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilderTask.Statements
{
    public class Insert : Statement
    {
        public Insert(string tableName, JToken token)
        {
            this.tableName = tableName;
            Columns = new();
            JObject tokenObject = (JObject)token;
            foreach (JProperty prop in tokenObject.Properties())
            {
                AddColumn(prop.Name, prop.Value);
            }
        }

        public override string ToString(TimeZoneInfo timeZone)
        {
            string columns = SerializeColumns();
            string values = SerializeValues(timeZone);

            return @$"INSERT INTO {tableName} ({columns}) 
                       VALUES ({values});";
        }

        private string SerializeColumns()
        {
            if (Columns == null)
                throw new Exception("Cannot get update columns because Columns property is null!");

            StringBuilder columnStringBuilder = new();

            foreach (KeyValuePair<string, JToken> column in Columns)
            {
                string columnLiteral = $"{column.Key},";
                columnStringBuilder.AppendLine(columnLiteral);
            }

            QueryBuilderHelper.RemoveLastSignAndNewLine(columnStringBuilder);

            return columnStringBuilder.ToString();
        }

        private string SerializeValues(TimeZoneInfo timeZone)
        {
            if (Columns == null)
                throw new Exception("Cannot get update values because Columns property is null!");

            StringBuilder columnStringBuilder = new();

            foreach (KeyValuePair<string, JToken> column in Columns)
            {
                string convertedValue = QueryBuilderHelper.ConvertJTokenToString(column.Value, timeZone);
                string columnLiteral = $"{convertedValue},";
                columnStringBuilder.AppendLine(columnLiteral);
            }

            QueryBuilderHelper.RemoveLastSignAndNewLine(columnStringBuilder);

            return columnStringBuilder.ToString();
        }
    }
}
