using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryBuilderTask.Statements
{
    /// <summary>
    /// Abstract Statement class.
    /// </summary>
    public abstract class Statement
    {
        protected string? tableName;

        protected string? sequencer;

        protected string? tableId;

        protected Dictionary<string, KeyValuePair<string, JToken>>? WhereClauses { get; set; } = null;

        protected Dictionary<string, JToken>? Columns { get; set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Statement"/> class.
        /// </summary>
        /// <param name="tableName">Target table name.</param>
        public Statement(string tableName)
        {
            this.tableName = tableName;
            this.Columns = new ();
        }

        /// <summary>
        /// String representation of statement.
        /// </summary>
        /// <param name="timeZone">TimeZone.</param>
        public abstract string ToString(TimeZoneInfo timeZone);

        /// <summary>
        /// Add column name and value to listo of columns.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="value">Column value.</param>
        public void AddColumn(string name, JToken value)
        {
            if (Columns is null)
                throw new Exception("Cannot add column because Columns property is null!");

            Columns.Add(name, value);
        }

        /// <summary>
        /// Add where clause to list of where clauses.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="sign">Arithmetic sign.</param>
        /// <param name="value">Column value.</param>
        public void Where(string columnName, string sign, JToken value)
        {
            if (WhereClauses is null)
                throw new Exception("Cannot add where clause because WhereClauses property is null!");

            KeyValuePair<string, JToken> pair = new (sign, value);
            WhereClauses.Add(columnName, pair);
        }

        /// <summary>
        /// Create where clause in string format.
        /// </summary>
        /// <param name="timeZone">TimeZone.</param>
        protected string SerializeWhereClauses(TimeZoneInfo timeZone)
        {
            if (WhereClauses is null || WhereClauses.Count == 0)
                return string.Empty;

            StringBuilder whereClause = new ();
            whereClause.Append("WHERE ");

            AppendWhereClauseLiterals(timeZone, whereClause);

            return whereClause.ToString();
        }

        /// <summary>
        /// Append where clause line.
        /// </summary>
        /// <param name="timeZone">TimeZone.</param>
        /// <param name="whereClause">Where clause.</param>
        private void AppendWhereClauseLiterals(TimeZoneInfo timeZone, StringBuilder whereClause)
        {
            if (WhereClauses is null)
                throw new Exception("Cannot serialize where clause to string because WhereClauses property is null!");

            foreach (KeyValuePair<string, KeyValuePair<string, JToken>> primaryKeyLookup in WhereClauses)
            {
                string arithmeticSign = primaryKeyLookup.Value.Key;
                string convertedValue = QueryBuilderHelper.ConvertJTokenToString(primaryKeyLookup.Value.Value, timeZone);
                string whereClauseLiteral = $"{primaryKeyLookup.Key} {arithmeticSign} {convertedValue} AND ";
                whereClause.Append(whereClauseLiteral);
            }

            // Remove last " AND ".
            whereClause.Length -= 5;
        }
    }
}
