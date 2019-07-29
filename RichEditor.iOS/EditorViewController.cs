using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Plugin.RichEditor.Shared;
using UIKit;

namespace Plugin.RichEditor.iOS
{
    [Register("EditorViewController")]
    public sealed class EditorViewController : UIViewController
    {
        private readonly Shared.RichEditor _richTextEditor;
        private UIWebView _webView;
        private UIScrollView _toolbarScroll;
        private UIToolbar _toolbar;
        private UIView _toolbarHolder;
        private UIBarButtonItem _keyboardItem;
        private List<UIBarButtonItem> _uiToolbarItems;
        private double _keyboardHeight;
        private PopColorPickerViewController _colorPickerViewController;
        private UIPopoverController _popoverController;
        private NSObject _keyboardWillShowToken;
        private NSObject _keyboardWillHideToken;

        public EditorViewController() : base()
        {
            _richTextEditor = new Shared.RichEditor();
            _richTextEditor.SetJavaScriptEvaluatingFunction((input) =>
            {
                _webView.EvaluateJavascript(input);
            });
            _richTextEditor.SetJavaScriptEvaluatingWithResultFunction((input) =>
            {
                return Task.Run(() =>
                {
                    var res = string.Empty;
                    InvokeOnMainThread(() =>
                    {
                        res = _webView.EvaluateJavascript(input);
                    });
                    return res;
                });
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            StyleWebView();
            StyleScrollView();
            StyleToolbar();
            AddColorPickerControl();
            LayoutToolBarHolder();
            LoadResource();

            _richTextEditor.UpdateHtml();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _keyboardWillShowToken = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyboardWillShowOrHide);
            _keyboardWillHideToken = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyboardWillShowOrHide);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardWillShowToken);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardWillHideToken);
        }

        private void StyleWebView()
        {
            _webView = new UIWebView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                Delegate = new EditorViewDelegate(_richTextEditor),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                                   | UIViewAutoresizing.FlexibleHeight
                                   | UIViewAutoresizing.FlexibleTopMargin
                                   | UIViewAutoresizing.FlexibleBottomMargin,
                KeyboardDisplayRequiresUserAction = false,
                ScalesPageToFit = true,
                BackgroundColor = UIColor.White
            };

            _webView.ScrollView.Bounces = false;

            Add(_webView);
            HideFormAccessoryBar.SetHideFormAccessoryBar(true);
        }

        private void StyleScrollView()
        {
            var width = View.Frame.Width - 44;
            if (IsIpad())
                width = View.Frame.Width;
            _toolbarScroll = new UIScrollView
            {
                Frame = new CGRect(0, 0, width, 44),
                BackgroundColor = UIColor.Clear,
                ShowsVerticalScrollIndicator = false
            };
        }

        private void StyleToolbar()
        {
            _toolbar = new UIToolbar(_toolbarScroll.Frame) { BackgroundColor = UIColor.Clear };
            _toolbarScroll.AddSubview(_toolbar);
            _toolbarScroll.AutoresizingMask = _toolbar.AutoresizingMask;
        }

        private void AddColorPickerControl()
        {
            _colorPickerViewController = new PopColorPickerViewController();

            _colorPickerViewController.CancelButton.Clicked += (object sender, EventArgs e) =>
            {
                if (!IsIpad())
                {
                    _colorPickerViewController.NavigationController.PopViewController(true);
                }
                else
                {
                    _popoverController.Dismiss(true);
                }
            };

            _colorPickerViewController.DoneButton.Clicked += (object sender, EventArgs e) =>
            {
                if (!IsIpad())
                {
                    _colorPickerViewController.NavigationController.PopViewController(true);
                }
                else
                {
                    _popoverController.Dismiss(true);
                }
                nfloat r, g, b, a;
                _colorPickerViewController.SelectedColor.GetRGBA(out r, out g, out b, out a);
                _richTextEditor.SetTextColor((int)(r * 255), (int)(g * 255), (int)(b * 255));
            };

            _richTextEditor.LaunchColorPicker = () =>
            {
                if (!IsIpad())
                {
                    NavigationController.PushViewController(_colorPickerViewController, true);
                }
                else
                {
                    var navController = new UINavigationController(_colorPickerViewController);

                    _popoverController = new UIPopoverController(navController);
                    _popoverController.PresentFromRect(_toolbarHolder.Frame, View, UIPopoverArrowDirection.Down, true);
                }
            };
        }

        private void LayoutToolBarHolder()
        {
            var backgroundToolbar = new UIToolbar
            {
                Frame = new CGRect(0, 0, View.Frame.Width, 44),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            _toolbarHolder = new UIView(new CGRect(0, View.Frame.Height, View.Frame.Width, 44))
            {
                AutoresizingMask = _toolbar.AutoresizingMask
            };
            _toolbarHolder.AddSubview(_toolbarScroll);
            _toolbarHolder.InsertSubview(backgroundToolbar, 0);

            if (!IsIpad())
            {
                var toolbarCropper = new UIView(new CGRect(View.Frame.Width - 44, 0, 44, 44))
                {
                    AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin,
                    ClipsToBounds = true
                };

                var keyboardToolbar = new UIToolbar(new CGRect(-7, -1, 44, 44));
                toolbarCropper.AddSubview(keyboardToolbar);

                _keyboardItem = new UIBarButtonItem(UIImage.FromFile("ZSSkeyboard.png"), UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e)
                {
                    View.EndEditing(true);
                });

                keyboardToolbar.Items = new[] { _keyboardItem };

                _toolbarHolder.AddSubview(toolbarCropper);

                var line = new UIView(new CGRect(0, 0, 0.6, 44))
                {
                    BackgroundColor = UIColor.LightGray,
                    Alpha = (nfloat)0.7
                };
                toolbarCropper.AddSubview(line);
            }

            var toolbarWidth = _uiToolbarItems.Count == 0 ? 0.0f : (_uiToolbarItems.Count * 39 - 10);
            _toolbar.Items = _uiToolbarItems.ToArray();
            _toolbar.Frame = new CGRect(0, 0, toolbarWidth, 44);
            _toolbarScroll.ContentSize = new CGSize(_toolbar.Frame.Width, 44);

            View.AddSubview(_toolbarHolder);
        }

        private bool IsIpad()
        {
            return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
        }

        public void BuildToolbar(ToolbarBuilder builder)
        {
            _uiToolbarItems = new List<UIBarButtonItem>();
            foreach (var toolbaritem in builder)
            {
                _uiToolbarItems.Add(new UIBarButtonItem(
                    UIImage.FromFile(toolbaritem.ImagePath),
                    UIBarButtonItemStyle.Plain,
                    delegate { toolbaritem.ClickFunc.Invoke(_richTextEditor); }
                ));
            }
        }

        public void LoadResource()
        {
            var htmlResource = _richTextEditor.LoadResources();
            _webView.LoadHtmlString(htmlResource, new NSUrl("http://www.zedsaid.com"));
        }

        public void SetHtml(string html)
        {
            _richTextEditor.InternalHtml = html;
        }

        public async Task<string> GetHtmlAsync()
        {
            return await _richTextEditor.GetHtmlAsync();
        }

        public void SetAutoFocusInput(bool autoFocusInput)
        {
            _richTextEditor.AutoFocusInput = autoFocusInput;
        }

        public void SetMacrosDicitionary(Dictionary<string, string> macros)
        {
            _richTextEditor.Macros = macros;
        }

        private void KeyboardWillShowOrHide(NSNotification notification)
        {
            var orientation = UIApplication.SharedApplication.StatusBarOrientation;

            var info = notification.UserInfo;
            var duration = ((NSNumber)info.ObjectForKey(UIKeyboard.AnimationDurationUserInfoKey)).FloatValue;
            var curve = ((NSNumber)info.ObjectForKey(UIKeyboard.AnimationCurveUserInfoKey)).Int32Value;
            var keyboardEnd = ((NSValue)info.ObjectForKey(UIKeyboard.FrameEndUserInfoKey)).CGRectValue;

            double sizeOfToolbar = _toolbarHolder.Frame.Height;

            if (keyboardEnd.Height != 0)
                _keyboardHeight = keyboardEnd.Height;

            if (orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight)
            {
                if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0) && keyboardEnd.Width != 0)
                    _keyboardHeight = keyboardEnd.Width;
            }

            var animationOptions = (UIViewAnimationOptions)(curve << 16);

            if (notification.Name == UIKeyboard.WillShowNotification)
            {
                UIView.Animate(duration, 0, animationOptions, () =>
                {
                    var frame = _toolbarHolder.Frame;
                    frame.Y = View.Frame.Height - (nfloat)(_keyboardHeight + sizeOfToolbar);
                    _toolbarHolder.Frame = frame;

                    var editorFrame = _webView.Frame;
                    editorFrame.Height = View.Frame.Height - (nfloat)(_keyboardHeight + sizeOfToolbar);
                    _webView.Frame = editorFrame;
                }, null);
            }
            else
            {
                UIView.Animate(duration, 0, animationOptions, () =>
                {
                    var frame = _toolbarHolder.Frame;
                    frame.Y = View.Frame.Height + (nfloat)_keyboardHeight;
                    _toolbarHolder.Frame = frame;

                    var editorFrame = _webView.Frame;
                    editorFrame.Height = View.Frame.Height;
                    _webView.Frame = editorFrame;
                }, null);
            }
        }
    }

    public class EditorViewDelegate : UIWebViewDelegate
    {
        private readonly Shared.RichEditor _richTextEditor;

        public EditorViewDelegate(Shared.RichEditor richTextEditor)
        {
            _richTextEditor = richTextEditor;
        }

        public override bool ShouldStartLoad(UIWebView webView, NSUrlRequest request,
            UIWebViewNavigationType navigationType)
        {
            return navigationType != UIWebViewNavigationType.LinkClicked;
        }

        public override void LoadingFinished(UIWebView webView)
        {
            _richTextEditor.EditorLoaded = true;
            _richTextEditor.SetPlatformAsIos();
            if (string.IsNullOrEmpty(_richTextEditor.InternalHtml))
                _richTextEditor.InternalHtml = "";
            _richTextEditor.UpdateHtml();

            /*if (_richTextEditor.AutoFocusInput)*/
            _richTextEditor.Focus();
        }
    }
}