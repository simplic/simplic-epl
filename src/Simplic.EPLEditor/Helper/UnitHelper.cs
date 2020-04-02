using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPLEditor
{
    /// <summary>
    /// Helper for working with screen, printer and other coordinate systems / units.
    /// Sample: inch, mm, cm, ppi, dpi
    /// </summary>
    public static class UnitHelper
    {
        /// <summary>
        /// Convert dot with dot per millimeter to screen coordinates based on specific dpi
        /// </summary>
        /// <param name="dotPerMM">Dots / mm</param>
        /// <param name="dotX">X coordinate of the dot</param>
        /// <param name="dotY">Y coordinate of the dot</param>
        /// <param name="targetDPI">DPI of the target screen</param>
        /// <returns>X/Y coordinates on the screen</returns>
        public static Tuple<double, double> DotToPixel(int dotPerMM, double dotX, double dotY, int targetDPIX, int targetDPIY)
        {            
            double x = (dotX / dotPerMM) / 10.0;
            double y = (dotY / dotPerMM) / 10.0;
            
            return new Tuple<double, double>(x / 2.54 * targetDPIX, y / 2.54 * targetDPIY);
        }

        public static double DotToPixel(int dotPerMM, double dot, int targetDPI)
        {
            return ((dot / (double)dotPerMM) / 10.0) / 2.54 * targetDPI;
        }

        public static double PixelToDot(int dotPerMM, double pixel, int dpi)
        {
            double cm = pixel * (2.54 / (double)dpi);

            // CM * 10 = mm * dotPerMM = dots

            return cm * 10 * (double)dotPerMM;
        }

        /// <summary>
        /// Centimeter to pixel
        /// </summary>
        /// <param name="cm">Centimeter to use which will be turned into pixel</param>
        /// <param name="dpi">DPI of the screen</param>
        /// <returns>Cm -> Pixel</returns>
        public static double CmToPx(double cm, int dpi)
        {
            return cm / 2.54 * dpi;
        }
    }
}
