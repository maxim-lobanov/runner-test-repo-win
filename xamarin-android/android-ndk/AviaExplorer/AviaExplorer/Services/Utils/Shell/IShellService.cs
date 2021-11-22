using System.Threading.Tasks;
using Xamarin.Forms;

namespace AviaExplorer.Services.Utils.Shell
{
    public interface IShellService
    {
        Xamarin.Forms.Shell Current { get; }

        Task<bool> ChangeShellAsync<TShell>(TShell shell) where TShell : Xamarin.Forms.Shell;

        void DetermineAndSetCurrent();

        bool CheckCurrentPageType<T>() where T : Page;

        Task OpenModalAsync(Page modal, bool animated = true);

        Task CloseModalAsync(bool animated = true);

        Task NavigateToPageAsync(string routeWithParams, bool animated = true);

        Task NavigateToPageAsync(Page page, bool animated = true);

        Task NavigateBackAsync(bool animated = true);

        Task NavigateToRootAsync(bool animated = true);

        bool CheckCanExit();
    }
}