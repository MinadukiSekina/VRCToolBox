using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VRCToolBox.Common
{
    public class ProcessEx : Process
    {
        public static Process? Start(string fileName, bool useShellExecute)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = fileName;
            processStartInfo.UseShellExecute = useShellExecute;
            return Process.Start(processStartInfo);
        }
    }
}
