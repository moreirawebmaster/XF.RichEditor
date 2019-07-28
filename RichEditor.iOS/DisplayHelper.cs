using UIKit;

namespace Plugin.RichEditor.iOS
{
    public static class DisplayHelper
    {
        public static bool UserInterfaceIdiomIsPhone => UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone;

        public static bool Is4InchDisplay() => UIScreen.MainScreen.Bounds.Size.Height >= 568f;
    }
}