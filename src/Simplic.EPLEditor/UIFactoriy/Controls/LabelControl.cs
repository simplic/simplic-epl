using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplic.EPL;
using System.Windows.Controls;
using System.Windows.Media;

namespace Simplic.EPLEditor
{
    /// <summary>
    /// Label control factory
    /// </summary>
    public class LabelControl : UIControl
    {
        ///<summary>
        /// Render the control on a canvas
        ///</summary>
        /// <param name="canvas">Canvas formular</param>
        /// <param name="formular">Formular</param>
        /// <param name="command">Command instance</param>
        /// <param name="relative">Relative position</param>
        /// <param name="dpi">DPI</param>
        public override void Render(Canvas canvas, EPLFormular formular, EPLCommand command, Tuple<double, double> relative, int dpiX, int dpiY)
        {
            EPL.TextCommand cmd = (EPL.TextCommand)command;

            TextBlock block = new TextBlock();
            block.FontFamily = new FontFamily("Consolas");
            block.ToolTip = cmd.CommandName;
            string displayText = cmd.Data ?? "--null--";

            if (displayText.StartsWith("\""))
            {
                displayText = displayText.Remove(0, 1);
            }
            if (displayText.EndsWith("\""))
            {
                displayText = displayText.Remove(displayText.Length - 1, 1);
            }

            block.Text = displayText;

            // Set coordinates
            int _x = 0;
            int _y = 0;

            if (int.TryParse(cmd.P1, out _x) && int.TryParse(cmd.P2, out _y))
            {
                var coord = UnitHelper.DotToPixel(8, _x, _y, dpiX, dpiY);

                if (relative != null)
                {
                    Canvas.SetLeft(block, coord.Item1 + relative.Item1);
                    Canvas.SetTop(block, coord.Item2 + relative.Item2);
                }
                else
                {
                    Canvas.SetLeft(block, coord.Item1);
                    Canvas.SetTop(block, coord.Item2);
                }
            }

            switch (cmd.P3)
            {
                case "1":
                    block.LayoutTransform = new RotateTransform(90);
                    break;

                case "2":
                    block.LayoutTransform = new RotateTransform(180);
                    break;

                case "3":
                    block.LayoutTransform = new RotateTransform(270);
                    break;
            }

            //block.FontSize = PointsToPixels(12);

            if (cmd.P7 == "R")
            {
                block.Foreground = new SolidColorBrush(Colors.White);
                block.Background = new SolidColorBrush(Colors.Black);
            }

            canvas.Children.Add(block);
        }
    }
}
