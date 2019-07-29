using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.RichEditor.Shared
{
    public abstract class Base : IRichEditor
    {
        private bool _disposed;

        public abstract Task<Response> ShowAsync(string html, ToolbarBuilder toolbarBuilder = null, bool autoFocusInput = false, Dictionary<string, string> macros = null);

        /// <summary>
        /// Dispose of class and parent classes
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose up
        /// </summary>
        ~Base()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //dispose only
                }

                _disposed = true;
            }
        }
    }
}