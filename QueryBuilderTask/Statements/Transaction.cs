using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilderTask.Statements
{
    public class Transaction
    {
        private List<Statement> Statements { get; } = new ();

        public string ToString(TimeZoneInfo timeZone, bool iSTransactional)
        {
            if (iSTransactional)
            {
                return @$"BEGIN
                        {GetStatementLiterals(timeZone)}
                      END;";
            }

            return $"{GetStatementLiterals(timeZone)}";
        }

        public void AddStatement(Statement statement)
        {
            Statements.Add(statement);
        }

        public int GetStatementCount()
        {
            return Statements.Count;
        }

        private string GetStatementLiterals(TimeZoneInfo timeZone)
        {
            StringBuilder transaction = new ();

            foreach (Statement statement in Statements)
            {
                string literal = statement.ToString(timeZone);
                transaction.AppendLine(literal);
            }

            return transaction.ToString();
        }
    }
}
