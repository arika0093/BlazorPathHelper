using System;

namespace BlazorPathHelper;

/// <summary>
/// Attribute to automatically generate a partial class with a Route/Parameter/Query
/// </summary>
/// <typeparam name="TPage">Blazor Page Type</typeparam>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class PageAttribute<TPage> : Attribute;