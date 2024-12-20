
// ReSharper disable StringLiteralTypo

namespace BlazorPathHelper.Models;

/// <summary>
/// stored information for razor file.
/// </summary>
internal record ParseRazorStructure
{
    /// <summary>
    /// project directory of razor file.
    /// </summary>
    public required string ProjectDirectory { get; init; }

    /// <summary>
    /// default namespace of project.
    /// </summary>
    public required string DefaultNamespace { get; init; }

    /// <summary>
    /// full path of razor file.
    /// </summary>
    public required string FullPath { get; init; }

    /// <summary>
    /// namespace of razor file.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// class name of razor file.
    /// </summary>
    public required string PageClassName { get; init; }

    /// <summary>
    /// full class name
    /// </summary>
    public string FullClassName => $"{Namespace}.{PageClassName}";
}