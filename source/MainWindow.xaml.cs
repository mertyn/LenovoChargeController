using System;
using System.Management;
using System.Windows;
using LenovoController.Features;

namespace LenovoController
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly ManagementClass wmi;
        private readonly BatteryFeature _batteryFeature = new BatteryFeature();
        private bool _isRunning;

        public MainWindow()
        {
            InitializeComponent();

            // mainWindow.Title += $" v{AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version}";

            wmi = new ManagementClass("Win32_Battery");

            GetState();
        }

        private void GetState()
        {
            Status.Text = _batteryFeature.GetState().ToString();
        }

        private int GetBatteryPercent()
        {
            int percent = -1;
            foreach (ManagementBaseObject battery in wmi.GetInstances())
                percent = Convert.ToInt16(battery["EstimatedChargeRemaining"]);

            return percent;
        }

        private void OnStartStop(object sender, RoutedEventArgs e)
        {
            _isRunning = !_isRunning;

            StartStop.Content = _isRunning ? "Stop" : "Start";
        }

    }
}