using System;

namespace BlazorPathHelper;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BlazorPathAttribute : Attribute
{
    public string? Namespace { get; set; } = null;
    public string? ClassName { get; set; } = null;
}

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class BlazorPathItemAttribute : Attribute
{
    public bool Visible { get; set; } = true;
    public bool RootForce { get; set; } = false;
    public string? Name { get; set; } = null;
    public string? Icon { get; set; } = null;
    public string? Group { get; set; } = null;
}


public class BlazorPathMenuItem
{
    public int Index { get; set; }
    public string GroupKey { get; set; } = default!;
    public int GroupLevel { get; set; }
    public int GroupIndex { get; set; }
    public string Name { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string? Icon { get; set; }
    public BlazorPathMenuItem[] Children { get; set; } = [];
    public bool IsGroup => Children.Length > 0;
}