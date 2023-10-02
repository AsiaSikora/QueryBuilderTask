namespace QueryBuilderTask;

using Newtonsoft.Json.Linq;
using QueryBuilderTask.Definitions;
using QueryBuilderTask.Statements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

/// <summary>
/// Main class of the QueryBuilder Task.
/// </summary>
public class QueryBuilder
{
    /// <summary>
    /// This is Task.
    /// [Documentation](link for repo).
    /// </summary>
    /// <param name="input">Input parameters for create query.</param>
    /// <param name="options">Teansaction properties.</param>
    /// <param name="cancellationToken">Cancellation token given by Frends.</param>
    /// <returns>Query string.</returns>
    public static Result CreateQuery([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        var transactions = CreateTransactions(input, options);

        StringBuilder query = new ();

        foreach (Transaction transaction in transactions)
        {
            // Checking cancellation token.
            cancellationToken.ThrowIfCancellationRequested();

            // Create single transaction.
            string transactionStr = transaction.ToString(input.TimeZone, options.TransactionalResult);
            query.AppendLine(transactionStr);
        }

        string queryWithoutWhiteSpace = QueryBuilderHelper.RemoveWhitespace(query.ToString());

        if (queryWithoutWhiteSpace == "BEGINEND;")
        {
            return new Result(String.Empty);
        }

        return new Result(query.ToString());
    }

    /// <summary>
    /// Creating list of transactions.
    /// </summary>
    /// <param name="input">Input parameters for create query.</param>
    /// <param name="options">Teansaction properties.</param>
    private static List<Transaction> CreateTransactions(Input input, Options options)
    {
        #region Initialization of variables
        string tableName = input.TargetTableName;
        JArray sourceJSONData = JArray.Parse(input.SourceJSONData);
        JArray targetJSONData = JArray.Parse(input.TargetJSONData);
        List<string> primaryKeys = new List<string>(input.PrimaryKeys);
        List<Transaction> transactions = new();
        ushort maxTransactionSize = (ushort)options.TransactionSize;
        string sequencer = string.Empty;
        string tableIdentifier = string.Empty;
        #endregion

        // Sequencer and tableIdentifier values assigment.
        if (input.Sequence != string.Empty)
        {
            sequencer = QueryBuilderHelper.SplitString(input.Sequence, ";")[0];
            tableIdentifier = QueryBuilderHelper.SplitString(input.Sequence, ";")[1];
        }

        Transaction transaction = new();

        // Adding statements to transations.
        for (int i = 0; i < sourceJSONData.Count; i++)
        {
            JToken entity = sourceJSONData[i];
            IEnumerable<JToken> matches = FindMatches(entity, targetJSONData, primaryKeys);
            Statement? statement = TryGetStatement(entity, matches, primaryKeys, tableName, sequencer, tableIdentifier);

            if (statement != null)
                transaction.AddStatement(statement);

            if ((transaction.GetStatementCount() % maxTransactionSize == 0) && transaction.GetStatementCount() != 0)
            {
                transactions.Add(transaction);
                transaction = new Transaction();
            }

            if (i == sourceJSONData.Count - 1)
                transactions.Add(transaction);
        }

        return transactions;
    }

    /// <summary>
    /// Creating a list of entities from the target DB that matches the entity from the base DB.
    /// </summary>
    /// <param name="sourceJSONDataEntity">Source DB entity.</param>
    /// <param name="targetJSONData">Target DB entities JArray.</param>
    /// <param name="primaryKeys">List of primary keys.</param>
    private static IEnumerable<JToken> FindMatches(JToken sourceJSONDataEntity, JArray targetJSONData, List<string> primaryKeys)
    {
        IEnumerable<JToken> matches = targetJSONData;

        if (primaryKeys.Count == 0)
            return Enumerable.Empty<JToken>();

        foreach (string identifier in primaryKeys)
        {
            matches = matches.Where(
                x => x[identifier].ToString() == sourceJSONDataEntity[identifier].ToString());
        }

        return matches;
    }

    /// <summary>
    /// Creating query statement.
    /// </summary>
    /// <param name="entity">Source DB entity.</param>
    /// <param name="matches">Maches of target DB to source entity.</param>
    /// <param name="primaryKeys">List of primary keys.</param>
    /// <param name="tableName">Target table name.</param>
    /// <param name="sequencer">Sequencer definition.</param>
    /// <param name="tableId">Identity column for target DB.</param>
    private static Statement? TryGetStatement(JToken entity, IEnumerable<JToken> matches, List<string> primaryKeys, string tableName, string sequencer, string tableId)
    {
        // Create Insert statement.
        if (!matches.Any())
            return new Insert(tableName, entity, sequencer, tableId);

        JToken match = matches.First();

        // Remove of the identifying column for the purpose of comparing entities compatibility when the sequencer is present.
        if (sequencer != string.Empty)
        {
            var jObj = match.ToObject<JObject>();
            jObj.Remove($"{tableId}");
            match = JToken.FromObject(jObj);
        }

        // Create Update statement.
        if (!JToken.DeepEquals(entity, match))
        {
            Update update = new (tableName);
            JObject tokenObject = (JObject)entity;
            foreach (JProperty prop in tokenObject.Properties())
            {
                if (!primaryKeys.Contains(prop.Name))
                    update.AddColumn(prop.Name, prop.Value);
            }

            foreach (string identifier in primaryKeys)
            {
                update.Where(identifier, "=", entity[identifier]);
            }

            return update;
        }

        return null;
    }
}
