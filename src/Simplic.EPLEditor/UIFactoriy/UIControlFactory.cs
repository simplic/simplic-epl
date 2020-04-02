using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.EPLEditor
{
    /// <summary>
    /// Factory for creating ui controls
    /// </summary>
    public static class UIControlFactory
    {
        #region Private Member
        private static IDictionary<Type, UIControl> controls;
        #endregion

        #region Constructor
        /// <summary>
        /// Create command
        /// </summary>
        static UIControlFactory()
        {
            controls = new Dictionary<Type, UIControl>();

            controls.Add(typeof(EPL.TextCommand), new LabelControl());
            controls.Add(typeof(EPL.BarcodeCommand), new BarcodeControl());
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get UI Control factory instance
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static UIControl Get(Type type)
        {
            if (controls.ContainsKey(type))
            {
                return controls[type];
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
