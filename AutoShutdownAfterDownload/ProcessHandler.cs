using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoShutdownAfterDownload
{
    public class ProcessHandler : IDisposable
    {
        private TraceEventSession etwSession;

        public readonly Counters counterReadOnly = new Counters();
        private Process processToMonitor;

        public ProcessHandler(Process processToMonitor)
        {
            this.processToMonitor = processToMonitor;
        }

        public ProcessHandler StartMonitoringProcess()
        {
            var networkPerformancePresenter = new ProcessHandler(processToMonitor);
            networkPerformancePresenter.Initialise();
            return networkPerformancePresenter;
        }

        private void Initialise()
        {
            // Note that the ETW class blocks processing messages, so should be run on a different thread if you want the application to remain responsive.
            Task.Run(() => StartEtwSession());
        }

        public Process[] GetListOfProcesses()
        {
            return Process.GetProcesses();
        }

        public long GetTotalReceived()
        {
            return counterReadOnly.Received;
        }

        private void StartEtwSession()
        {
            try
            {
                var processId = processToMonitor.Id;

                using (etwSession = new TraceEventSession("MyKernelAndClrEventsSession"))
                {
                    etwSession.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);

                    etwSession.Source.Kernel.TcpIpRecv += data =>
                    {
                        if (data.ProcessID == processId)
                        {
                            counterReadOnly.Received += data.size;
                        }
                    };

                    etwSession.Source.Process();
                }
            }
            catch (Exception e)
            {
                //ResetCounters(); // Stop reporting figures
                throw e;
            }
        }

        public void Dispose()
        {
            etwSession?.Dispose();
        }
    }
}
