namespace BlazorPathHelper;

/// <summary>
/// Generated menu item from path definition
/// </summary>
public record BlazorPathMenuItem
{
    /// <summary>
    /// Index in the entire path definition. It is mainly intended to be used for the @key attribute.
    /// </summary>
    public int Index { get; init; }
    
    /// <summary>
    /// Key of the parent group to which it belongs.
    /// </summary>
    public string GroupKey { get; init; } = default!;
    
    /// <summary>
    /// Level of the group. 0 if it is the top level.
    /// </summary>
    public int GroupLevel { get; init; }
    
    /// <summary>
    /// Index in the group.
    /// </summary>
    public int GroupIndex { get; init; }
    
    /// <summary>
    /// Defined path.
    /// </summary>
    public string Path { get; init; } = default!;
    
    /// <summary>
    /// Name for display.
    /// </summary>
    public string Name { get; init; } = default!;
    
    /// <summary>
    /// Description for display.
    /// </summary>
    public string Description { get; init; } = "";
    
    /// <summary>
    /// Icon value.
    /// </summary>
    public object? Icon { get; init; } = null;
    
    /// <summary>
    /// Child items.
    /// </summary>
    public BlazorPathMenuItem[] Children { get; init; } = [];
    
    /// <summary>
    /// Is the root group.
    /// </summary>
    public bool IsRootGroup => Children.Length == 0;
}

