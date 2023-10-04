namespace QueryBuilderTask.Tests;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using QueryBuilderTask.Definitions;
using System;
using System.Xml.Linq;

[TestFixture]
internal class UnitTests
{
    private string[] PrimaryKeys { get; set; } = [ "Id" ];

    private TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    private string Sequence { get; set; } = "SEQ;Id";

    private string NullSequence { get; set; } = "";

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
            Sequence = this.NullSequence,
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
            Sequence = this.NullSequence,
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
            Sequence = this.NullSequence,
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
                                        Active = 1
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
            Sequence = this.NullSequence,
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
                                        SEQ.NEXT_VAL,
                                        'John',
                                        1
                                    );
                                 END;";

        string actualEscaped = TestHelper.RemoveWhitespace(actualResult.Query);

        string expectedEscaped = TestHelper.RemoveWhitespace(expectedResult);

        Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
    }

    [Test]
    public void TestQuerySequenceWithoutChanges()
    {
        string sourceJSONString = GetExampleJSONString("John");
        string targetJSONString = GetExampleJSONString(1, "John");

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
        string actualEscaped = TestHelper.RemoveWhitespace(actualResult.Query);

        Assert.That(actualEscaped, Is.EqualTo(string.Empty));
    }

    [Test]
    public void TestQueryWithManyTransactions()
    {
        var idsForSourceData = new int[] { 1, 2, 3 };
        var idsForTargetData = new int[] { 4, 5, 6 };
        var namesForSourceData = new string[] { "Ann", "Johny", "Emma" };
        var namesForTargetData = new string[] { "Helen", "Jenny", "Joe" };
        string sourceJSONString = GetExampleJSONString(idsForSourceData, namesForSourceData);
        string targetJSONString = GetExampleJSONString(idsForTargetData, namesForTargetData);

        Input input = new Input
        {
            SourceJSONData = sourceJSONString,
            TargetJSONData = targetJSONString,
            PrimaryKeys = this.PrimaryKeys,
            TimeZone = this.TimeZone,
            Sequence = this.NullSequence,
            TargetTableName = this.TableName,
        };

        Options options = new Options
        {
            TransactionalResult = true,
            TransactionSize = 2,
        };

        var actualResult = QueryBuilder.CreateQuery(input, options, default);

        string expectedResult = @$"BEGIN
                                    INSERT INTO {this.TableName} (
                                        Id,
                                        Name,
                                        Active
                                    ) VALUES (
                                        1,
                                        'Ann',
                                        1
                                    );
                                    INSERT INTO {this.TableName} (
                                        Id,
                                        Name,
                                        Active
                                    ) VALUES (
                                        2,
                                        'Johny',
                                        1
                                    );
                                    END;
                                    BEGIN
                                    INSERT INTO {this.TableName} (
                                        Id,
                                        Name,
                                        Active
                                    ) VALUES (
                                        3,
                                        'Emma',
                                        1
                                    );
                                 END;";

        string actualEscaped = TestHelper.RemoveWhitespace(actualResult.Query);

        string expectedEscaped = TestHelper.RemoveWhitespace(expectedResult);

        Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
    }

    [Test]
    public void TestQueryWithManySatements()
    {
        var idsForSourceData = new int[] { 1, 2, 3 };
        var idsForTargetData = new int[] { 1, 2, 4 };
        var namesForSourceData = new string[] { "Ann", "John", "Emma" };
        var namesForTargetData = new string[] { "Ann", "Johny", "Joe" };
        string sourceJSONString = GetExampleJSONString(idsForSourceData, namesForSourceData);
        string targetJSONString = GetExampleJSONString(idsForTargetData, namesForTargetData);

        Input input = new Input
        {
            SourceJSONData = sourceJSONString,
            TargetJSONData = targetJSONString,
            PrimaryKeys = this.PrimaryKeys,
            TimeZone = this.TimeZone,
            Sequence = this.NullSequence,
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
                                    WHERE Id = 2;
                                    INSERT INTO {this.TableName} (
                                        Id,
                                        Name,
                                        Active
                                    ) VALUES (
                                        3,
                                        'Emma',
                                        1
                                    );
                                 END;";

        string actualEscaped = TestHelper.RemoveWhitespace(actualResult.Query);

        string expectedEscaped = TestHelper.RemoveWhitespace(expectedResult);

        Assert.That(actualEscaped, Is.EqualTo(expectedEscaped));
    }

    [Test]
    public void TestQueryWithWitjJArray()
    {
        string sourceJSONString = TestHelper.JsonStringSource;
        string targetJSONString = TestHelper.JsonStringTarget;
        
        Input input = new Input
        {
            SourceJSONData = sourceJSONString,
            TargetJSONData = targetJSONString,
            PrimaryKeys = ["id"],
            TimeZone = this.TimeZone,
            Sequence = this.NullSequence,
            TargetTableName = this.TableName,
        };

        Options options = new Options
        {
            TransactionalResult = true,
            TransactionSize = 2024,
        };

        var actualResult = QueryBuilder.CreateQuery(input, options, default);

        string expectedResult = @$"BEGIN 
                                    UPDATE Table2 SET 
                                    adult = 0,
                                    backdrop_path = '/jllyc0pY7xFYLyjuCEP6pO7hm8t.jpg',
                                    genre_ids = [  80,  35,  53],
                                    original_language = 'en',
                                    original_title = 'Gołł',
                                    overview = 'Grocery store clerk Simon occasionally sells drugs from his cash register at work, so when soap opera actors Adam and Zack come looking for Ecstasy on a quiet Christmas Eve, they are surprised to find Ronna covering his shift. Desperate for money, Ronna decides to become an impromptu drug dealer, unaware that Adam and Zack are secretly working for obsessed narcotics officer Burke.',
                                    popularity = 13.444,
                                    poster_path = '/kP0OOAa4GTZSUPW8fgPbk1OmKEW.jpg',
                                    release_date = '1999-04-09',
                                    title = 'Go',
                                    video = 0,
                                    vote_average = 6.901,
                                    vote_count = 625 
                                    WHERE id = 9430; 
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

    private static string GetExampleJSONString(int[] ids, string[] names)
    {
        JArray array = new JArray();
        int index = 0;
        foreach (int id in ids)
        {
            JObject jsonObject = new ()
            {
                ["Id"] = id,
                ["Name"] = names[index],
                ["Active"] = 1,
            };
            array.Add(jsonObject);
            index++;
        }

        var jsonString = JsonConvert.SerializeObject(array, Formatting.None);

        return jsonString;
    }
}
