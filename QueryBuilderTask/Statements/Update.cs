using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilderTask.Statements
{
    public class Update : Statement
    {
        public Update(string tableName)
        {
            this.tableName = tableName;
            WhereClauses = new();
            Columns = new();
        }

        public override string ToString(TimeZoneInfo timeZone)
        {
            if (WhereClauses is null || WhereClauses.Count == 0)
            {
                string errorMessage =
                    "Cannot serialize update object to string because WhereClauses property is null or contains zero elements!";
                throw new Exception(errorMessage);
            }

            string primaryKeyLookups = SerializeWhereClauses(timeZone);
            string columns = SerializeColumns(timeZone);

            return @$"UPDATE {tableName} SET
                          {columns} 
                      {primaryKeyLookups};";
        }

        private string SerializeColumns(TimeZoneInfo timeZone)
        {
            if (Columns is null)
                throw new Exception("Cannot serialize update columns because Columns property is null!");

            StringBuilder columns = new();

            // It's ok to have a null exception.
            // This is already handled in constructor
            foreach (KeyValuePair<string, JToken> column in Columns)
            {
                string convertedValue = QueryBuilderHelper.ConvertJTokenToString(column.Value, timeZone);
                string columnLiteral = $"{column.Key} = {convertedValue},";
                columns.AppendLine(columnLiteral);
            }

            QueryBuilderHelper.RemoveLastSignAndNewLine(columns);

            return columns.ToString();
        }
    }
}
