using Simplic.EPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Simplic.EPLEditor
{
    /// <summary>
    /// Base class which must be implemented in all controls for creating ui elements
    ///<summary>
    public abstract class UIControl
    {
        ///<summary>
        /// Render the control on a canvas
        ///</summary>
        /// <param name="canvas">Canvas formular</param>
        /// <param name="formular">Formular</param>
        /// <param name="command">Command instance</param>
        /// <param name="fixPoint">Relative position</param>
        /// <param name="dpi">DPI</param>
        public abstract void Render(Canvas canvas, EPLFormular formular, EPLCommand command, Tuple<double, double> fixPoint, int dpiX, int dpiY);
    }
}
