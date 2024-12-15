using AntDesign.ProLayout;
using BlazorPathHelper;
using Microsoft.AspNetCore.Components;

namespace Example.AntBlazor.Pro.Layouts
{
    public partial class BasicLayout : LayoutComponentBase, IDisposable
    {
        private MenuDataItem[] _menuData = [];

        [Inject] private ReuseTabsService TabService { get; set; } = default!;

        protected override void OnInitialized()
        {
            _menuData = ConverterMenuDataItem(WebPaths.MenuItem);
        }
        
        private static MenuDataItem[] ConverterMenuDataItem(BlazorPathMenuItem[] items)
        {
            return items.Select(item => new MenuDataItem
            {
                Path = item.Path,
                Name = item.Name,
                Key = item.Key,
                Icon = item.Icon?.ToString(),
                Children = item.Children.Length > 0
                    ? ConverterMenuDataItem(item.Children) : null
            }).ToArray();
        }

        void Reload()
        {
            TabService.ReloadPage();
        }

        public void Dispose()
        {
        }
    }
}