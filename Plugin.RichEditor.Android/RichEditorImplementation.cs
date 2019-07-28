using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Android.Content;
using Plugin.RichEditor.Droid;
using Plugin.RichEditor.Droid.Controls;
using Plugin.RichEditor.Shared;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace Plugin.RichEditor.Droid
{
    public class RichEditorImplementation : Base, IRichEditor
    {
        public static ToolbarBuilder ToolbarBuilder = null;

        public override Task<Response> ShowAsync(string html, ToolbarBuilder toolbarBuilder = null, bool autoFocusInput = false, Dictionary<string, string> macros = null)
        {
            var result = new TaskCompletionSource<Response>();

            var tActivity = new Intent(Application.Context, typeof(RichEditorActivity));
            ToolbarBuilder = toolbarBuilder ?? new ToolbarBuilder().AddAll();
            tActivity.PutExtra("HTMLString", html);
            tActivity.PutExtra("AutoFocusInput", autoFocusInput);

            if (macros != null)
            {
                tActivity.PutStringArrayListExtra("macroKeys", macros.Keys.ToList());
                tActivity.PutStringArrayListExtra("macroValues", macros.Values.ToList());
            }

            tActivity.SetFlags(ActivityFlags.NewTask);
            RichEditorActivity.SetOutput = (res, resStr) =>
            {
                RichEditorActivity.SetOutput = null;
                result.SetResult(res
                    ? new Response() { IsSave = true, Html = resStr }
                    : new Response() { IsSave = false, Html = string.Empty });
            };
            Application.Context.StartActivity(tActivity);
            return result.Task;
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ToolbarBuilder = null;
        }
    }
}