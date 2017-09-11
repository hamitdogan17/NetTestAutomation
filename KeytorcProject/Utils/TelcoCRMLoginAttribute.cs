using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeytorcProject.Utils
{
    public class TelcoCRMLoginAttribute : Attribute
    {
        private TCRMUser userName;
        public TCRMUser UserName
        {
            get { return this.userName; }
        }


        public TelcoCRMLoginAttribute(TCRMUser userName)
        {
            this.userName = userName;
        }
    }
}
