using System;

namespace BlazorPathHelper;

/// <summary>
/// Attribute to customize the generated builder method
/// </summary>
/// <typeparam name="TQuery">Query Type</typeparam>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class BlazorPathQueryAttribute<TQuery> : Attribute;