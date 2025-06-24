#pragma warning disable CS9113
using System;

namespace BlazorPathHelper;

/// <summary>
/// Attribute to customize the generated builder method
/// </summary>
/// <typeparam name="TQuery">Query Type</typeparam>
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property,
    Inherited = false,
    AllowMultiple = false
)]
public class QueryAttribute<TQuery> : Attribute;

/// <summary>
/// Attribute to specify the display name of the query value
/// </summary>
/// <param name="name">name of the query value</param>
public class QueryNameAttribute(string name) : Attribute;
