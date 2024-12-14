using BlazorPathHelper;
using Microsoft.AspNetCore.Components;

[BlazorPath]
public partial class WebPaths
{
    // [Item("Menu name"), Page<PageComponent>, Query<QueryClass>]
    // public const string VariableName = "/path";  
    [Item("TopPage"), Page<Home>]
    public const string Index = "/";
    [Item("Sample1a"), Page<Sample>]
    public const string Sample = "/sample";
    [Item("Sample1b"), Page<SampleChild>]
    public const string SampleChild = "/sample/child";
    [Item("Sample2a"), Page<Counter>]
    public const string Counter = "/counter";
    [Item("Sample2b"), Page<Counter2>]
    public const string CounterWithState = "/counter/{count:int}";
    [Item("Sample2c"), Page<Counter3>, Query<QueryRecord>]
    public const string CounterWithQuery = "/counter/query";
}

// e.g. component class
public partial class Home : ComponentBase;
public partial class Sample : ComponentBase;
public partial class SampleChild : ComponentBase;
public partial class Counter : ComponentBase;
public partial class Counter2 : ComponentBase;
public partial class Counter3 : ComponentBase;
public record QueryRecord(string query = "hello", int page = 0, bool? opt = null);
