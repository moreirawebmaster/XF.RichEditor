using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.RichEditor.iOS;
using Plugin.RichEditor.Shared;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(EditorImplementation))]

namespace Plugin.RichEditor.iOS
{
    public class EditorImplementation : Base, IRichEditor
    {
        public override Task<Response> ShowAsync(string html, ToolbarBuilder toolbarBuilder = null,
            bool autoFocusInput = false, Dictionary<string, string> macros = null)
        {
            var taskRes = new TaskCompletionSource<Response>();
            var tvc = new EditorViewController();
            var builder = toolbarBuilder;
            if (toolbarBuilder == null)
                builder = new ToolbarBuilder().AddAll();
            tvc.BuildToolbar(builder);
            tvc.SetHtml(html);
            tvc.SetAutoFocusInput(autoFocusInput);
            tvc.Title = CrossRichEditor.PageTitle;

            tvc.SetMacrosDicitionary(macros);

            UINavigationController nav = null;
            foreach (var vc in UIApplication.SharedApplication.Windows[0].RootViewController.ChildViewControllers)
            {
                if (vc is UINavigationController controller)
                    nav = controller;
                else
                    nav = new UINavigationController(UIApplication.SharedApplication.Windows[0].RootViewController);
            }

            tvc.NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(CrossRichEditor.CancelText,
                UIBarButtonItemStyle.Plain, (item, args) =>
                {
                    nav?.PopViewController(true);
                    taskRes.SetResult(new Response() { IsSave = false, Html = string.Empty });
                }), true);

            tvc.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(CrossRichEditor.SaveText,
                UIBarButtonItemStyle.Done, async (item, args) =>
                {
                    nav?.PopViewController(true);
                    taskRes.SetResult(new Response() { IsSave = true, Html = await tvc.GetHtmlAsync() });
                }), true);

            nav?.PushViewController(tvc, true);
            return taskRes.Task;
        }
    }
}