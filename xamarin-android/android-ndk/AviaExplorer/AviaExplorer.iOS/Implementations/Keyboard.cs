using AviaExplorer.Services.Interfaces;
using UIKit;

namespace AviaExplorer.iOS.Implementations
{
    public class Keyboard : IKeyboard
    {
        public void HideKeyboard() =>
            UIApplication.SharedApplication.KeyWindow.EndEditing(true);
    }
}