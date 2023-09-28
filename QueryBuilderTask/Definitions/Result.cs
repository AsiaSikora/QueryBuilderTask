namespace QueryBuilderTask.Definitions;

/// <summary>
/// Result class usually contains properties of the return object.
/// </summary>
public class Result
{
    internal Result(string query)
    {
        this.Query = query;
    }

    /// <summary>
    /// Result query string.
    /// </summary>
    /// <example></example>
    public string Query { get; private set; }
}
