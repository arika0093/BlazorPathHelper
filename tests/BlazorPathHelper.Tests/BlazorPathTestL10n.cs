// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
using System;
using Xunit;
using FluentAssertions;

namespace BlazorPathHelper.Tests;

[BlazorPath]
internal partial class DefinitionL10N
{
    [BlazorPathItem(Name = nameof(localize.Sample))]
    public const string SampleUrl = $"/sample/path";
}

public class DefinitionL10NTest
{
    [Fact]
    public void PathBuildableTest()
    {
        DefinitionL10N.Helper.SampleUrl()
            .Should().Be("/sample/path");
    }
}