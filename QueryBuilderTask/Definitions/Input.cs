namespace QueryBuilderTask.Definitions;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    /// TimeZone for DateTime.
    /// </summary>
    /// <example>Central European Standard Time.</example>
    [DefaultValue("Central European Standard Time")]
    public TimeZoneInfo TimeZone { get; set; }

    /// <summary>
    /// Custom sequence type.
    /// </summary>
    /// <example>SEQ_NEXT_VAL.</example>
    [DefaultValue("SEQ_NEXT_VAL")]
    public string Sequence { get; set; }

    /// <summary>
    /// Something that will be repeated.
    /// </summary>
    /// <example>Some example of the expected value.</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("Lorem ipsum dolor sit amet.")]
    public string Content { get; set; }
}