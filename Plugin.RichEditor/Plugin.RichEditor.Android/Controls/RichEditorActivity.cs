using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Plugin.RichEditor.Shared;

namespace Plugin.RichEditor.Droid.Controls
{
    [Activity(Label = "Rich Editor",
        WindowSoftInputMode = SoftInput.AdjustResize | SoftInput.StateVisible,
        Theme = "@style/Theme.AppCompat.NoActionBar.FullScreen")]
    public class RichEditorActivity : AppCompatActivity
    {
        private const int ToolbarFixHeight = 60;
        private RichEditorWebView _richEditorWebView;
        private LinearLayoutDetectsSoftKeyboard _rootLayout;
        private LinearLayout _toolbarLayout;
        private global::Android.Support.V7.Widget.Toolbar _topToolBar;

        private IList<string> _macros;
        private IList<string> _macrosValues;

        public static Action<bool, string> SetOutput { get; set; }

        private static readonly Android.Graphics.Color _keysColor = Android.Graphics.Color.ParseColor("#FAFAFA");

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RichEditorActivity);

            _topToolBar = FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.TopToolbar);

            _topToolBar.Title = CrossRichEditor.PageTitle;

            SetSupportActionBar(_topToolBar);

            if (SupportActionBar != null)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);
                SupportActionBar.SetDisplayShowCustomEnabled(true);
            }

            _rootLayout = FindViewById<LinearLayoutDetectsSoftKeyboard>(Resource.Id.RootRelativeLayout);
            _richEditorWebView = FindViewById<RichEditorWebView>(Resource.Id.EditorWebView);
            _toolbarLayout = FindViewById<LinearLayout>(Resource.Id.ToolbarLayout);

            _rootLayout.onKeyboardShown += HandleSoftKeyboardShwon;
            _richEditorWebView.SetOnCreateContextMenuListener(this);

            BuildToolbar();

            var htmlString = Intent.GetStringExtra("HTMLString") ?? "<p></p>";
            _richEditorWebView.SetHTML(htmlString);

            var autoFocusInput = Intent.GetBooleanExtra("AutoFocusInput", false);
            _richEditorWebView.SetAutoFocusInput(autoFocusInput);

            _macros = Intent.GetStringArrayListExtra("macroKeys");
            _macrosValues = Intent.GetStringArrayListExtra("macroValues");
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.TopToolbarMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.TitleFormatted?.ToString() == "Save")
            {
                var action = _richEditorWebView.GetHtmlAsync();

                action.ContinueWith(x =>
                {
                    var html = x.Result;
                    SetOutput?.Invoke(true, html);

                    Finish();
                });
            }
            else if (item.TitleFormatted?.ToString() == "Macros")
            {
                PromptMacros();
            }

            return base.OnOptionsItemSelected(item);
        }

        private void PromptMacros()
        {
            var builderSingle = new Android.App.AlertDialog.Builder(this);

            builderSingle.SetTitle("Select Macro");

            var negative = new EventHandler<DialogClickEventArgs>(
                (s, args) =>
                {
                });

            var positive = new EventHandler<DialogClickEventArgs>(
                async (s, args) =>
                {
                    if (_macrosValues != null && _macrosValues.Count > args.Which)
                    {
                        var value = _macrosValues[args.Which];
                        var currentHtml = await _richEditorWebView.GetHtmlAsync();
                        _richEditorWebView.SetHTML(currentHtml + value + "<br/>");
                    }
                });

            builderSingle.SetItems(_macros?.ToArray(), positive);
            builderSingle.SetNegativeButton("Cancel", negative);

            var adialog = builderSingle.Create();

            adialog.Show();
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            SetOutput?.Invoke(false, null);
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _rootLayout.onKeyboardShown -= HandleSoftKeyboardShwon;
        }

        public void BuildToolbar()
        {
            var builder = RichEditorImplementation.ToolbarBuilder ?? new ToolbarBuilder().AddAll();

            foreach (var item in builder)
            {
                var imagebutton = new ImageButton(this);
                imagebutton.SetBackgroundColor(_keysColor);
                imagebutton.Click += (sender, e) =>
                {
                    item.ClickFunc.Invoke(_richEditorWebView.RichTextEditor);
                };
                var imagename = item.ImagePath.Split('.')[0];
                var resourceId = (int)typeof(Resource.Drawable).GetField(imagename).GetValue(null);
                imagebutton.SetImageResource(resourceId);
                var toolbarItems = FindViewById<LinearLayout>(Resource.Id.ToolbarItemsLayout);
                toolbarItems.AddView(imagebutton);
            }
        }

        public void HandleSoftKeyboardShwon(bool shown, int newHeight)
        {
            if (shown)
            {
                _toolbarLayout.Visibility = ViewStates.Visible;
                var widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                var heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                _toolbarLayout.Measure(widthSpec, heightSpec);
                var toolbarHeight = _toolbarLayout.MeasuredHeight == 0 ? (int)(ToolbarFixHeight * Resources.DisplayMetrics.Density) : _toolbarLayout.MeasuredHeight;
                var topToolbarHeight = _topToolBar.MeasuredHeight == 0 ? (int)(ToolbarFixHeight * Resources.DisplayMetrics.Density) : _topToolBar.MeasuredHeight;
                var editorHeight = newHeight - toolbarHeight - topToolbarHeight;

                _richEditorWebView.LayoutParameters.Height = editorHeight;
                _richEditorWebView.LayoutParameters.Width = ViewGroup.LayoutParams.MatchParent;
                _richEditorWebView.RequestLayout();
            }
            else
            {
                if (newHeight != 0)
                {
                    _toolbarLayout.Visibility = ViewStates.Invisible;
                    _richEditorWebView.LayoutParameters = new LinearLayout.LayoutParams(-1, -1);
                }
            }
        }
    }
}