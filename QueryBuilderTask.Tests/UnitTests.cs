namespace QueryBuilderTask.Tests;

using QueryBuilderTask.Definitions;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Collections.Generic;

[TestFixture]
internal class UnitTests
{
    private string[] PrimaryKeys { get; set; } = [ "Id" ];
    private string CultureInfo { get; set; } = "fi-FI";
    private string Sequence { get; set; } = string.Empty;
    private string TableName { get; set; } = "Table2";

    [Test]
    public void Test()
    {
        var input = new Input
        {
            Content = "foobar",
        };

        var options = new Options
        {
            Amount = 3,
            Delimiter = ", ",
        };

        var ret = QueryBuilder.CreateQuery(input, options, default);

        Assert.That(ret.Output, Is.EqualTo("foobar, foobar, foobar"));
    }

    [Test]
    public void TestQueryWithInsert()
    {
        string sourceJSONString = GetExampleJSONString(1, "John");
        string targetJSONString = GetExampleJSONString(2, "Jane");

        Input input = new Input
        {
            SourceJSONData = sourceJSONString,
            TargetJSONData = targetJSONString,
            PrimaryKeys = this.PrimaryKeys,
            CultureInfo = this.CultureInfo,
            Sequence = this.Sequence,
            TargetTableName = this.TableName,
        };

        Options options = new Options
        {
            TransactionalResult = true,
            TransactionSize = 2024,
        };

        var actualResult = QueryBuilder.CreateQuery(input, options, default);

        string expectedResult = @$"BEGIN
                                    INSERT INTO {this.TableName} (
                                        Id,
                                        Name,
                                        Active
                                    ) VALUES (
                                        1,
                                        'John',
                                        'true'
                                    );
                                 END;";

        string actualEscaped = TestHelper.RemoveWhitespace(actualResult.Query);

        string expectedEscaped = TestHelper.RemoveWhitespace(expectedResult);

        Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
    }

    [Test]
    public void TestQueryWithUpdate()
    {
        string sourceJSONString = GetExampleJSONString(1, "John");
        string targetJSONString = GetExampleJSONString(1, "Jane");

        Input input = new Input
        {
            SourceJSONData = sourceJSONString,
            TargetJSONData = targetJSONString,
            PrimaryKeys = this.PrimaryKeys,
            CultureInfo = this.CultureInfo,
            Sequence = this.Sequence,
            TargetTableName = this.TableName,
        };

        Options options = new Options
        {
            TransactionalResult = true,
            TransactionSize = 2024,
        };

        var actualResult = QueryBuilder.CreateQuery(input, options, default);

        string expectedResult = @$"BEGIN
                                    UPDATE {this.TableName} SET 
                                        Name = 'John',
                                        Active = 'true',
                                    WHERE Id = 1;
                                 END;";

        string actualEscaped = TestHelper.RemoveWhitespace(actualResult.Query);

        string expectedEscaped = TestHelper.RemoveWhitespace(expectedResult);

        Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
    }

    [Test]
    public void TestQueryWithoutTransaction()
    {
        string sourceJSONString = GetExampleJSONString(1, "John");
        string targetJSONString = GetExampleJSONString(1, "Jane");

        Input input = new Input
        {
            SourceJSONData = sourceJSONString,
            TargetJSONData = targetJSONString,
            PrimaryKeys = this.PrimaryKeys,
            CultureInfo = this.CultureInfo,
            Sequence = this.Sequence,
            TargetTableName = this.TableName,
        };

        Options options = new Options
        {
            TransactionalResult = false,
            TransactionSize = 2024,
        };

        var actualResult = QueryBuilder.CreateQuery(input, options, default);

        string expectedResult = @$"UPDATE {this.TableName} SET 
                                        Name = 'John',
                                        Active = 'true',
                                    WHERE Id = 1;";

        string actualEscaped = TestHelper.RemoveWhitespace(actualResult.Query);

        string expectedEscaped = TestHelper.RemoveWhitespace(expectedResult);

        Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
    }

    private static string GetExampleJSONString(int id, string name)
    {
        JObject jsonObject = new ()
        {
            ["Id"] = id,
            ["Name"] = name,
            ["Active"] = true,
        };

        var jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.None);

        return jsonString;
    }
}
