﻿namespace QueryBuilderTask.Definitions;

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
    /// <example>BEGIN UPDATE Table SET Name = 'John' WHERE Id = 1; END;.</example>
    public string Query { get; private set; }
}
