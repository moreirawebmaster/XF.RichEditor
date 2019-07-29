using Xamarin.Forms;

namespace Plugin.RichEditor.Droid
{
    public static class RichEditor
    {
        public static void Init()
        {
            DependencyService.Register<RichEditorImplementation>();
        }
    }
}