using System;
using Android.Graphics;

namespace Plugin.RichEditor.Droid.ColorPicker
{
    public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs e);

    public class ColorChangedEventArgs : EventArgs
    {
        public Color Color { get; set; }
    }
}