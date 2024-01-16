using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;   //This namespace is used to work with WMI classes. For using this namespace add reference of System.Management.dll .
using Microsoft.Win32;     //This namespace is used to work with Registry editor.

namespace Launcher.Classes
{
    public class SystemInfo
    {
        public static string getOperatingSystemInfo()
        {
            string info = "";
            info += "Displaying operating system info\n";
            //Create an object of ManagementObjectSearcher class and pass query as parameter.
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject managementObject in mos.Get())
            {
                if (managementObject["Caption"] != null)
                {
                    info += "Operating System Name  :  " + managementObject["Caption"].ToString() + "\n";   //Display operating system caption
                }
                if (managementObject["OSArchitecture"] != null)
                {
                    info += "Operating System Architecture  :  " + managementObject["OSArchitecture"].ToString() + "\n";   //Display operating system architecture.
                }
                if (managementObject["CSDVersion"] != null)
                {
                    info += "Operating System Service Pack   :  " + managementObject["CSDVersion"].ToString() + "\n";     //Display operating system version.
                }
            }
            return info;
        }

        public static string getProcessorInfo()
        {
            RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.

            if (processor_name != null)
            {
                if (processor_name.GetValue("ProcessorNameString") != null)
                {
                    return Convert.ToString(processor_name.GetValue("ProcessorNameString"));   //Display processor ingo.
                }
            }
            return "";
        }
    }
}
