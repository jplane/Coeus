# Coeus - JQ-compatible JSON Queries in .NET

| | |
| --- | --- |
| Coeus NUGET Package | [![NuGet](https://img.shields.io/nuget/v/Coeus)](https://www.nuget.org/packages/Coeus/) |
| | |

Simply put, Coeus implements [JQ](https://stedolan.github.io/jq/)-compatible queries for [JSON.NET](https://www.newtonsoft.com/json). Query and update JToken, JObject, JArray, JValue and friends.

JQ is more powerful than JSON Path/Patch and is a widely-adopted pseudo-standard from the Linux command line.


## Goals

- Simple extension methods for System.String to query against JToken

- No CLI (yet)

- 100% JQ compatibility may not be realistic, though we'll strive to include most operators and functions

## Usage

```csharp

var json = new JObject
{
    ["foo"] = new JArray { 1, 2, 3 }
};

var query = "{ \"bar\": .foo[2] }";

var output = query.EvalToToken(json);

// output --> { 'bar': 3 }

Console.WriteLine(output["bar"].Value<int>());  // 3

```

## Background and Resources

- The JQ [manual](https://stedolan.github.io/jq/manual/) is a wealth of information about query syntax and behavior

- Coeus is implemented using the excellent [Sprache](https://github.com/sprache/Sprache) parser combinator library

- More on parser combinators [here](https://en.wikipedia.org/wiki/Parser_combinator). Your head will hurt, until it doesn't :-)
