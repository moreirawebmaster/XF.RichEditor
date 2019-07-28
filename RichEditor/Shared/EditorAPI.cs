using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Plugin.RichEditor.Shared
{
    public partial class RichEditor : IRichEditorApi
    {
        private bool _platformIsDroid;
        private Func<string, Task<string>> _javaScriptEvaluatFuncWithResult;
        private Action<string> _javaScriptEvaluatFunc;
        public Action LaunchColorPicker { get; set; }

        public void SetJavaScriptEvaluatingFunction(Action<string> function)
        {
            _javaScriptEvaluatFunc = function ?? throw new ArgumentNullException("Function cannot be null");
        }

        public void SetJavaScriptEvaluatingWithResultFunction(Func<string, Task<string>> function)
        {
            _javaScriptEvaluatFuncWithResult = function ?? throw new ArgumentNullException("Function cannot be null");
        }

        public void UpdateHtml()
        {
            var html = InternalHtml;
            var cleanedHTML = RemoveQuotesFromHTML(html);
            var trigger = $"zss_editor.setHTML(\"{cleanedHTML}\");";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public async Task<string> GetHtmlAsync()
        {
            var trigger = "zss_editor.getHTML();";
            var html = await _javaScriptEvaluatFuncWithResult(trigger);
            return html;
        }

        private string RemoveQuotesFromHTML(string html)
        {
            html = html.Replace("\"", "\\\"");
            html = html.Replace("“", "&quot;");
            html = html.Replace("”", "&quot;");
            html = html.Replace("\r", "\\r");
            html = html.Replace("\n", "\\n");
            return html;
        }

        private async Task<string> TidyHtmlAsync(string html)
        {
            html = html.Replace("<br>", "<br />");
            html = html.Replace("<hr>", "<hr />");
            if (FormatHtml)
                html = await _javaScriptEvaluatFuncWithResult.Invoke($"style_html(\"{html}\");");
            return html;
        }

        public void InsertHtml(string html)
        {
            var cleanedHTML = RemoveQuotesFromHTML(html);
            var trigger = $"zss_editor.insertHTML(\"{cleanedHTML}\");";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void Focus()
        {
            var trigger = @"zss_editor.focusEditor();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void RemoveFormat()
        {
            var trigger = @"zss_editor.removeFormating();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void AlignLeft()
        {
            var trigger = @"zss_editor.setJustifyLeft();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void AlignCenter()
        {
            var trigger = @"zss_editor.setJustifyCenter();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void AlignRight()
        {
            var trigger = @"zss_editor.setJustifyRight();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void AlignFull()
        {
            var trigger = @"zss_editor.setJustifyFull();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetBold()
        {
            var trigger = @"zss_editor.setBold();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetItalic()
        {
            var trigger = @"zss_editor.setItalic();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetSubscript()
        {
            var trigger = @"zss_editor.setSubscript();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetUnderline()
        {
            var trigger = @"zss_editor.setUnderline();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetSuperscript()
        {
            var trigger = @"zss_editor.setSuperscript();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetStrikethrough()
        {
            var trigger = @"zss_editor.setStrikeThrough();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetUnorderedList()
        {
            var trigger = @"zss_editor.setUnorderedList();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetOrderedList()
        {
            var trigger = @"zss_editor.setOrderedList();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetHr()
        {
            var trigger = @"zss_editor.setHorizontalRule();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetIndent()
        {
            var trigger = @"zss_editor.setIndent();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetOutdent()
        {
            var trigger = @"zss_editor.setOutdent();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void Heading1()
        {
            var trigger = @"zss_editor.setHeading('h1');";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void Heading2()
        {
            var trigger = @"zss_editor.setHeading('h2');";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void Heading3()
        {
            var trigger = @"zss_editor.setHeading('h3');";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void Heading4()
        {
            var trigger = @"zss_editor.setHeading('h4');";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void Heading5()
        {
            var trigger = @"zss_editor.setHeading('h5');";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void Heading6()
        {
            var trigger = @"zss_editor.setHeading('h6');";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void Paragraph()
        {
            var trigger = @"zss_editor.setParagraph();";
            _javaScriptEvaluatFunc.Invoke(trigger);
        }

        public void SetPlatformAsIos() => _javaScriptEvaluatFunc.Invoke(@"zss_editor.setPlatformAsIOS();");

        public void SetPlatformAsDroid()
        {
            _javaScriptEvaluatFunc.Invoke(@"zss_editor.setPlatformAsDroid();");
            _platformIsDroid = true;
        }

        public void QuickLink() => _javaScriptEvaluatFunc.Invoke(@"zss_editor.quickLink();");

        public void Redo() => _javaScriptEvaluatFunc.Invoke(@"zss_editor.redo();");

        public void SetStrikeThrough()
            => _javaScriptEvaluatFunc.Invoke(@"zss_editor.setStrikeThrough();");

        public void Undo() => _javaScriptEvaluatFunc.Invoke(@"zss_editor.undo();");

        public void SetFooterHeight(double height) =>
            _javaScriptEvaluatFunc.Invoke($"zss_editor.setFooterHeight(\"{height:F}\");");

        public void SetContentHeight(double height) =>
            _javaScriptEvaluatFunc.Invoke($"zss_editor.contentHeight = {height:F};");

        public void PrepareInsert() => _javaScriptEvaluatFunc.Invoke("zss_editor.prepareInsert();");

        public void SetTextColor(int r, int g, int b) =>
            _javaScriptEvaluatFunc.Invoke($"zss_editor.setTextColor(\"#{r:x2}{g:x2}{b:x2}\");");
    }
}