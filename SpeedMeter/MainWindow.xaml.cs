using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpeedMeter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        private IPv4InterfaceStatistics _interfaceStats;
        private NetworkInterface _networkInterface;
        private long _previousUpload = 0;
        private long _previousDownload = 0;
        private readonly string _registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private readonly string _appName = "SpeedMeterV2";

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            //Startup Location.
            Left = SystemParameters.PrimaryScreenWidth - Width;
            Top = 0;

            //Culture
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = " . ";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            //Adapter with internet.
            GetBestInterface();

            //Timer
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            //Start at startup?
            if (Registry.GetValue($@"{Registry.CurrentUser}\{_registryKey}", _appName, null) == null)
            {
                mnuRunAtStartup.IsChecked = false;
            }
            else
            {
                mnuRunAtStartup.IsChecked = true;
            }
        }

        #endregion

        #region Events

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //_interfaceStats = _networkInterface.GetIPStatistics();
            _interfaceStats = _networkInterface.GetIPv4Statistics();

            //Current values.
            long currentUpload = _interfaceStats.BytesSent;
            long currentDownload = _interfaceStats.BytesReceived;
            lblUploadTotal.Text = SizeSuffix(currentUpload).Item1.ToString();
            lblUploadTotalUnit.Text = $"{SizeSuffix(currentUpload).Item2}";
            lblDownloadTotal.Text = SizeSuffix(currentDownload).Item1.ToString();
            lblDownloadTotalUnit.Text = $"{SizeSuffix(currentDownload).Item2}";

            //Difference.
            long diffUpload = currentUpload - _previousUpload;
            long diffDownload = currentDownload - _previousDownload;
            lblUpload.Text = SizeSuffix(diffUpload).Item1.ToString();
            lblUploadUnit.Text = $"{SizeSuffix(diffUpload).Item2}/s";
            lblDownload.Text = SizeSuffix(diffDownload).Item1.ToString();
            lblDownloadUnit.Text = $"{SizeSuffix(diffDownload).Item2}/s";

            _previousUpload = currentUpload;
            _previousDownload = currentDownload;
        }

        private void mnuRunAtStartup_Checked(object sender, RoutedEventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(_registryKey, true);

            if (mnuRunAtStartup.IsChecked)
            {
                rk.SetValue(_appName, System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                rk.DeleteValue(_appName, false);
            }
        }

        private void mnuTopMost_Checked(object sender, RoutedEventArgs e)
        {
            if (mnuTopMost.IsChecked)
            {
                Topmost = true;
            }
            else
            {
                Topmost = false;
            }
        }

        #endregion

        #region Methods

        private void GetBestInterface()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet && ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
                {
                    _networkInterface = ni;
                    //_interfaceStats = _networkInterface.GetIPStatistics();
                    _interfaceStats = _networkInterface.GetIPv4Statistics();

                    long upload = _interfaceStats.BytesSent;
                    long download = _interfaceStats.BytesReceived;

                    _previousUpload = upload;
                    _previousDownload = download;

                    //lblUploadTotal.Text = upload.ToString();
                    //lblDownloadTotal.Text = download.ToString();

                    break;
                }
            }
        }

        static Tuple<string, string> SizeSuffix(long value, int decimalPlaces = 2)
        {
            Tuple<string, string> tpl = new Tuple<string, string>("0.00", "B");

            string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (value < 0) { return tpl = new Tuple<string, string>("-" + SizeSuffix(-value), "??"); }
            if (value == 0) { return tpl; }
            
            int mag = (int)Math.Log(value, 1024);

            decimal adjustedSize = (decimal)value / (1L << (mag * 10));
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            string decimalFormatString = "#.";
            for(int i = 0; i < decimalPlaces; i++)
            {
                decimalFormatString += "#";
            }

            return tpl = new Tuple<string, string>(adjustedSize.ToString($"{decimalFormatString}"), sizeSuffixes[mag]);
        }

        #endregion
    }
}
