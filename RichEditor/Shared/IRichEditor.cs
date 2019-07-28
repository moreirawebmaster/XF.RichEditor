using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.RichEditor.Shared
{
    public interface IRichEditor : IDisposable
    {
        Task<Response> ShowAsync(string html, ToolbarBuilder toolbarBuilder = null, bool autoFocusInput = false, Dictionary<string, string> macros = null);
    }
}