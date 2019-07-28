using Xamarin.Forms;

namespace Plugin.RichEditor.iOS
{
    public static class RichEditor
    {
        public static void Init()
        {
            DependencyService.Register<EditorImplementation>();
        }
    }
}