
using GameOverlay;
using GameOverlay.Drawing;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace cs2.GameOverlay
{
    internal static class GraphicsExtensions
    {



        //public static float Distance(Vector2 v1, Vector2 v2)
        //{
        //    return MathF.Sqrt(((v2.X - v1.X) * (v2.X - v1.X)) + ((v2.Y - v1.Y) * (v2.Y - v1.Y)));
        //}

        public static Color Invert(this Color color) =>
            Color.FromARGB(0xffffff ^ color.ToARGB());

        public static Rectangle GetTextRect(this Graphics g, global::GameOverlay.Drawing.Font font, string text, out float rectOffset, float x = 1, float y = 1)
        {
            return GetTextRect(g, font, font.FontSize, text, out rectOffset, x, y);
        }

        public static void DrawText(this Graphics g, Rectangle area, global::GameOverlay.Drawing.Font font, float fontSize, string text, IBrush brush)
        {
            var r = g.GetTextRect(font, fontSize, text, out float rectOffset, 1, 1);
            g.DrawText(font, fontSize, brush, new Point(area.Left + area.Width / 2 - r.Width / 2 + rectOffset, area.Top + area.Height / 2 - r.Height / 2 + rectOffset), text);
        }

        public static void DrawText(this Graphics g, Rectangle area, global::GameOverlay.Drawing.Font font, string text, IBrush brush)
        {
            g.DrawText(area, font, font.FontSize, text, brush);
        }

        public static Rectangle GetTextRect(this Graphics g, global::GameOverlay.Drawing.Font font, float fontSize, string text, out float rectOffset, float x = 1, float y = 1)
        {
            float num = (x < 0f) ? ((float)g.Width + x) : ((float)g.Width - x);
            float num2 = (y < 0f) ? ((float)g.Height + y) : ((float)g.Height - y);
            if (num <= fontSize)
            {
                num = (float)g.Width;
            }
            if (num2 <= fontSize)
            {
                num2 = (float)g.Height;
            }
            TextLayout textLayout = new TextLayout(Fonts.FontFactory, text, font.TextFormat, num, num2);
            if (fontSize != font.FontSize)
            {
                textLayout.SetFontSize(fontSize, new TextRange(0, text.Length));
            }
            float num3 = fontSize * 0.25f;
            RawRectangleF rect = new RawRectangleF(x - num3, y - num3, x + textLayout.Metrics.Width + num3, y + textLayout.Metrics.Height + num3);

            textLayout.Dispose();

            rectOffset = num3;

            return new Rectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public static bool IsValidScreen(this SharpDX.Vector3 value)
        {
            return !value.X.IsInfinityOrNaN() && !value.Y.IsInfinityOrNaN() && value.Z >= 0 && value.Z < 1;
        }

        public static bool IsInfinityOrNaN(this float value)
        {
            return float.IsNaN(value) || float.IsInfinity(value);
        }


        private const float _PI_Over_180 = (float)Math.PI / 180.0f;

        private const float _180_Over_PI = 180.0f / (float)Math.PI;

        public static float DegreeToRadian(this float degree)
        {
            return degree * _PI_Over_180;
        }

        public static double RadianToDegree(this double radian)
        {
            return radian * _180_Over_PI;
        }

        public static SharpDX.Vector3 Cross(this SharpDX.Vector3 left, SharpDX.Vector3 right)
        {
            return SharpDX.Vector3.Cross(left, right);
        }

        /// <inheritdoc cref="Vector3.Dot"/>
        public static float Dot(this SharpDX.Vector3 left, SharpDX.Vector3 right)
        {
            return SharpDX.Vector3.Dot(left, right);
        }
        public static SharpDX.Vector3 Normalized(this SharpDX.Vector3 value)
        {
            return SharpDX.Vector3.Normalize(value);
        }

     

        public static bool IsParallelTo(this SharpDX.Vector3 vector, SharpDX.Vector3 other, float tolerance = 1E-6f)
        {
            return System.Math.Abs(1.0 - System.Math.Abs(vector.Normalized().Dot(other.Normalized()))) <= tolerance;
        }

        public static float AngleBetweenUnitVectors(SharpDX.Vector3 leftNormalized, SharpDX.Vector3 rightNormalized)
        {
            return AcosClamped(leftNormalized.Dot(rightNormalized));
        }
        public static float AcosClamped(float value, float tolerance = 1E-6f)
        {
            if (value > 1 - tolerance)
            {
                return 0;
            }
            if (value < tolerance - 1)
            {
                return (float)System.Math.PI;
            }
            return (float)System.Math.Acos(value);
        }
    }
}
