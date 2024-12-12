using System;

namespace BlazorPathHelper;

/// <summary>
/// Attribute to customize the generated builder method
/// </summary>
/// <typeparam name="TPage">Blazor Page Type</typeparam>
/// <typeparam name="TQuery">Query Type</typeparam>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class BlazorPathQueryAttribute<TPage, TQuery> : Attribute;