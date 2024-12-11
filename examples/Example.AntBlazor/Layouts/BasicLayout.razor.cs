using AntDesign.Extensions.Localization;
using AntDesign.ProLayout;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Net.Http.Json;
using BlazorPathHelper;

namespace Example.AntBlazor.Layouts
{
    public partial class BasicLayout : LayoutComponentBase, IDisposable
    {
        private MenuDataItem[] _menuData;

        [Inject] private ReuseTabsService TabService { get; set; }

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
                Key = item.Index.ToString(),
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