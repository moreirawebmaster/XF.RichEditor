using System;
using System.Threading.Tasks;

namespace Plugin.RichEditor.Shared
{
    public interface IRichEditorApi
    {
        void AlignCenter();

        void AlignFull();

        void AlignLeft();

        void AlignRight();

        Task<string> GetHtmlAsync();

        void Heading1();

        void Heading2();

        void Heading3();

        void Heading4();

        void Heading5();

        void Heading6();

        void InsertHtml(string html);

        void Paragraph();

        void QuickLink();

        void RemoveFormat();

        void Redo();

        void SetBold();

        void SetHr();

        void SetIndent();

        void SetItalic();

        void SetOrderedList();

        void SetOutdent();

        void SetStrikeThrough();

        void SetSubscript();

        void SetSuperscript();

        void SetUnderline();

        void SetUnorderedList();

        void SetPlatformAsIos();

        void SetPlatformAsDroid();

        void UpdateHtml();

        void Undo();

        void SetFooterHeight(double height);

        void SetContentHeight(double height);

        Action LaunchColorPicker { get; set; }

        void PrepareInsert();

        void SetTextColor(int r, int g, int b);
    }
}