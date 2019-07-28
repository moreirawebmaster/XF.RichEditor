using System;
using Android.App;
using Android.Graphics;
using Android.Util;
using Android.Widget;

namespace Plugin.RichEditor.Droid.Controls
{
    public class LinearLayoutDetectsSoftKeyboard : LinearLayout
    {
        public LinearLayoutDetectsSoftKeyboard(global::Android.Content.Context context) : base(context)
        {
        }

        public LinearLayoutDetectsSoftKeyboard(global::Android.Content.Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public LinearLayoutDetectsSoftKeyboard(global::Android.Content.Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public LinearLayoutDetectsSoftKeyboard(global::Android.Content.Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public Action<bool, int> onKeyboardShown;

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var height = MeasureSpec.GetSize(heightMeasureSpec);
            var activity = this.Context as Activity;
            var rect = new Rect();
            activity.Window.DecorView.GetWindowVisibleDisplayFrame(rect);
            var VisibleHeight = rect.Height();
            var size = new Point();
            activity.WindowManager.DefaultDisplay.GetSize(size);
            var screenHeight = size.Y;
            var diff = screenHeight - VisibleHeight;
            onKeyboardShown?.Invoke((diff > 128) && VisibleHeight != 0, VisibleHeight - (screenHeight - height));
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }
}