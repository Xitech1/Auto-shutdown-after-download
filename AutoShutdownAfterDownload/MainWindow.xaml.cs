using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoShutdownAfterDownload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessHandler processHandler;
        private List<SimpleProcess> curProcessesSimple;
        private ShutdownHandler shutdownHandler;
        private List<Process> curProcesses = new List<Process>();
        private Timer timer = new Timer();
        private long totalDownloadReceived = 0;
        private SimpleProcess curFollowingProcess;

        private int ticked = 0;

        private readonly int NUMBER_OF_TICKS_NEEDED_TO_SHUTDOWN = 15;
        public MainWindow()
        {
            InitializeComponent();
            shutdownHandler = new ShutdownHandler();
            FillList();

        }

        void StartTimer()
        {
            timer.Interval = 1000;
            timer.Elapsed += TimerThick;
            timer.Start();
        }

        private void TimerThick(object sender, ElapsedEventArgs e)
        {
            shutdownHandler.CancelShutdown();
            if (TotalDownloadChanged(totalDownloadReceived))
            {
                ticked = 0;
            }
            else
            {
                ticked++;

                //We waited 5 minutes and nothing has changed.
                if (ticked == 300 && totalDownloadReceived == 0)
                {
                    shutdownHandler.ScheduleShutdown(0);
                }

                //We waited 15 seconds and we have havent received new data anymore
                if (ticked >= NUMBER_OF_TICKS_NEEDED_TO_SHUTDOWN && totalDownloadReceived != 0)
                {
                    shutdownHandler.ScheduleShutdown(0);
                }
            }
            Console.WriteLine(processHandler.GetTotalReceived());
            totalDownloadReceived = processHandler.GetTotalReceived();
        }

        bool TotalDownloadChanged(long oldTotalDownload)
        {
            if (processHandler.GetTotalReceived() > oldTotalDownload)
            {
                return true;
            }
            return false;
        }

        void FillList()
        {
            processHandler = new ProcessHandler(null);
            var list = processHandler.GetListOfProcesses();
            curProcesses = list.ToList();
            curProcessesSimple = ConvertToSimpleList(list);
            grid_process.ItemsSource = curProcessesSimple;
        }

        void MoveSelectedItem()
        {
            if (grid_process.SelectedItem != null)
            {
                var selectedItem = (SimpleProcess)grid_process.SelectedItem;
                grid_selected.ItemsSource = new List<SimpleProcess>() { selectedItem };
            }
        }

        private void btn_activateAutoShutDown_Click(object sender, RoutedEventArgs e)
        {
            if (grid_selected.Items.Count < 0) return;
            curFollowingProcess = (SimpleProcess)grid_selected.Items[0];
            SelectProcessToMonitor(curFollowingProcess.ID);
            grid_selected.ItemsSource = null;
            txtbox_currentlyActive.Text = "Currently active on: " + curFollowingProcess.Name;

            StartTimer();
        }

        void SelectProcessToMonitor(int processID)
        {
            Process proc = GetProccessByID(processID);
            processHandler = new ProcessHandler(proc);
            processHandler = processHandler.StartMonitoringProcess();
        }

        Process GetProccessByID(int id)
        {
            for (int i = 0; i < curProcesses.Count; i++)
            {
                if (curProcesses[i].Id == id)
                {
                    return curProcesses[i];
                }
            }
            return null;
        }

        private List<SimpleProcess> ConvertToSimpleList(Process[] list)
        {
            var listToReturn = new List<SimpleProcess>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].ProcessName != "svchost")
                {
                    listToReturn.Add(new SimpleProcess(list[i].ProcessName, list[i].MainWindowTitle, list[i].Id));
                }
            }
            return listToReturn;
        }

        private void btn_MoveToSelected(object sender, RoutedEventArgs e)
        {
            MoveSelectedItem();
        }
    }
}