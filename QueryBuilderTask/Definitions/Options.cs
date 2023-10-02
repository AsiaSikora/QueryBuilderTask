namespace QueryBuilderTask.Definitions;

using System.ComponentModel;

/// <summary>
/// Options class usually contains parameters that are required.
/// </summary>
public class Options
{
    /// <summary>
    /// Should be the result a transactional.
    /// </summary>
    /// /// <example>True.</example>
    [DefaultValue(true)]
    public bool TransactionalResult { get; set; }

    /// <summary>
    /// Limit of the transaction statements.
    /// </summary>
    /// /// <example>2048.</example>
    [DefaultValue(2048)]
    public int TransactionSize { get; set; }
}