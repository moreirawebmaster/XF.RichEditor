using System.Collections.Generic;
using CoreAnimation;
using UIKit;
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;

namespace Plugin.RichEditor.iOS
{
    public class ColorPickerStandardViewController : UIViewController
    {
        private readonly List<UIColor> _colors;

        public ColorPickerStandardViewController()
            : base()
        {
            _colors = new List<UIColor>();
            var colorCount = 20;

            for (var i = 0; i < colorCount; i++)
            {
                var color = UIColor.FromHSBA(i / (float)colorCount, 1f, 1f, 1f);
                _colors.Add(color);
            }

            colorCount = 8;

            for (var i = 0; i < colorCount; i++)
            {
                var color = UIColor.FromWhiteAlpha(i / (float)(colorCount - 1), 1f);
                _colors.Add(color);
            }
        }

        private UIView _paletteView;
        private CALayer _previousLayer;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.View.BackgroundColor = UIColor.Clear;

            _paletteView = new UIView(new RectangleF(0, 10f, 320f, this.View.Frame.Height - 80f));
            _paletteView.BackgroundColor = UIColor.Clear;

            this.View.AddSubview(_paletteView);

            var layer = new CALayer
            {
                Frame = new RectangleF(130f, 16f, 100f, 40f),
                CornerRadius = 6f,
                ShadowColor = UIColor.Black.CGColor,
                ShadowOffset = new SizeF(0f, 2f),
                ShadowOpacity = 0.8f
            };

            _paletteView.Layer.AddSublayer(layer);

            var colorCount = 28;

            for (var i = 0; i < colorCount && i < _colors.Count; i++)
            {
                layer = new CALayer { Name = $"Color_{i}", CornerRadius = 6f, BackgroundColor = _colors[i].CGColor };

                var column = i % 4;
                var row = i / 4;
                layer.Frame = new RectangleF((float)(8 + (column * 78)), (float)(8 + row * 48), 70f, 40f);
                LayerHelper.SetupShadow(layer);
                _paletteView.Layer.AddSublayer(layer);
            }

            var colorRecognizer = new UITapGestureRecognizer(ColorGridTapped);
            _paletteView.AddGestureRecognizer(colorRecognizer);
        }

        public void ColorGridTapped(UITapGestureRecognizer recognizer)
        {
            var point = recognizer.LocationInView(_paletteView);
            var touchedLayer = _paletteView.Layer.PresentationLayer.HitTest(_paletteView.ConvertPointToView(point, _paletteView.Superview));

            if (touchedLayer != null && !string.IsNullOrWhiteSpace(touchedLayer.Name) && touchedLayer.Name.IndexOf("Color") == 0)
            {
                var actualLayer = touchedLayer.ModelLayer;

                if (_previousLayer == null)
                {
                    actualLayer.BorderWidth = 3f;
                    actualLayer.BorderColor = UIColor.White.CGColor;

                    _previousLayer = actualLayer;
                }
                else
                {
                    if (actualLayer.Name.Equals(_previousLayer.Name) == false)
                    {
                        _previousLayer.BorderWidth = 0f;
                        _previousLayer.BorderColor = UIColor.Clear.CGColor;

                        actualLayer.BorderWidth = 3f;
                        actualLayer.BorderColor = UIColor.White.CGColor;

                        _previousLayer = actualLayer;
                    }
                }
            }

            if (_previousLayer != null)
            {
                var temp = _previousLayer.Name.Split(new char[] { '_' });
                var index = int.Parse(temp[1]);

                if (index < _colors.Count)
                {
                    var parent = this.ParentViewController as PopColorPickerViewController;
                    parent.SelectedColor = _colors[index];
                }
            }
        }
    }
}