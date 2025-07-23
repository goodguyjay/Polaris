using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace Polaris.UI.Views.Controls;

public sealed partial class StarField : UserControl
{
    private readonly List<Ellipse> _stars = [];
    private readonly Random _random = new();

    public int StarCount { get; set; } = 100;
    public double MinStarSize { get; set; } = 1.0;
    public double MaxStarSize { get; set; } = 3.7;
    public double MinDistance { get; set; } = 12.0;

    // safe zones
    public Rect LogoSafeZone { get; set; }
    public Rect BottomSafeZone { get; set; }

    public StarField()
    {
        InitializeComponent();

        AttachedToVisualTree += (_, _) => GenerateStars();
        SizeChanged += (_, _) => GenerateStars();

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        timer.Tick += (_, _) => TwinkleStars();
        timer.Start();
    }

    private void GenerateStars()
    {
        StarCanvas.Children.Clear();
        _stars.Clear();

        var width = StarCanvas.Bounds.Width > 0 ? StarCanvas.Bounds.Width : 600;
        var height = StarCanvas.Bounds.Height > 0 ? StarCanvas.Bounds.Height : 400;

        // use defaults unless set externally
        var logoSafeRect =
            LogoSafeZone.Width > 0
                ? LogoSafeZone
                : new Rect(width * 0.25, height * 0.38, width * 0.5, height * 0.18);
        var bottomSafeRect =
            BottomSafeZone.Width > 0
                ? BottomSafeZone
                : new Rect(width * 0.2, height * 0.83, width * 0.6, height * 0.15);

        var attempts = 0;
        while (_stars.Count < StarCount && attempts < StarCount * 15)
        {
            var size = MinStarSize + _random.NextDouble() * (MaxStarSize - MinStarSize);
            var x = _random.NextDouble() * (width - size);
            var y = _random.NextDouble() * (height - size);

            var newStarRect = new Rect(x, y, size, size);

            // avoid logo/loading safe zones
            if (logoSafeRect.Intersects(newStarRect) || bottomSafeRect.Intersects(newStarRect))
            {
                attempts++;
                continue;
            }

            var overlaps = (
                from existingStar in _stars
                let ex = Canvas.GetLeft(existingStar)
                let ey = Canvas.GetTop(existingStar)
                let esize = existingStar.Width
                select new Rect(ex, ey, esize, esize)
            ).Any(existingRect => existingRect.Intersects(newStarRect));

            if (overlaps)
            {
                attempts++;
                continue;
            }

            var colors = new[] { "#FFE7F6FF", "#FFDDE6FF", "#FFFDF7B2", "#FFF6E7B4", "#FFC1D8FF" };
            var fill = Brush.Parse(colors[_random.Next(colors.Length)]);

            var star = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = fill,
                Opacity = 0.7 + _random.NextDouble() * 0.3,
            };

            Canvas.SetLeft(star, x);
            Canvas.SetTop(star, y);

            StarCanvas.Children.Add(star);
            _stars.Add(star);

            attempts = 0; // reset attempts after successful addition
        }
    }

    private void TwinkleStars()
    {
        foreach (var star in _stars)
        {
            var baseOpacity =
                0.6 + 0.4 * Math.Sin(DateTime.Now.Millisecond / 120.0 + _random.NextDouble());
            baseOpacity += _random.NextDouble() * 0.15 - 0.075;
            baseOpacity = Math.Clamp(baseOpacity, 0.3, 1.0);
            star.Opacity = baseOpacity;
        }
    }
}
