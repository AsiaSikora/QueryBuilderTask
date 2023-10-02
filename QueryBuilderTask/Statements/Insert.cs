using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryBuilderTask.Statements
{
    /// <summary>
    /// Insert class.
    /// </summary>
    public class Insert : Statement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Insert"/> class.
        /// </summary>
        /// <param name="tableName">Target table name.</param>
        /// <param name="entity">Entity to be insert.</param>
        /// <param name="seq">Sequencer.</param>
        /// <param name="tableId">Identity column for target DB.</param>
        public Insert(string tableName, JToken entity, string seq, string tableId) 
            : base(tableName)
        {
            this.sequencer = seq;
            this.tableId = tableId;
            JObject tokenObject = (JObject)entity;

            if (this.sequencer != string.Empty)
            {
                this.sequencer = $"{seq}.NEXT_VAL";
                AddColumn(tableId, this.sequencer);
            }

            foreach (JProperty prop in tokenObject.Properties())
            {
                AddColumn(prop.Name, prop.Value);
            }
        }

        /// <summary>
        /// String representation of insert statement.
        /// </summary>
        /// <param name="timeZone">TimeZone.</param>
        public override string ToString(TimeZoneInfo timeZone)
        {
            string columns = SerializeColumnNames();
            string values = SerializeColumnValues(timeZone);

            return @$"INSERT INTO {this.tableName} ({columns}) 
                       VALUES ({values});";
        }

        /// <summary>
        /// Create string with columns names.
        /// </summary>
        private string SerializeColumnNames()
        {
            if (this.Columns == null)
                throw new Exception("Cannot get update columns because Columns property is null!");

            StringBuilder columnStringBuilder = new();

            foreach (KeyValuePair<string, JToken> column in this.Columns)
            {
                string columnKey = $"{column.Key},";
                columnStringBuilder.AppendLine(columnKey);
            }

            QueryBuilderHelper.RemoveLastSignAndNewLine(columnStringBuilder);

            return columnStringBuilder.ToString();
        }

        /// <summary>
        /// Create string with columns values.
        /// </summary>
        /// <param name="timeZone">TimeZone.</param>
        private string SerializeColumnValues(TimeZoneInfo timeZone)
        {
            if (this.Columns == null)
                throw new Exception("Cannot get update values because Columns property is null!");

            StringBuilder columnStringBuilder = new ();

            foreach (KeyValuePair<string, JToken> column in this.Columns)
            {
                string convertedValue = QueryBuilderHelper.ConvertJTokenToString(column.Value, timeZone);

                // Sequencer is created separately to be put into query withaut apostrophes.
                if (column.Value.ToString() == this.sequencer)
                {
                    convertedValue = column.Value.ToString();
                }

                string columnValue = $"{convertedValue},";
                columnStringBuilder.AppendLine(columnValue);
            }

            QueryBuilderHelper.RemoveLastSignAndNewLine(columnStringBuilder);

            return columnStringBuilder.ToString();
        }
    }
}
