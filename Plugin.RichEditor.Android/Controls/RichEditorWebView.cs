using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Webkit;
using Java.IO;
using Plugin.RichEditor.Droid.ColorPicker;

namespace Plugin.RichEditor.Droid.Controls
{
    public class RichEditorWebViewClient : WebViewClient
    {
        private readonly Shared.RichEditor _richTextEditor;

        public RichEditorWebViewClient(Shared.RichEditor richTextEditor)
        {
            _richTextEditor = richTextEditor;
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            return request.HasGesture ? base.ShouldInterceptRequest(view, request) : null;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            if (url.Contains("scroll://"))
                return false;
            if (url.Contains("callback://"))
                return true;
            else
                view.LoadUrl(url);
            return true;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            _richTextEditor.EditorLoaded = true;
            _richTextEditor.SetPlatformAsDroid();
            if (string.IsNullOrEmpty(_richTextEditor.InternalHtml))
                _richTextEditor.InternalHtml = "";
            _richTextEditor.UpdateHtml();

            /*if (_richTextEditor.AutoFocusInput)
                _richTextEditor.Focus();*/

            base.OnPageFinished(view, url);
        }
    }

    public class TEditorChromeWebClient : WebChromeClient
    {
        public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
        {
            Log.Info("WebView", consoleMessage.Message());
            return base.OnConsoleMessage(consoleMessage);
        }
    }

    public class JavaScriptResult : Java.Lang.Object, IValueCallback
    {
        private readonly TaskCompletionSource<string> _taskResult = new TaskCompletionSource<string>();

        public void OnReceiveValue(Java.Lang.Object result)
        {
            string msg = "";
            try
            {
                var reader = new JsonReader(new StringReader(result.ToString())) { Lenient = true };

                if (reader.Peek() != JsonToken.Null)
                {
                    if (reader.Peek() == JsonToken.String)
                    {
                        msg = reader.NextString();
                    }
                }
            }
            catch (Exception ex)
            {
                _taskResult.SetException(ex);
            }
            finally
            {
                _taskResult.SetResult(msg);
            }
        }

        public Task<string> GetResultAsync()
        {
            return _taskResult.Task;
        }
    }

    public class RichEditorWebView : WebView
    {
        private Shared.RichEditor _richTextEditor;
        private ColorPickerDialog _colorPickerDialog;

        public Shared.RichEditor RichTextEditor => _richTextEditor;

        public RichEditorWebView(Context context) : base(context)
        {
            Init(context);
        }

        private void Init(Context context)
        {
            _richTextEditor = new Shared.RichEditor();
            SetWebViewClient(new RichEditorWebViewClient(_richTextEditor));
            SetWebChromeClient(new TEditorChromeWebClient());
            _richTextEditor.SetJavaScriptEvaluatingFunction((input) =>
            {
                EvaluateJavascript(input, null);
            });
            _richTextEditor.SetJavaScriptEvaluatingWithResultFunction(async (input) =>
            {
                var result = new JavaScriptResult();

                EvaluateJavascript(input, result);

                var result2 = await result.GetResultAsync();

                return result2;
            });
            _colorPickerDialog = new ColorPickerDialog(context, Color.Red);
            _colorPickerDialog.ColorChanged += (o, args) =>
            {
                _richTextEditor.SetTextColor((int)args.Color.R, (int)args.Color.G, (int)args.Color.B);
            };

            _richTextEditor.LaunchColorPicker = () => { _colorPickerDialog.Show(); };
            LoadResource();
        }

        public RichEditorWebView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context);
        }

        public RichEditorWebView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Init(context);
        }

        public void LoadResource()
        {
            Settings.JavaScriptEnabled = true;
            Settings.AllowUniversalAccessFromFileURLs = true;
            Settings.AllowFileAccessFromFileURLs = true;
            Settings.AllowFileAccess = true;
            Settings.DomStorageEnabled = true;

            string htmlResource = _richTextEditor.LoadResources();
            LoadDataWithBaseURL("https://www.google.com", htmlResource, "text/html", "UTF-8", "");
        }

        public void SetHTML(string html)
        {
            _richTextEditor.InternalHtml = html;
            _richTextEditor.UpdateHtml();
        }

        public void InsertHtml(string html)
        {
            _richTextEditor.InternalHtml += html;
            _richTextEditor.UpdateHtml();
        }

        public async Task<string> GetHtmlAsync()
        {
            return await _richTextEditor.GetHtmlAsync();
        }

        public void SetAutoFocusInput(bool autoFocusInput)
        {
            _richTextEditor.AutoFocusInput = autoFocusInput;
        }
    }
}