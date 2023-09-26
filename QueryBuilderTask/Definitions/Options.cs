namespace QueryBuilderTask.Definitions;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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


    /// <summary>
    /// Number of times input is repeated.
    /// </summary>
    /// <example>Example of the parameter value.</example>
    [DefaultValue(3)]
    public int Amount { get; set; }

    /// <summary>
    /// How repeats of the input are separated.
    /// </summary>
    /// <example>Example of the parameter value.</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue(" ")]
    public string Delimiter { get; set; }
}