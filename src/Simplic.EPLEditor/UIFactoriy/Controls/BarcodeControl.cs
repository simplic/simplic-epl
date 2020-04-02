using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplic.EPL;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Barcode;

namespace Simplic.EPLEditor
{
    /// <summary>
    /// Render barcode into canvas
    /// </summary>
    public class BarcodeControl : UIControl
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
            EPL.BarcodeCommand cmd = (EPL.BarcodeCommand)command;

            BarcodeBase barcode = null;

            switch (cmd.P4)
            {
                case "3":
                    barcode = new RadBarcode39();
                    break;

                case "1A":
                    barcode = new RadBarcode128A();
                    break;
                case "B":
                    barcode = new RadBarcode128B();
                    break;
                case "C":
                    barcode = new RadBarcode128C();
                    break;

                case "E80":
                case "E82":
                case "E85":
                    barcode = new RadBarcodeEAN8();
                    break;

                case "E30":
                case "E32":
                case "E35":
                    barcode = new RadBarcodeEAN8();
                    break;

                case "UA0":
                    barcode = new RadBarcodeUPCA();
                    break;

                case "UE0":
                    barcode = new RadBarcodeUPCA();
                    break;


                case "1":
                default:
                    barcode = new RadBarcode128();
                    break;
            }

            if (barcode != null)
            {
                barcode.ToolTip = cmd.CommandName;
                string displayText = cmd.Data ?? "--null--";

                barcode.FontFamily = new FontFamily("Consolas");

                if (displayText.StartsWith("\""))
                {
                    displayText = displayText.Remove(0, 1);
                }
                if (displayText.EndsWith("\""))
                {
                    displayText = displayText.Remove(displayText.Length - 1, 1);
                }

                barcode.Text = displayText;

                // Set coordinates
                int _x = 0;
                int _y = 0;

                if (int.TryParse(cmd.P1, out _x) && int.TryParse(cmd.P2, out _y))
                {
                    var coord = UnitHelper.DotToPixel(8, _x, _y, dpiX, dpiY);

                    if (relative != null)
                    {
                        Canvas.SetLeft(barcode, coord.Item1 + relative.Item1);
                        Canvas.SetTop(barcode, coord.Item2 + relative.Item2);
                    }
                    else
                    {
                        Canvas.SetLeft(barcode, coord.Item1);
                        Canvas.SetTop(barcode, coord.Item2);
                    }
                }

                // Set height, default will be 70 dots
                double bHeight = 70;
                double.TryParse(cmd.P7, out bHeight);
                barcode.Height = UnitHelper.DotToPixel(8, bHeight, dpiY) + 5;

                switch (cmd.P3)
                {
                    case "1":
                        barcode.LayoutTransform = new RotateTransform(90);
                        break;

                    case "2":
                        barcode.LayoutTransform = new RotateTransform(180);
                        break;

                    case "3":
                        barcode.LayoutTransform = new RotateTransform(270);
                        break;
                }

                //block.FontSize = PointsToPixels(12);

                barcode.ShowText = true;

                canvas.Children.Add(barcode);

                barcode.Background = new SolidColorBrush(Colors.WhiteSmoke);

                canvas.UpdateLayout();
                barcode.UpdateLayout();

                int narrowBarWide = 1;
                if (int.TryParse(cmd.P5, out narrowBarWide))
                {
                    barcode.Width = barcode.ActualWidth * narrowBarWide;
                }

                // Must be under the width calculation, because otherwise it did not work...
                if (cmd.P8 != "B")
                {
                    barcode.ShowText = false;
                }

                barcode.ShowChecksum = false;
            }
        }
    }
}