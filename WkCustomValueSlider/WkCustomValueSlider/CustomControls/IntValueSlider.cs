using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace WkCustomValueSlider.CustomControls
{
    public class IntValueSlider : SKGLView
    {
        public enum IntValueSliderOrientation { Vertical, Horizontal };

        public readonly static BindableProperty BorderColorProperty = BindableProperty.Create(
            nameof(BorderColor),
            typeof(Color),
            typeof(IntValueSlider),
            Color.Black,
            propertyChanged: OnDrawn_PropertyChanged);
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public readonly static BindableProperty BorderThicknessProperty =
            BindableProperty.Create(nameof(BorderThickness), typeof(int), typeof(IntValueSlider), 1, propertyChanged: OnDrawn_PropertyChanged);
        public int BorderThickness
        {
            get => (int)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public readonly static BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(IntValueSlider), 0, propertyChanged: OnDrawn_PropertyChanged);
        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public readonly static BindableProperty MinValueGradColorProperty =
                BindableProperty.Create(nameof(MinValueGradColor), typeof(Color), typeof(IntValueSlider), Color.Black, propertyChanged: OnDrawn_PropertyChanged);
        public Color MinValueGradColor
        {
            get => (Color)GetValue(MinValueGradColorProperty);
            set => SetValue(MinValueGradColorProperty, value);
        }

        public readonly static BindableProperty MaxValueGradColorProperty =
                BindableProperty.Create(nameof(MaxValueGradColor), typeof(Color), typeof(IntValueSlider), Color.Black, propertyChanged: OnDrawn_PropertyChanged);
        public Color MaxValueGradColor
        {
            get => (Color)GetValue(MaxValueGradColorProperty);
            set => SetValue(MaxValueGradColorProperty, value);
        }

        public readonly static BindableProperty UnselectedColorProperty =
                BindableProperty.Create(nameof(UnselectedColor), typeof(Color), typeof(IntValueSlider), Color.White, propertyChanged: OnDrawn_PropertyChanged);

        public Color UnselectedColor
        {
            get => (Color)GetValue(UnselectedColorProperty);
            set => SetValue(UnselectedColorProperty, value);
        }

        public readonly static BindableProperty OrientationProperty =
            BindableProperty.Create(nameof(Orientation), typeof(IntValueSliderOrientation), typeof(IntValueSlider), IntValueSliderOrientation.Vertical);
        public IntValueSliderOrientation Orientation
        {
            get => (IntValueSliderOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public readonly static BindableProperty MinValueProperty = BindableProperty.Create(
            nameof(MinValue),
            typeof(int),
            typeof(IntValueSlider),
            0,
            propertyChanged: OnDrawn_PropertyChanged);
        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public readonly static BindableProperty MaxValueProperty = BindableProperty.Create(
            nameof(MaxValue),
            typeof(int),
            typeof(IntValueSlider),
            100,
            propertyChanged: OnDrawn_PropertyChanged);
        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public readonly static BindableProperty SelectedValueProperty = BindableProperty.Create(
            nameof(SelectedValue),
            typeof(int),
            typeof(IntValueSlider),
            0,
            BindingMode.TwoWay,
            propertyChanged: OnDrawn_PropertyChanged);
        public int SelectedValue
        {
            get => (int)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        private float curBoxHeight;
        private float curBoxWidth;

        public IntValueSlider()
        {
            PaintSurface += IntValueSlider_PaintSurface;
            Touch += IntValueSlider_Touch;
            PropertyChanged += IntValueSlider_PropertyChanged;

            EnableTouchEvents = true;
        }

        private void IntValueSlider_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Height) || e.PropertyName == nameof(Width))
                InvalidateSurface();
        }

        private static void OnDrawn_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is null)
                return;

            ((IntValueSlider)bindable).InvalidateSurface();
        }

        private void IntValueSlider_Touch(object sender, SKTouchEventArgs e)
        {
            if (Orientation == IntValueSliderOrientation.Vertical)
            {
                var touchedY = e.Location.Y;

                if (touchedY > curBoxHeight)
                    SelectedValue = MinValue;
                else if (touchedY < 0)
                    SelectedValue = MaxValue;
                else
                    TouchPercToValue(100 - touchedY * 100 / curBoxHeight);
            }
            else
            {
                var touchedX = e.Location.X;

                if (touchedX > curBoxWidth)
                    SelectedValue = MaxValue;
                else if (touchedX < 0)
                    SelectedValue = MinValue;
                else
                    TouchPercToValue(touchedX * 100 / curBoxWidth);
            }

            e.Handled = true;
        }

        private void TouchPercToValue(in float tPerc)
        {
            //Interpolation formula
            SelectedValue = (int)Math.Round((MaxValue - MinValue) / (100 / tPerc) + MinValue);
        }

        private void IntValueSlider_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            curBoxWidth = e.Surface.Canvas.DeviceClipBounds.Width;
            curBoxHeight = e.Surface.Canvas.DeviceClipBounds.Height;

            canvas.Clear(SKColors.Transparent);

            using var unselectedPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                IsStroke = false,
                IsAntialias = true,
                Color = UnselectedColor.ToSKColor()
            };

            if (CornerRadius > 0)
                canvas.DrawRoundRect(0, 0, curBoxWidth, curBoxHeight, CornerRadius, CornerRadius, unselectedPaint);
            else
                canvas.DrawRect(0, 0, curBoxWidth, curBoxHeight, unselectedPaint);

            if (SelectedValue > MinValue)
            {
                var vertOrient = Orientation == IntValueSliderOrientation.Vertical;

                var colors = vertOrient ?
                    new SKColor[]
                    {
                        MaxValueGradColor.ToSKColor(),
                        MinValueGradColor.ToSKColor(),
                    } :
                    new SKColor[]
                    {
                        MinValueGradColor.ToSKColor(),
                        MaxValueGradColor.ToSKColor()
                    };

                using var fgPaintShader = vertOrient ?
                    SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, curBoxHeight), colors, SKShaderTileMode.Decal) :
                    SKShader.CreateLinearGradient(new SKPoint(0, curBoxHeight), new SKPoint(curBoxWidth, curBoxHeight), colors, SKShaderTileMode.Decal);

                using var forePaint = new SKPaint()
                {
                    Style = SKPaintStyle.Fill,
                    IsStroke = false,
                    IsAntialias = true,
                    Shader = fgPaintShader
                };

                var qttPerc = (SelectedValue - MinValue) * 100 / (MaxValue - MinValue);

                if (vertOrient)
                {
                    var invPerc = 100 - qttPerc;
                    var selectedHeight = curBoxHeight * invPerc / 100;

                    if (CornerRadius > 0)
                        canvas.DrawRoundRect(0, selectedHeight, curBoxWidth, curBoxHeight - selectedHeight, CornerRadius, CornerRadius, forePaint);
                    else
                        canvas.DrawRect(0, selectedHeight, curBoxWidth, curBoxHeight - selectedHeight, forePaint);
                }
                else
                {
                    var selectedWidth = curBoxWidth * qttPerc / 100;

                    if (CornerRadius > 0)
                        canvas.DrawRoundRect(0, 0, selectedWidth, curBoxHeight, CornerRadius, CornerRadius, forePaint);
                    else
                        canvas.DrawRect(0, 0, selectedWidth, curBoxHeight, forePaint);
                }
            }

            using var borderPaint = new SKPaint()
            {
                Color = BorderColor.ToSKColor(),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = BorderThickness,
                IsStroke = true,
                IsAntialias = true
            };

            if (CornerRadius > 0)
                canvas.DrawRoundRect(0, 0, curBoxWidth, curBoxHeight, CornerRadius, CornerRadius, borderPaint);
            else
                canvas.DrawRect(0, 0, curBoxWidth, curBoxHeight, borderPaint);
        }
    }
}
