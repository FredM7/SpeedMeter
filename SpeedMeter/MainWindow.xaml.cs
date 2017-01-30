using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

        private static List<Ping> pingers = new List<Ping>();
        private static int instances = 0;
        private static object @lock = new object();
        private static int ttl = 5;
        private static int timeOut = 250;
        private static int result = 0;
        private List<Computers> _lsActiveIPAddresses = new List<Computers>();

        BackgroundWorker _backgroundWorker = new BackgroundWorker();

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            Height = 70;

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
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(_registryKey, true))
            {
                key.SetValue(_appName, System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }
        
        private void mnuRunAtStartup_Unchecked(object sender, RoutedEventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(_registryKey, true))
            {
                key.DeleteValue(_appName, false);
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

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void Ping_completed(object s, PingCompletedEventArgs e)
        {
            lock (@lock)
            {
                instances -= 1;
            }

            if (e.Reply.Status == IPStatus.Success)
            {
                Console.WriteLine(string.Concat("Active IP: ", e.Reply.Address.ToString()));

                Computers comps = new Computers();
                comps.Name = GetMachineNameFromIPAddress(e.Reply.Address.ToString());
                comps.IPAddress = e.Reply.Address.ToString();
                _lsActiveIPAddresses.Add(comps);
                
                result += 1;
            }
            else
            {
                //Console.WriteLine(String.Concat("Non-active IP: ", e.Reply.Address.ToString()))
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _backgroundWorker.RunWorkerAsync();

            //Thread thread = new Thread(GetIPAddresses);
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.IsBackground = true;
            //thread.Start();
            
        }

        private void btnIPAddresses_Checked(object sender, RoutedEventArgs e)
        {
            Height = 210;
        }

        private void btnIPAddresses_Unchecked(object sender, RoutedEventArgs e)
        {
            Height = 70;
        }

        private void btnRefreshIPAddresses_Click(object sender, RoutedEventArgs e)
        {
            btnRefreshIPAddresses.IsEnabled = false;
            _backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetIPAddresses();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //grdIPAddresses.Dispatcher.Invoke((Action)(() => grdIPAddresses.ItemsSource = _lsActiveIPAddresses));
            grdIPAddresses.ItemsSource = _lsActiveIPAddresses;
        }

        #endregion

        #region Methods

        private void GetBestInterface()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up 
                    && ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet 
                    && ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
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

        private void GetIPAddresses()
        {
            result = 0;
            _lsActiveIPAddresses.Clear();

            //Get my current IP. We use this to determine the base IP on the network.
            string baseIP = string.Empty;
            foreach (UnicastIPAddressInformation ip in _networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    baseIP = $"{ip.Address.ToString().Substring(0, ip.Address.ToString().LastIndexOf("."))}.";
                    break;
                }
            }
            
            Console.WriteLine("Pinging 255 destinations of D-class in {0}*", baseIP);

            CreatePingers(255);

            PingOptions po = new PingOptions(ttl, true);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] data = enc.GetBytes("abababababababababababababababab");

            SpinWait wait = new SpinWait();
            int cnt = 1;

            Stopwatch watch = Stopwatch.StartNew();

            foreach (Ping p in pingers)
            {
                lock (@lock)
                {
                    instances += 1;
                }

                p.SendAsync(string.Concat(baseIP, cnt.ToString()), timeOut, data, po);
                cnt += 1;
            }

            while (instances > 0)
            {
                wait.SpinOnce();
            }

            watch.Stop();

            DestroyPingers();

            lblFinishedTime.Dispatcher.Invoke((Action)(() =>
                lblFinishedTime.Text = $" {watch.Elapsed.Milliseconds}ms"
            ));

            lblIPCount.Dispatcher.Invoke((Action)(() =>
                lblIPCount.Text = $" {result}"
            ));

            btnRefreshIPAddresses.Dispatcher.Invoke((Action)(() =>
             btnRefreshIPAddresses.IsEnabled = true
            ));
            //Console.ReadKey();
        }

        private void CreatePingers(int cnt)
        {
            for (int i = 1; i <= cnt; i++)
            {
                Ping p = new Ping();
                p.PingCompleted += Ping_completed;
                pingers.Add(p);
            }
        }

        private void DestroyPingers()
        {
            foreach (Ping p in pingers)
            {
                p.PingCompleted -= Ping_completed;
                p.Dispose();
            }

            pingers.Clear();

        }

        private string GetMachineNameFromIPAddress(string ipAdress)
        {
            string machineName = string.Empty;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAdress);
                machineName = hostEntry.HostName;
            }
            catch (Exception ex)
            {
                // Machine not found...
            }
            return machineName;
        }

        #endregion
    }
}
