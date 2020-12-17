using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplic.EPL;

namespace Simplic.EPLDesigner
{
    public class IronPythonProcessor : Simplic.EPL.IPreprocessor
    {
        private Simplic.Dlr.DlrHost<Simplic.Dlr.IronPythonLanguage> host;

        public IronPythonProcessor()
        {
            host = new Dlr.DlrHost<Dlr.IronPythonLanguage>(new Dlr.IronPythonLanguage());
        }

        public string Process(string code, EPLFormular formular, int commandIndex)
        {
            host.DefaultScope.Execute(code);
            return "";
        }

        public string ProcessStatement(string statement)
        {
            var val = host.DefaultScope.Execute(statement);

            if (val == null)
            {
                return "<null>";
            }
            else
            {
                return val.ToString();
            }
        }
    }
}
