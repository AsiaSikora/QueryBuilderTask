namespace QueryBuilderTask.Definitions;

using System;
using System.ComponentModel;

/// <summary>
/// Input class usually contains parameters that are required.
/// </summary>
public class Input
{
    /// <summary>
    /// Incoming data from source DB in string JSON format.
    /// </summary>
    public string SourceJSONData { get; set; }

    /// <summary>
    /// Incoming data from target DB in string JSON format.
    /// </summary>
    public string TargetJSONData { get; set; }

    /// <summary>
    /// Primary keys fot tabels.
    /// </summary>
    public string[] PrimaryKeys { get; set; }

    /// <summary>
    /// Name of target DB Table.
    /// </summary>
    /// <example>ExampleTableName.</example>
    [DefaultValue("ExampleTableName")]
    public string TargetTableName { get; set; }

    /// <summary>
    /// TimeZone.
    /// </summary>
    /// <example>Central European Standard Time.</example>
    [DefaultValue("Central European Standard Time")]
    public TimeZoneInfo TimeZone { get; set; }

    /// <summary>
    /// Custom sequence type.
    /// First positions is name of sequecncer, second is identity column name.
    /// </summary>
    /// <example>SEQ;Id.</example>
    [DefaultValue("SEQ;Id")]
    public string Sequence { get; set; }
}