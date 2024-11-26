using System;

namespace BlazorPathHelper;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BlazorPathAttribute : Attribute
{
}

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class BlazorPathItemAttribute : Attribute
{
    public bool Visible { get; set; } = true;
    public bool IsRoot { get; set; } = false;
    public string? Name { get; set; } = null;
    public string? Icon { get; set; } = null;
    public string? Group { get; set; } = "";
}


public class BlazorPathMenuItem
{
    public string Name { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string? Icon { get; set; }
    public BlazorPathMenuItem[] Children { get; set; } = [];
    public bool IsGroup => Children.Length > 0;
}