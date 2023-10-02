using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryBuilderTask.Statements
{
    /// <summary>
    /// Update class.
    /// </summary>
    public class Update : Statement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Update"/> class.
        /// </summary>
        /// <param name="tableName">Target table name.</param>
        public Update(string tableName)
            : base(tableName)
        {
            this.WhereClauses = new ();
        }

        /// <summary>
        /// String representation of update statement.
        /// </summary>
        /// <param name="timeZone">TimeZone.</param>
        public override string ToString(TimeZoneInfo timeZone)
        {
            if (this.WhereClauses is null || this.WhereClauses.Count == 0)
            {
                string errorMessage =
                    "Cannot serialize update object to string because WhereClauses property is null or contains zero elements!";
                throw new Exception(errorMessage);
            }

            string whereClauses = SerializeWhereClauses(timeZone);
            string columns = SerializeColumns(timeZone);

            return @$"UPDATE {this.tableName} SET
                          {columns} 
                      {whereClauses};";
        }

        /// <summary>
        /// Create string with columns to be updated.
        /// </summary>
        /// <param name="timeZone">TimeZone.</param>
        private string SerializeColumns(TimeZoneInfo timeZone)
        {
            if (this.Columns is null)
                throw new Exception("Cannot serialize update columns because Columns property is null!");

            StringBuilder columns = new ();

            foreach (KeyValuePair<string, JToken> column in this.Columns)
            {
                string convertedValue = QueryBuilderHelper.ConvertJTokenToString(column.Value, timeZone);
                string columnAssign = $"{column.Key} = {convertedValue},";
                columns.AppendLine(columnAssign);
            }

            QueryBuilderHelper.RemoveLastSignAndNewLine(columns);

            return columns.ToString();
        }
    }
}
