using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
//using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
//using Microsoft.TeamFoundation.VersionControl.Client;
using System.IO;
using System.Text;
//using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace KeytorcProject.Utils
{
    public class TFSHelper
    {
        TeamFoundationIdentity identity;
        
        public static string GetAppSetting(string key)
        {
            string result = string.Empty;

            if (ConfigurationManager.AppSettings[key] != null)
                return ConfigurationManager.AppSettings[key].ToString();
            else if (ConfigurationManager.ConnectionStrings[key] != null)
                return ConfigurationManager.ConnectionStrings[key].ToString();

            return result;
        }
        

    }
}