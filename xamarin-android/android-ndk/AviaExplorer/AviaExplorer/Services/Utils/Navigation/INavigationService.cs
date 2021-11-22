using System.Threading.Tasks;
using Xamarin.Forms;

namespace AviaExplorer.Services.Utils.Navigation
{
    public interface INavigationService
    {
        Task<bool> SwitchMainPageAsync<TShell>(TShell shell) where TShell : Page;

        bool DetermineAndSetMainPage();

        bool CheckCurrentPageType<T>() where T : Page;

        Task NavigateToPageAsync(string routeWithParams, bool animated = true);

        Task NavigateToPageAsync(Page page, bool animated = true);

        Task NavigateBackAsync(bool animated = true);

        Task NavigateToRootAsync(bool animated = true);

        Task OpenModalAsync(Page modal, bool animated = true);

        Task CloseModalAsync(bool animated = true);

        bool? CheckCanExit();
    }
}