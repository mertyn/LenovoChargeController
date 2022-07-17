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
        private int _chargeGoal;

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
            // update displays
            int batteryPercentage = GetBatteryPercentage();
            BatteryState state = _batteryFeature.GetState();

            ChargeState.Text = batteryPercentage.ToString();

            bool isIdle = state == BatteryState.Conservation;

            ChargeGoal.IsEnabled = isIdle;
            RadioGroupMode.IsEnabled = isIdle;
            StartStop.Content = isIdle ? "Start" : "Stop";

            if (!isIdle)
            {
                Status.Text = state == BatteryState.RapidCharge ? "Fast charging..." : "Charging...";

                // update battery mode
                if (batteryPercentage >= _chargeGoal)
                    _batteryFeature.SetState(BatteryState.Conservation);
            }
            else
                Status.Text = "Idle";
        }

        private void OnStartStop(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ChargeGoal.Text, out _chargeGoal))
            {
                ChargeGoal.Text = "0";
                _chargeGoal = 0;
            }

            if (_batteryFeature.GetState() == BatteryState.Conservation)
            {
                _batteryFeature.SetState(BatteryState.Normal);
            }
            else
                _batteryFeature.SetState(BatteryState.Conservation);
        }

    }
}