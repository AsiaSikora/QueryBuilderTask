using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QueryBuilderTask.Statements
{
    /// <summary>
    /// Transaction class.
    /// </summary>
    public class Transaction
    {
        private CancellationToken cancellationToken;

        private List<Statement> Statements { get; } = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token given by Frends.</param>
        public Transaction(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        /// <summary>
        /// String representation of Transaction.
        /// </summary>
        /// <param name="timeZone">TimeZone.</param>
        /// <param name="isTransactional">Is query transactional.</param>
        public string ToString(TimeZoneInfo timeZone, bool isTransactional)
        {
            if (isTransactional)
                return @$"BEGIN {GetStatements(timeZone)} END;";

            return $"{GetStatements(timeZone)}";
        }

        /// <summary>
        /// Add Statement to list of statement.
        /// </summary>
        /// <param name="statement">Statement to add.</param>
        public void AddStatement(Statement statement)
        {
            this.Statements.Add(statement);
        }

        /// <summary>
        /// Get count of statements.
        /// </summary>
        public int GetStatementCount()
        {
            return this.Statements.Count;
        }

        /// <summary>
        /// Get statements in string format.
        /// </summary>
        /// <param name="timeZone">TimeZone.</param>
        private string GetStatements(TimeZoneInfo timeZone)
        {
            StringBuilder transactionString = new ();

            foreach (Statement statement in this.Statements)
            {
                string statementString = statement.ToString(timeZone);
                transactionString.AppendLine(statementString);
                this.cancellationToken.ThrowIfCancellationRequested();
            }

            return transactionString.ToString();
        }
    }
}
