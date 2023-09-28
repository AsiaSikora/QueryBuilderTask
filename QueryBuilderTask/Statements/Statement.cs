﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilderTask.Statements
{
    public abstract class Statement
    {
        protected string? tableName;

        protected Dictionary<string, KeyValuePair<string, JToken>>? WhereClauses { get; set; } = null;

        protected Dictionary<string, JToken>? Columns { get; set; } = null;

        public abstract string ToString(TimeZoneInfo timeZone);

        public void AddColumn(string name, JToken value)
        {
            if (Columns is null)
                throw new Exception("Cannot add column because Columns property is null!");

            Columns.Add(name, value);
        }

        public void Where(string columnName, string arithmeticSign, JToken value)
        {
            if (WhereClauses is null)
                throw new Exception("Cannot add where clause because WhereClauses property is null!");

            KeyValuePair<string, JToken> pair = new(arithmeticSign, value);
            WhereClauses.Add(columnName, pair);
        }

        protected string SerializeWhereClauses(TimeZoneInfo timeZone)
        {
            if (WhereClauses is null || WhereClauses.Count == 0)
                return string.Empty;

            StringBuilder whereClauseLiterals = new();
            whereClauseLiterals.Append("WHERE ");

            AppendWhereClauseLiterals(timeZone, whereClauseLiterals);

            return whereClauseLiterals.ToString();
        }

        private void AppendWhereClauseLiterals(TimeZoneInfo timeZone, StringBuilder whereClauseLiterals)
        {
            if (WhereClauses is null)
                throw new Exception("Cannot serialize where clause to string because WhereClauses property is null!");

            foreach (KeyValuePair<string, KeyValuePair<string, JToken>> primaryKeyLookup in WhereClauses)
            {
                string arithmeticSign = primaryKeyLookup.Value.Key;
                string convertedValue = QueryBuilderHelper.ConvertJTokenToString(primaryKeyLookup.Value.Value, timeZone);
                string whereClauseLiteral = $"{primaryKeyLookup.Key} {arithmeticSign} {convertedValue} AND ";
                whereClauseLiterals.Append(whereClauseLiteral);
            }

            whereClauseLiterals.Length -= 5; // remove last " AND "
        }

    }
}
