using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWON_GUI.Controls
{
    public class Led : TemplatedControl
    {
        // Proprietà Dipendenti
        public static readonly StyledProperty<double> DiameterProperty =
            AvaloniaProperty.Register<Led, double>(nameof(Diameter), 50.0);

        public static readonly StyledProperty<Color> OnColorProperty =
            AvaloniaProperty.Register<Led, Color>(nameof(OnColor), Colors.LimeGreen);

        public static readonly StyledProperty<Color> OffColorProperty =
            AvaloniaProperty.Register<Led, Color>(nameof(OffColor), Colors.DarkGray);

        public static readonly StyledProperty<bool> IsOnProperty =
            AvaloniaProperty.Register<Led, bool>(nameof(IsOn), false);

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<Led, string>(nameof(Text), string.Empty);

        public static readonly StyledProperty<FontFamily> TextFontFamilyProperty =
            AvaloniaProperty.Register<Led, FontFamily>(nameof(TextFontFamily), FontFamily.Default);

        public static readonly StyledProperty<double> TextFontSizeProperty =
            AvaloniaProperty.Register<Led, double>(nameof(TextFontSize), 12.0);

        public static readonly StyledProperty<Color> TextColorProperty =
            AvaloniaProperty.Register<Led, Color>(nameof(TextColor), Colors.White);

        // Proprietà
        /// <summary>
        /// Diametro del cerchio LED in pixel
        /// </summary>
        public double Diameter
        {
            get => GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }

        /// <summary>
        /// Colore del LED quando acceso
        /// </summary>
        public Color OnColor
        {
            get => GetValue(OnColorProperty);
            set => SetValue(OnColorProperty, value);
        }

        /// <summary>
        /// Colore del LED quando spento
        /// </summary>
        public Color OffColor
        {
            get => GetValue(OffColorProperty);
            set => SetValue(OffColorProperty, value);
        }

        /// <summary>
        /// Stato del LED (acceso/spento)
        /// </summary>
        public bool IsOn
        {
            get => GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        /// <summary>
        /// Testo da visualizzare nel LED (es. "ON", "OFF", simboli)
        /// </summary>
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Font del testo
        /// </summary>
        public FontFamily TextFontFamily
        {
            get => GetValue(TextFontFamilyProperty);
            set => SetValue(TextFontFamilyProperty, value);
        }

        /// <summary>
        /// Dimensione del font del testo
        /// </summary>
        public double TextFontSize
        {
            get => GetValue(TextFontSizeProperty);
            set => SetValue(TextFontSizeProperty, value);
        }

        /// <summary>
        /// Colore del testo
        /// </summary>
        public Color TextColor
        {
            get => GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }



        private  class SimpleObserver<T> : IObserver<T>
        {
            private readonly Action<T> _onNext;

            public SimpleObserver(Action<T> onNext)
            {
                _onNext = onNext;
            }

            public void OnCompleted() { }
            public void OnError(Exception error) { }
            public void OnNext(T value) => _onNext(value);
        }


        public Led()
        {
            // Impostazioni di default
            //Width = 50;
            //Height = 50;
            //HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            //VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

            //associo che ad ogni mod della property si ridisegna
            this.GetObservable(DiameterProperty).Subscribe(new SimpleObserver<double>(_ => InvalidateVisual()));
            this.GetObservable(OnColorProperty).Subscribe(new SimpleObserver<Color>(_ => InvalidateVisual()));
            this.GetObservable(OffColorProperty).Subscribe(new SimpleObserver<Color>(_ => InvalidateVisual()));
            this.GetObservable(IsOnProperty).Subscribe(new SimpleObserver<bool>(_ => InvalidateVisual()));
            this.GetObservable(TextProperty).Subscribe(new SimpleObserver<String>(_ => InvalidateVisual()));
            this.GetObservable(TextFontFamilyProperty).Subscribe(new SimpleObserver<FontFamily>(_ => InvalidateVisual()));
            this.GetObservable(TextFontSizeProperty).Subscribe(new SimpleObserver<double>(_ => InvalidateVisual()));
            this.GetObservable(TextColorProperty).Subscribe(new SimpleObserver<Color>(_ => InvalidateVisual()));
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            UpdateLayout();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = new Size(Diameter, Diameter);
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(Diameter, Diameter);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            // Seleziona il colore in base allo stato
            var currentColor = IsOn ? OnColor : OffColor;
            var brush = new SolidColorBrush(currentColor);

            // Disegna il cerchio
            var radius = (Diameter-2) / 2.0;
            var center = new Point(radius+1, radius+1);
            context.DrawEllipse(brush, null, center, radius, radius);

            // Disegna il bordo più scuro per effetto 3D
            var borderColor = IsOn
                ? currentColor.Darken(0.3)
                : currentColor.Darken(0.2);
            var borderBrush = new SolidColorBrush(borderColor);
            var borderPen = new Pen(borderBrush, 2.0);
            context.DrawEllipse(null, borderPen, center, radius, radius);

            // Disegna il testo se presente
            if (!string.IsNullOrEmpty(Text))
            {
                var textBrush = new SolidColorBrush(TextColor);
                var formattedText = new FormattedText(
                    Text,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(TextFontFamily),
                    TextFontSize,
                    textBrush);

                // Centra il testo
                var textX = center.X - formattedText.Width / 2.0;
                var textY = center.Y - formattedText.Height / 2.0;

                context.DrawText(formattedText, new Point(textX, textY));
            }
        }
    }

    // Estensione per scurire i colori (effetto 3D)
    public static class ColorExtensions
    {
        public static Color Darken(this Color color, double factor)
        {
            return new Color(
                color.A,
                (byte)(color.R * (1 - factor)),
                (byte)(color.G * (1 - factor)),
                (byte)(color.B * (1 - factor)));
        }
    }
}
