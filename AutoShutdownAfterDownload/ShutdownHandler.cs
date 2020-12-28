using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShutdownAfterDownload
{
    public class ShutdownHandler
    {
        public void ScheduleShutdown(float secondsUntillShutdown)
        {
            var psi = new ProcessStartInfo("shutdown", "/s /t " + secondsUntillShutdown);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        public void CancelShutdown()
        {
            var psi = new ProcessStartInfo("shutdown", "-a");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }
    }
}