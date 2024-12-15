using Microsoft.AspNetCore.Components;
namespace BlazorPathHelper.Demo.Pages;

public partial class WithQuery : ComponentBase
{
    public record QueryParameters()
    {
        [SupplyParameterFromQuery(Name = "rs")]
        public required string RequiredString { get; set; }
        [SupplyParameterFromQuery(Name = "os")]
        public string? OptionalString { get; set; } = null;
        [SupplyParameterFromQuery(Name = "n")]
        public int Number { get; set; } = 0;
    }
}