using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Classes
{
    public class csMS
    {
        public static string _masterno = "";

        internal static string CusId
        {
            get { return _masterno; }
            set { _masterno = value; }
        }
    }
}
