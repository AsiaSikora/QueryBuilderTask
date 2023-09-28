namespace QueryBuilderTask.Tests;

using QueryBuilderTask.Definitions;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Collections.Generic;
using System;

[TestFixture]
internal class UnitTests
{
    private string[] PrimaryKeys { get; set; } = [ "Id" ];

    private TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    private string Sequence { get; set; } = "SEQ.NEXT_VAL";

    private string TableName { get; set; } = "Table2";

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
            TimeZone = this.TimeZone,
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
                                        1
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
            TimeZone = this.TimeZone,
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
                                        Active = 1
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
            TimeZone = this.TimeZone,
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
                                        Active = 1,
                                    WHERE Id = 1;";

        string actualEscaped = TestHelper.RemoveWhitespace(actualResult.Query);

        string expectedEscaped = TestHelper.RemoveWhitespace(expectedResult);

        Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
    }

    [Test]
    public void TestQueryWithEmptyResult()
    {
        string sourceJSONString = GetExampleJSONString(1, "John");
        string targetJSONString = GetExampleJSONString(1, "John");

        Input input = new Input
        {
            SourceJSONData = sourceJSONString,
            TargetJSONData = targetJSONString,
            PrimaryKeys = this.PrimaryKeys,
            TimeZone = this.TimeZone,
            Sequence = this.Sequence,
            TargetTableName = this.TableName,
        };

        Options options = new Options
        {
            TransactionalResult = true,
            TransactionSize = 2024,
        };

        var actualResult = QueryBuilder.CreateQuery(input, options, default);

        Assert.That(actualResult.Query, Is.EqualTo(string.Empty));
    }

    [Test]
    public void TestQuerySequence()
    {
        string sourceJSONString = GetExampleJSONString("John");
        string targetJSONString = GetExampleJSONString(1, "Jane");

        Input input = new Input
        {
            SourceJSONData = sourceJSONString,
            TargetJSONData = targetJSONString,
            PrimaryKeys = ["Name"],
            TimeZone = this.TimeZone,
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
                                        {this.Sequence},
                                        'John',
                                        1
                                    );
                                 END;";

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
            ["Active"] = 1,
        };

        JArray array = new JArray();

        array.Add(jsonObject);

        var jsonString = JsonConvert.SerializeObject(array, Formatting.None);

        return jsonString;
    }

    private static string GetExampleJSONString(string name)
    {
        JObject jsonObject = new()
        {
            ["Name"] = name,
            ["Active"] = 1,
        };

        JArray array = new JArray();

        array.Add(jsonObject);

        var jsonString = JsonConvert.SerializeObject(array, Formatting.None);

        return jsonString;
    }
}
