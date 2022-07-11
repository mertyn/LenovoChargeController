using System;
using System.Management;
using System.Windows;
using System.Windows.Threading;
using LenovoController.Features;

namespace LenovoController
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int UPDATE_INTERVAL = 100;
        private readonly ManagementClass _wmi;
        private readonly BatteryFeature _batteryFeature = new BatteryFeature();
        private bool _isRunning;

        public MainWindow()
        {
            InitializeComponent();

            // mainWindow.Title += $" v{AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version}";

            _wmi = new ManagementClass("Win32_Battery");

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += OnUpdate;
            timer.Interval = new TimeSpan(0, 0, 0, 0, UPDATE_INTERVAL);
            timer.Start();
        }

        private int GetBatteryPercentage()
        {
            int percent = -1;
            foreach (ManagementBaseObject battery in _wmi.GetInstances())
                percent = Convert.ToInt16(battery["EstimatedChargeRemaining"]);

            return percent;
        }

        private void OnUpdate(object sender, EventArgs eventArgs)
        {
            Update();
        }

        private void Update()
        {
            ChargeState.Text = GetBatteryPercentage().ToString();
        }

        private void OnStartStop(object sender, RoutedEventArgs e)
        {
            _isRunning = !_isRunning;

            StartStop.Content = _isRunning ? "Stop" : "Start";
        }

    }
}