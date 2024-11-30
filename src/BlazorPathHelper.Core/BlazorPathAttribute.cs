using System;
using System.Runtime.CompilerServices;
// ReSharper disable StringIndexOfIsCultureSpecific.1

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
public class BlazorPathItemAttribute(
    string? name = null,
    string? description = null,
    [CallerArgumentExpression(nameof(name))] string? namePath = null,
    [CallerArgumentExpression(nameof(description))] string? descriptionPath = null
) : Attribute
{
    public bool Visible { get; set; } = true;
    public string? Name { get; set; } = name;
    public string? Description { get; set; } = description;
    public string? Icon { get; set; }
    public string? Group { get; set; }
    public string? NamePath { get; set; } = namePath;
    public string? DescriptionPath { get; set; } = descriptionPath;
}


public class BlazorPathMenuItem
{
    public int Index { get; set; }
    public string GroupKey { get; set; } = default!;
    public int GroupLevel { get; set; }
    public int GroupIndex { get; set; }
    public string Path { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public BlazorPathMenuItem[] Children { get; set; } = [];
    public bool HasLocalizeName { get; set; } = false;
    public bool HasLocalizeDescription { get; set; } = false;
    public bool IsGroup => Children.Length > 0;
}