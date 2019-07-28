using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Xamarin.Forms;

namespace Plugin.RichEditor.Shared
{
    public class CrossRichEditor
    {
        private static readonly Lazy<IRichEditor> RichTextEditorInstance = new Lazy<IRichEditor>(() => DependencyService.Get<IRichEditor>());

        public static string PageTitle { get; set; } = "HTML Editor";
        public static string SaveText { get; set; } = "Save";
        public static string CancelText { get; set; } = "Cancel";

        /// <summary>
        /// Current Text Editor
        /// </summary>
        public static IRichEditor Current
        {
            get
            {
                var ret = RichTextEditorInstance.Value;
                if (ret == null)
                {
                    throw new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }

                return ret;
            }
        }

        /// <summary>
        /// Dispose of everything
        /// </summary>
        public static void Dispose()
        {
            if (RichTextEditorInstance != null && RichTextEditorInstance.IsValueCreated)
            {
                RichTextEditorInstance.Value.Dispose();
            }
        }
    }
}