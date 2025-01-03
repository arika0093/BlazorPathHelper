## Basic Usage

The minimum requirements are as follows:

* Prepare a class with the `#!csharp [BlazorPath]` attribute. This class must be defined with the `#!csharp partial` keyword.
* Define constants of type `#!csharp const string` as members within this class.
* Add the `#!csharp [Query<QueryClass>]` attribute to these members.

During the process of generating URL builder functions, a query-compatible version of the function will be created based on the `#!csharp [Query<QueryClass>]` definition.

```csharp title="WebPaths.cs"
[BlazorPath]
public partial class WebPaths
{
  [Query<QueryRecord>]
  public const string CounterWithQuery = "/counter/query";
}

public record QueryRecord(string query = "hello", int page = 0, bool? opt = null);
```

!!! warning "Caution!"

    The `QueryRecord` class must be written in a `.cs` file. Due to the specifications of the SourceGenerator, it cannot be written in a `.razor` file.

!!! note "Recommendation: Clearly define default parameter values"

    It is recommended to specify default values for each parameter or make them nullable. (You need to consider the case where query parameters are not specified.)

??? abstract "Generated Code"

    ```csharp title="Auto Generated Code"
    // <auto-generated />
    public partial class WebPaths
    {
      public partial class Helper
      {
        public static string CounterWithQuery(QueryRecord __query)
          => string.Format("/counter/query{0}", BuildQuery([
            ToEscapedStrings("query", __query.query),
            ToEscapedStrings("page", __query.page),
            ToEscapedStrings("opt", __query.opt)
          ]));
      }
    }
    ```

## How It Works

For members with the `#!csharp [Query<QueryClass>]` attribute, the following function is generated:

```csharp title="Auto Generated Code"
public static string CounterWithQuery(QueryRecord __query)
  => string.Format("/counter/query{0}", BuildQuery([
    ToEscapedStrings("query", __query.query),
    ToEscapedStrings("page", __query.page),
    ToEscapedStrings("opt", __query.opt)
  ]));
```

As you can see from the generated code above, each property of the `QueryRecord` class is expanded as a query parameter. When generating this function, the BlazorPathHelper performs the following internally:

1. Checks if the `#!csharp [Page<QueryClass>]` attribute is present.
2. If specified, extracts the properties defined in `QueryClass`.
    - Members can also be used, but it is not recommended.
3. Calls the `ToEscapedStrings` function for each property to generate query parameters.
    - For example, in the above case, strings like `#!csharp "query=hello"`, `#!csharp "page=0"`, `#!csharp "opt=true"` are generated.
4. Passes the generated query parameters to the `BuildQuery` function to create the query string.
    - In the above case, a string like `#!csharp "?query=hello&page=0&opt=true"` is generated.
    - If `opt=null`, it becomes `#!csharp "?query=hello&page=0"`, and the `opt` parameter is not output.

!!! tip "Nested properties are not supported"

    As defined above, the class itself is not restored; instead, properties are extracted once. Therefore, nested properties are not currently supported.

## Changing Query Names

When you specify the `#!csharp [Query<QueryRecord>]` attribute, the query name uses the property name of `QueryRecord` as is. If you want to change the query name, add the `#!csharp [QueryName("shortName")]` attribute.

```csharp title="WebPaths.cs"
public record QueryRecord
{
    [QueryName("short")] // or [SupplyParameterFromQuery(Name = "short")]
    public required string SuperLongName { get; set; }
}

[BlazorPath]
public partial class WebPaths
{
    [Query<QueryRecord>]
    public const string QueryTest = "/query-test";
      // -> /query-test?short=hello
}
```

## Supported Types

The following definitions are possible for properties of `QueryClass`.

### Standard Types

| Example Class Definition                      | Example Output Query URL         |
| --------------------------------------------- | -------------------------------- |
| `#!csharp record QueryClass(int val1)`        | `#!csharp "/?val1=5"`            |
| `#!csharp record QueryClass(bool? flg1)`      | `#!csharp "/?flg1=true", "/"`    |

In addition to these, any type that implements `#!csharp ToString()` and can be restored by Blazor should generally be supported.

### Arrays

| Example Class Definition                      | Example Output Query URL                 |
| --------------------------------------------- | ---------------------------------------- |
| `#!csharp record QueryClass(string[] arr)`    | `#!csharp "/?arr=foo&arr=bar&arr=buz"`   |

In addition to `#!csharp string[]`, types like `#!csharp int[]` and `#!csharp bool[]` are also supported. However, `IEnumerable` and `List` are not supported as Blazor does not handle them.