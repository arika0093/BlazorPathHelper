// ReSharper disable StringLiteralTypo

using System.Collections.Generic;

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
    /// list of page paths that this razor file be defined.
    /// </summary>
    public required List<string> PagePaths { get; init; } = [];

    /// <summary>
    /// full class name
    /// </summary>
    public string FullClassName => $"{Namespace}.{PageClassName}";

    /// <summary>
    /// full class name without default namespace.
    /// </summary>
    /// <remarks>
    /// e.g.
    /// * default namespace: "Example.FluentUI"
    /// * full class name: "Example.FluentUI.Foo.Bar.Test"
    /// => "Foo.Bar.Test"
    /// </remarks>
    public string PartialClassName => FullClassName.Replace(DefaultNamespace, "").Trim('.');
}
