using System;
using UIKit;
using RectangleF = global::CoreGraphics.CGRect;
using SizeF = global::CoreGraphics.CGSize;
using PointF = global::CoreGraphics.CGPoint;

namespace Plugin.RichEditor.iOS
{
    public class ColorPickerCustomViewController : UIViewController
    {
        private ColorPickerHueCircleView _hueCircleView;
        private ColorPickerHSBView _hsbView;

        private UISlider _sliderR;
        private UISlider _sliderG;
        private UISlider _sliderB;

        private UILabel _labelRValue;
        private UILabel _labelGValue;
        private UILabel _labelBValue;

        private readonly FavoriteColorManager _favoriteColorManager;

        public ColorPickerCustomViewController()
            : base()
        {
            _favoriteColorManager = new FavoriteColorManager();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetFavoriteButton();
            SetHueCircleView();
            SetHSLView();
            SetSliders();
        }

        private void SetFavoriteButton()
        {
            var favoriteButton = new UIButton(UIButtonType.Custom) { Frame = new RectangleF(280, 10, 30, 30) };
            favoriteButton.SetImage(new UIImage("color-picker-save@2x.png"), UIControlState.Normal);
            favoriteButton.TouchDown += (object sender, EventArgs e) =>
            {
                var newColor = UIColor.FromRGB(_hsbView.Red, _hsbView.Green, _hsbView.Blue);
                newColor.GetRGBA(out var r, out var g, out var b, out _);

                var count = _favoriteColorManager.List().Count;
                var colorName = $"{count + 1}_{r}_{g}_{b}";
                _favoriteColorManager.Add(colorName);
            };

            View.AddSubview(favoriteButton);
        }

        private void HueCircleView_ValueChanged(object sender, EventArgs e)
        {
            _hsbView.Hue = _hueCircleView.Hue;
        }

        private void HSLView_ValueChanged(object sender, EventArgs e)
        {
            _sliderR.SetValue(_hsbView.Red, false);
            _sliderG.SetValue(_hsbView.Green, false);
            _sliderB.SetValue(_hsbView.Blue, false);

            UpdateLabels();
        }

        private void SetHueCircleView()
        {
            var w = 320f - 80f;
            var h = w;
            var frame = new RectangleF(40f, 10f, w, h);

            _hueCircleView = new ColorPickerHueCircleView(frame);
            _hueCircleView.Layer.MasksToBounds = true;
            _hueCircleView.Changed += HueCircleView_ValueChanged;

            this.View.AddSubview(_hueCircleView);
        }

        private void SetHSLView()
        {
            var w = 140f;
            var h = 140f;
            var frame = new RectangleF(90f, 60f, w, h);

            _hsbView = new ColorPickerHSBView(frame);
            _hsbView.Layer.MasksToBounds = true;
            _hsbView.Changed += HSLView_ValueChanged;

            this.View.AddSubview(_hsbView);
        }

        private void SetSliders()
        {
            var y = DisplayHelper.Is4InchDisplay() ? 40f : 35f;
            var sliderSize = new SizeF(320f - 135f, 20f);
            var sliderX = 55f;

            _sliderR = new UISlider(new RectangleF(new PointF(sliderX, 270f), sliderSize))
            {
                MaxValue = 255,
                MinValue = 0,
                Value = 255
            };
            _sliderR.ValueChanged += Slider_ValueChanged;
            this.View.AddSubview(_sliderR);

            _sliderG = new UISlider(new RectangleF(new PointF(sliderX, 270f + y), sliderSize))
            {
                MaxValue = 255,
                MinValue = 0
            };
            _sliderG.ValueChanged += Slider_ValueChanged;
            this.View.AddSubview(_sliderG);

            _sliderB = new UISlider(new RectangleF(new PointF(sliderX, 270f + (y * 2)), sliderSize))
            {
                MaxValue = 255,
                MinValue = 0
            };
            _sliderB.ValueChanged += Slider_ValueChanged;
            this.View.AddSubview(_sliderB);

            var labelX = 40f;
            var labelSize = new SizeF(20f, 20f);

            var labelR = new UILabel(new RectangleF(new PointF(labelX, _sliderR.Frame.Y), labelSize))
            {
                Text = "R",
                TextColor = UIColor.White
            };
            this.View.AddSubview(labelR);

            var labelG = new UILabel(new RectangleF(new PointF(labelX, _sliderG.Frame.Y), labelSize))
            {
                Text = "G",
                TextColor = UIColor.White
            };
            this.View.AddSubview(labelG);

            var labelB = new UILabel(new RectangleF(new PointF(labelX, _sliderB.Frame.Y), labelSize))
            {
                Text = "B",
                TextColor = UIColor.White
            };
            this.View.AddSubview(labelB);

            var labelValueX = sliderSize.Width + 65f;
            var labelValueSize = new SizeF(40f, 20f);

            _labelRValue = new UILabel(new RectangleF(new PointF(labelValueX, _sliderR.Frame.Y), labelValueSize))
            {
                Text = "255",
                TextColor = UIColor.White
            };
            this.View.AddSubview(_labelRValue);

            _labelGValue = new UILabel(new RectangleF(new PointF(labelValueX, _sliderG.Frame.Y), labelValueSize))
            {
                Text = "0",
                TextColor = UIColor.White
            };
            this.View.AddSubview(_labelGValue);

            _labelBValue = new UILabel(new RectangleF(new PointF(labelValueX, _sliderB.Frame.Y), labelValueSize))
            {
                Text = "0",
                TextColor = UIColor.White
            };
            this.View.AddSubview(_labelBValue);
        }

        private void UpdateLabels()
        {
            _labelRValue.Text = _hsbView.Red.ToString();
            _labelGValue.Text = _hsbView.Green.ToString();
            _labelBValue.Text = _hsbView.Blue.ToString();

            var parent = this.ParentViewController as PopColorPickerViewController;
            parent.SelectedColor = UIColor.FromRGB(_hsbView.Red, _hsbView.Green, _hsbView.Blue);
        }

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            _hsbView.Red = (byte)_sliderR.Value;
            _hsbView.Green = (byte)_sliderG.Value;
            _hsbView.Blue = (byte)_sliderB.Value;

            _hueCircleView.Hue = _hsbView.Hue;
        }
    }
}