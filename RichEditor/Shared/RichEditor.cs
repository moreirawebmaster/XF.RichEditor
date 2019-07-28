using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Plugin.RichEditor.Shared
{
    public partial class RichEditor
    {
        public string InternalHtml { get; set; }
        public bool EditorLoaded { get; set; }
        public bool FormatHtml { get; set; }
        public bool AutoFocusInput { get; set; }
        public Dictionary<string, string> Macros { get; set; }

        public RichEditor()
        {
            EditorLoaded = false;
            FormatHtml = false;
            InternalHtml = string.Empty;
            AutoFocusInput = false;
            Macros = new Dictionary<string, string>();
        }

        public string LoadResources()
        {
            var assembly = typeof(RichEditor).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("Plugin.RichEditor.Shared.EditorResources.editor.html");
            string htmlData;
            using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                htmlData = reader.ReadToEnd();
            }
            string jsData;
            stream = assembly.GetManifestResourceStream("Plugin.RichEditor.Shared.EditorResources.ZSSRichTextEditor.js");
            using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                jsData = reader.ReadToEnd();
            }

            return htmlData.Replace("<!--editor-->", jsData);
        }
    }
}