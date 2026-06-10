using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CoffeeManager.Front.Styles
{
    public class CardPanel : Panel
    {
        public CardPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(235, 255, 255, 255);
            Padding = new Padding(25);
            Paint += CardPanel_Paint;
        }

        private void CardPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;
            rect.Inflate(-1, -1);

            using var path = RoundedRect(rect, 18);
            using var brush = new SolidBrush(Color.FromArgb(235, 255, 255, 255));
            using var pen = new Pen(Color.FromArgb(200, 200, 200), 1);

            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
