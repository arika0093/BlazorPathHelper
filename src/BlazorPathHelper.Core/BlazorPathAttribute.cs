using System;

// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace BlazorPathHelper;

/// <summary>
/// Attribute to be attached to classes to be automatically generated
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BlazorPathAttribute : Attribute
{
    /// <summary>
    /// export namespace. default: project root namespace
    /// </summary>
    public string? Namespace { get; set; } = null;
    
    /// <summary>
    /// export class name. default: same as defined class name
    /// </summary>
    public string? ClassName { get; set; } = null;

    /// <summary>
    /// Page template for inheritance
    /// </summary>
    public Type? PageTemplate { get; set; } = null;
}
