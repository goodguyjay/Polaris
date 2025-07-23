using System;
using Avalonia;

namespace Polaris.UI.Helpers;

public static class RectExtensions
{
    public static bool Intersects(this Rect a, Rect b, double minDist)
    {
        var dx = a.X + a.Width / 2 - (b.X + b.Width / 2);
        var dy = a.Y + a.Height / 2 - (b.Y + b.Height / 2);
        var dist = Math.Sqrt(dx * dx + dy * dy);
        var radiusSum = (a.Width + b.Width) / 2 / 2;
        return dist < (radiusSum + minDist);
    }
}