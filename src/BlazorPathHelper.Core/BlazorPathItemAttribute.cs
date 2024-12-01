using System;
using System.Runtime.CompilerServices;

namespace BlazorPathHelper;

/// <summary>
/// Attribute to customize the generated menu items
/// </summary>
/// <param name="name">Display name</param>
/// <param name="description">Display description</param>
/// <param name="namePath">Full path to the display name (auto-generated)</param>
/// <param name="descriptionPath">Full path to the display description (auto-generated)</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class BlazorPathItemAttribute(
    string? name = null,
    string? description = null,
    [CallerArgumentExpression(nameof(name))] string? namePath = null,
    [CallerArgumentExpression(nameof(description))] string? descriptionPath = null
) : Attribute
{
    /// <summary>
    /// Menu item visibility. default: true
    /// </summary>
    public bool Visible { get; set; } = true;
    
    /// <summary>
    /// Menu item display name. default: field/property name
    /// </summary>
    public string? Name { get; set; } = name;
    
    /// <summary>
    /// Menu item display description. default: null
    /// </summary>
    public string? Description { get; set; } = description;
    
    /// <summary>
    /// Menu item Icon-Property. default: null
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Menu item Icon-Property. default: null
    /// </summary>
    public string? CustomIcon { get; internal set; }

    /// <summary>
    /// Menu item group. default: defined URL parent path (e.g. /a/b -> /a)
    /// </summary>
    public string? Group { get; set; }
    
    /// <summary>
    /// Full path to the display name (auto-generated)
    /// </summary>
    public string? NamePath { get; } = namePath;
    
    /// <summary>
    /// Full path to the display description (auto-generated)
    /// </summary>
    public string? DescriptionPath { get; } = descriptionPath;
}

/// <summary>
/// Attribute to customize the generated menu items
/// </summary>
/// <typeparam name="TIcon">Customize Icon Type</typeparam>
/// <param name="name">Display name</param>
/// <param name="description">Display description</param>
/// <param name="namePath">Full path to the display name (auto-generated)</param>
/// <param name="descriptionPath">Full path to the display description (auto-generated)</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class BlazorPathItemAttribute<TIcon>(
    string? name = null,
    string? description = null,
    [CallerArgumentExpression(nameof(name))] string? namePath = null,
    [CallerArgumentExpression(nameof(description))] string? descriptionPath = null
) : BlazorPathItemAttribute(name,description, namePath, descriptionPath)
    where TIcon : new();