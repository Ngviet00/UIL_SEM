using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UILAlignProject.PLC;
using UnilityCommand.Plc.Mitsubishi;
using UnilityCommand.Plc;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Configuration;
using Irony;
using System.Text.RegularExpressions;
using static ICSharpCode.SharpZipLib.Zip.ZipEntryFactory;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using CsvHelper;
using System.Threading;
using DocumentFormat.OpenXml.Drawing.Charts;
using Zebra.Sdk.Printer;
using Zebra.Sdk.Comm;
using System.Drawing.Printing;
using Connection = Zebra.Sdk.Comm.Connection;
using QRCoder;
using Zebra.Sdk.Graphics;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;
using Zebra.Sdk.Printer.Discovery;
using UnilityCommand;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
using Color = System.Drawing.Color;
using NPOI.HSSF.Record.Chart;
using Sunny.UI;
using MathNet.Numerics;
using SixLabors.ImageSharp;
using NPOI.SS.Formula.Functions;
using SixLabors.ImageSharp.ColorSpaces;
using DocumentFormat.OpenXml.EMMA;
using System.Net.Sockets;
using Match = System.Text.RegularExpressions.Match;


namespace CIM
{
    public partial class Form1 : UIForm
    {
        PLCIOCollection pLCIOs = new PLCIOCollection();
        public int indexPLC1, indexPLC2, indexPLC3, indexPLC4;
        public static string PLClog1 = ConfigurationManager.AppSettings["LogPLC1"];
        public static string PLClog2 = ConfigurationManager.AppSettings["LogPLC2"];
        public static string PLClog3 = ConfigurationManager.AppSettings["LogPLC3"];
        public static string PLClog4 = ConfigurationManager.AppSettings["LogPLC4"];
        public static string PLCALL = ConfigurationManager.AppSettings["LogALL"];
        public static string labPath = @"C:\Label\";
        public static string fileName = ConfigurationManager.AppSettings["Labfile"];
        public static int copies = 1;
        //public TCPClientAirMachineHandle tcp;
        public static int excelrow = 1;
        //private readonly string printerIPAddress = "192.168.254.254";
        private ZebraPrinter printer;
        public static string CSV = ConfigurationManager.AppSettings["LogCSV"];
        public static string CSVD = ConfigurationManager.AppSettings["LogCSVD"];

        public System.Data.DataTable dt = new System.Data.DataTable();


        private PieChart pieChartControl;
        private PieSeries okSeries;
        private PieSeries ngSeries;

        public static int OK = 0;
        public static int NG = 0;
        public static int Total;
        public static int No = 0;
        public static List<EXCELDATA> list = new List<EXCELDATA>();
        public static string logFilePath1 = "";
        public static string logFilePath2 = "";
        public static string logFilePath3 = "";
        public static string logFilePath4 = "";
        public static string logFilePathALL = "";

        private ZebraZT411Printer _printerManager;

        //private LabelPrintingService labelPrinter;
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            pieChartControl = new PieChart();
            ResultAirMachine air = new ResultAirMachine();
            _printerManager = new ZebraZT411Printer();

            //labelPrinter = new LabelPrintingService();
            SqlLite.Instance.InitializeConnection();
            //tcp = new TCPClientAirMachineHandle();
#if DEBUG
            if (SingleTonPlcControl.Instance.Connect1())
            {
                WriteLog("Connected to PLC1");
                AddPLCI1(pLCIOs);
                //SingleTonPlcControl.Instance.AddRegisterRead(SingleTonPlcControl.Instance.RegisterRead, pLCIOs);
                //SingleTonPlcControl.Instance.AddRegisterWrite(SingleTonPlcControl.Instance.RegisterWrite, pLCIOs);
                //SingleTonPlcControl.Instance.RegisterRead.PlcIOs.PropertyChanged += RegisterRead_PropertyChanged;
            }
            if (SingleTonPlcControl.Instance.Connect2())
            {
                WriteLog("Connected to PLC2");
                AddPLCI2(pLCIOs);
                //SingleTonPlcControl.Instance.AddRegisterRead(SingleTonPlcControl.Instance.RegisterRead2, pLCIOs);
                //SingleTonPlcControl.Instance.AddRegisterWrite(SingleTonPlcControl.Instance.RegisterWrite2, pLCIOs);
                //SingleTonPlcControl.Instance.RegisterRead2.PlcIOs.PropertyChanged += RegisterRead_PropertyChanged;
            }

            if (SingleTonPlcControl.Instance.Connect3())
            {
                WriteLog("Connected to PLC3");
                AddPLCI3(pLCIOs);
                //SingleTonPlcControl.Instance.AddRegisterRead(SingleTonPlcControl.Instance.RegisterRead3, pLCIOs);
                //SingleTonPlcControl.Instance.AddRegisterWrite(SingleTonPlcControl.Instance.RegisterWrite3, pLCIOs);
                //SingleTonPlcControl.Instance.RegisterRead3.PlcIOs.PropertyChanged += RegisterRead_PropertyChanged;
            }

            if (SingleTonPlcControl.Instance.Connect4())
            {
                WriteLog("Connected to PLC4");
                AddPLCI4(pLCIOs);
                //SingleTonPlcControl.Instance.AddRegisterRead(SingleTonPlcControl.Instance.RegisterRead4, pLCIOs);
                //SingleTonPlcControl.Instance.AddRegisterWrite(SingleTonPlcControl.Instance.RegisterWrite4, pLCIOs);
                //SingleTonPlcControl.Instance.RegisterRead4.PlcIOs.PropertyChanged += RegisterRead_PropertyChanged;
            }
            SingleTonPlcControl.Instance.AddRegisterRead(SingleTonPlcControl.Instance.RegisterRead, pLCIOs);
            SingleTonPlcControl.Instance.AddRegisterWrite(SingleTonPlcControl.Instance.RegisterWrite, pLCIOs);
            SingleTonPlcControl.Instance.RegisterRead.PlcIOs.PropertyChanged += RegisterRead_PropertyChanged;

#else
#endif

        }
        private TcpClient[] clients = new TcpClient[10];
        private NetworkStream[] streams = new NetworkStream[10];
        private bool[] connected = new bool[10];
        //private List<string> serverIPs = new List<string>();
        private readonly string[] serverIPs = new string[]
                {
            "192.168.3.170","192.168.3.171", "192.168.3.172", "192.168.3.173", "192.168.3.174",
            "192.168.3.175", "192.168.3.176","192.168.3.177", "192.168.3.178","192.168.3.179"
                };
        private void ConnectAirTest()
        {
            try
            {

                for (int i = 0; i < 10; i++)
                {
                    int port = 23;
                    string ipAddress = serverIPs[i];
                    clients[i] = new TcpClient();

                    // Đợi kết thúc quá trình kết nối
                    try
                    {
                        ConnectClients(ipAddress, port, i);
                    }
                    catch (TimeoutException ex)  // Xử lý lỗi kết nối timeout ở đây
                    {
                        WriteLog($"Connection timeout: {ex.Message}");
                    }
                    catch (Exception ex) // Xử lý các lỗi khác nếu có
                    {
                        WriteLog($"Error: {ex.Message}");
                    }


                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error: {ex.Message}");
            }
        }
        private void ConnectClients(string ipAddress, int port, int clientIndex)
        {
            Task.Run(() => ConnectClient(clientIndex, ipAddress, port));
        }

        private void ConnectClient(int clientIndex, string ipAddress, int port)
        {
            int attempt = 0;
            while (attempt < 10)
            {
                try
                {
                    clients[clientIndex].Connect(ipAddress, port); // Synchronous connect
                    if (clients[clientIndex].Connected)
                    {
                        streams[clientIndex] = clients[clientIndex].GetStream();
                        connected[clientIndex] = true;
                        WriteLog($"Connected to server {ipAddress} on port {port}.");
                        SendataConnect(clientIndex); // sendata de ket noi toi LEAK Test
                        StartReceiving(clientIndex); // bat dau nhan du lieu
                    }
                }
                catch (SocketException ex)
                {
                    WriteLog($"SocketException for client {clientIndex}: {ex.Message}");
                    //throw; // Rethrow the exception to handle retry logic
                }
                catch (Exception ex)
                {
                    WriteLog($"Connection error for client {clientIndex}: {ex.Message}");
                }
                attempt++;
                Thread.Sleep(100);
            }
        }
        private async void SendataConnect(int connect)
        {
            try
            {
                string message = "1\r\n";
                byte[] data = Encoding.UTF8.GetBytes(message);
                if (connected[connect])
                {
                    await streams[connect].WriteAsync(data, 0, data.Length);
                    WriteLog($"Conect to client {connect + 1}.");
                }
                else
                {
                    WriteLog($"client {connect + 1} is null");
                }

            }
            catch (Exception ex)
            {
                WriteLog($"Error: {ex.Message}");
            }
        }
        private void StartReceiving(int clientIndex)
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (connected[clientIndex])
                {

                    int bytesRead = streams[clientIndex].Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Task.Run(() => ExtractValueSccm(message, clientIndex));
                }
            }
            catch (Exception ex)
            {
                if (connected[clientIndex])
                {
                    WriteLog($"Error: {ex.Message}");
                    connected[clientIndex] = false;
                }
            }
        }
        private void ExtractValueSccm(string input, int clientIndex)
        {

            if (input.Contains("sccm"))
            {
                string patternSccm = @"(\d+\.\d+)\s*sccm";
                Match match = Regex.Match(input, patternSccm);
                if (match.Success)
                {
                    string sccmValue = match.Groups[1].Value;

                    Task.Run(() => HandleReadAirTest(sccmValue, clientIndex));
                }
            }
            else if (input.Contains(" SL "))
            {
                Task.Run(() => HandleReadAirTest("10", clientIndex));

            }
        }
        public static void WriteLog1(int clinetIndex, string logMessage)
        {
            string logFormat = string.Empty;
            string logPath = $"D:\\Logs\\CIM\\";
            logFormat = DateTime.Now.ToLongDateString().ToString() + " - " +
                    DateTime.Now.ToLongTimeString().ToString() + " ==> ";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
                ///NOTE

            }

            try
            {
                //create new log
                using (StreamWriter writer = File.AppendText(logPath + clinetIndex + ".txt"))
                {

                    writer.WriteLine(logFormat + "Client:" + clinetIndex + logMessage);
                }
                //}


            }
            catch (Exception ex)
            {

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //string sourceDirectory = @"D:\Logs\CIM";
            //string destinationDirectory = @"D:\Logs\CIM_bak" + DateTime.Now.ToString("yyyyMMddHHmmss");

            //try
            //{
            //    CopyDirectory(sourceDirectory, destinationDirectory);
            //    DeleteDirectoryContents(sourceDirectory);
            //    MessageBox.Show("Backup completed successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error during backup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }
        private void DeleteDirectoryContents(string sourceDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                subDir.Delete(true);
            }
        }

        private string printerIpAddress = ConfigurationManager.AppSettings["PrinterIP"].ToString();
        private const int DPI = 300;

        private void Print(string Traycode)
        {

            //printby labfile
            //List<LabPrintVar> labVariableList = new List<LabPrintVar>
            //     {
            //    new LabPrintVar { LabVar = "data", Value = Traycode }
            //      };
            //bool printResult = labelPrinter.PrintLabel(labPath, fileName, copies, labVariableList, true);

            //if (printResult)
            //{
            //   WriteLog("Label printed successfully!");
            //}
            //else
            //{
            //    WriteLog("Failed to print label.");
            //}

            //print by command
            int widthMM = 30;
            int heightMM = 30;
            if (_printerManager.Connect(printerIpAddress))
            {
                int widthDots = (int)Math.Round(widthMM * DPI / 25.4);
                int heightDots = (int)Math.Round(heightMM * DPI / 25.4);
                string labelFormat = $"^XA^PW1800 ^LL1200^F0,0^BQN,2,10^FDMA,{Traycode}^FS^XZ";

                //string labelFormat = $"^XA" +
                //                     $"^FO120,50" +
                //                     $"^BQN,3,{heightDots},{widthDots}^FDMA,{QRCode}^FS" +  // ^BQN is for QR code, parameters are: QR code error correction level (2), height in dots, width in dots, data
                //                     $"^XZ";

                _printerManager.PrintLabel(labelFormat);
                _printerManager.Disconnect();
            }
            else
            {
                MessageBox.Show("Không thể kết nối với máy in Zebra qua Ethernet.");
            }
        }






        private void Form1_Load(object sender, EventArgs e)
        {
            string logPath1 = PLClog1;
            string logFileName1 = DateTime.Now.ToString("yyyyMMdd") + ".CSV";
            logFilePath1 = Path.Combine(logPath1, logFileName1);

            //tcp.ConnectAllAsync();//try catch
            if (!Directory.Exists(logPath1))
            {
                Directory.CreateDirectory(logPath1);
            }
            string logPath2 = PLClog2;
            string logFileName2 = DateTime.Now.ToString("yyyyMMdd") + ".CSV";
            logFilePath2 = Path.Combine(logPath2, logFileName2);

            if (!Directory.Exists(logPath2))
            {
                Directory.CreateDirectory(logPath2);
            }
            string logPath3 = PLClog3;
            string logFileName3 = DateTime.Now.ToString("yyyyMMdd") + ".CSV";
            logFilePath3 = Path.Combine(logPath3, logFileName3);

            if (!Directory.Exists(logPath3))
            {
                Directory.CreateDirectory(logPath3);
            }
            string logPath4 = PLClog4;
            string logFileName4 = DateTime.Now.ToString("yyyyMMdd") + ".CSV";
            logFilePath4 = Path.Combine(logPath4, logFileName4);

            if (!Directory.Exists(logPath4))
            {
                Directory.CreateDirectory(logPath4);
            }
            string logPath = PLCALL;
            string logFileName = DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            logFilePathALL = Path.Combine(logPath, logFileName);


            // Ensure the directory exists
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            //if (!Directory.Exists(CSV))
            //{
            //    Directory.CreateDirectory(CSV);
            //}

            ConnectAirTest();

            //Thread readdata = new Thread(() =>
            // {
            //     Readdata();
            // });
            //readdata.IsBackground = true;
            //readdata.Start();
            //Task.Delay(4).Wait();




        }
        //public async void Readdata()
        //{
        //    Task.Delay(5000).Wait();
        //    while (true)
        //    {
        //        ReadData1();
        //        ReadData2();
        //        ReadData3();
        //        ReadData4();

        //    }
        //}


        private async void RegisterRead_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PLCIO obj = sender as PLCIO;

            if (obj != null)
            {
                //var title = obj.Title;
                //if (title.Contains("ReadData"))
                //    Console.WriteLine($"{obj.Title}-{(bool)obj.CurrentValue}");
                //if (obj.Title == "ReadData" && obj.IndexPLC == 1 && (bool)obj.CurrentValue == true)
                if (obj.Title.Contains("ReadData") && (bool)obj.CurrentValue == true)
                {
                    //debug 
                    int indexPLC = obj.IndexPLC;
                    switch (indexPLC)
                    {
                        case 1:
                            ReadData1();
                            SingleTonPlcControl.Instance.SetValueRegister(true, indexPLC, "WriteData", true, EnumReadOrWrite.WRITE);
                            WriteLog("On bit WriteData in PLC 1");
                            break;
                        case 2:
                            ReadData2();
                            SingleTonPlcControl.Instance.SetValueRegister(true, indexPLC, "WriteData", true, EnumReadOrWrite.WRITE);
                            WriteLog("On bit WriteData in PLC 2");
                            break;
                        case 3:
                            ReadData3();
                            SingleTonPlcControl.Instance.SetValueRegister(true, indexPLC, "WriteData", true, EnumReadOrWrite.WRITE);
                            WriteLog("On bit WriteData in PLC 3");
                            break;
                        case 4:
                            ReadData4();
                            SingleTonPlcControl.Instance.SetValueRegister(true, indexPLC, "WriteData", true, EnumReadOrWrite.WRITE);
                            WriteLog("On bit WriteData in PLC 4");
                            break;

                    }


                }
                else if (obj.Title.Contains("ReadData") && (bool)obj.CurrentValue == false)
                {
                    SingleTonPlcControl.Instance.SetValueRegister(false, obj.IndexPLC, "WriteData", true, EnumReadOrWrite.WRITE);
                    WriteLog("OFF bit WriteData in PLC 4");
                }
                else if (obj.Title == "Alive" /*&& (bool)obj.CurrentValue==true */)
                {
                    UpdateStatus(obj.IndexPLC, obj.CurrentValue);


                }
                else if (obj.Title == "ReadPrint" && (bool)obj.CurrentValue == true && obj.IndexPLC == 4)
                {
                    CountPrint(obj.IndexPLC);
                    SingleTonPlcControl.Instance.SetValueRegister(true, obj.IndexPLC, "WritePrint", true, EnumReadOrWrite.WRITE);
                    WriteLog("On bit WritePrint in PLC 4");
                }
                else if (obj.Title == "ReadPrint" && (bool)obj.CurrentValue == false && obj.IndexPLC == 4)
                {
                    SingleTonPlcControl.Instance.SetValueRegister(false, obj.IndexPLC, "WritePrint", true, EnumReadOrWrite.WRITE);
                    WriteLog("OFF bit WritePrint in PLC 4");
                }
                else if (obj.Title == "EndTray" && (bool)obj.CurrentValue == true && obj.IndexPLC == 4)
                {
                    endtray = true;
                    CountPrint(obj.IndexPLC);

                }

            }

        }
        private void UpdateStatus(int indexPLC, object currvalue)
        {


            if (InvokeRequired)
            {
                BeginInvoke((Action)(() => UpdateStatus(indexPLC, currvalue)));
                return;
            }

            bool isConnected = (bool)currvalue;

            Color connectedColor = Color.Blue;
            Color disconnectedColor = Color.Red;

            switch (indexPLC)
            {
                case 1:
                    //lblPLC1.Text = isConnected ? "Connected" : "Disconnected";
                    lblPLC1.CheckBoxColor = isConnected ? connectedColor : disconnectedColor;
                    lblPLC1.ForeColor = isConnected ? connectedColor : disconnectedColor;
                    break;
                case 2:
                    //lblPLC2.Text = isConnected ? "Connected" : "Disconnected";
                    lblPLC2.CheckBoxColor = isConnected ? connectedColor : disconnectedColor;
                    lblPLC2.ForeColor = isConnected ? connectedColor : disconnectedColor;
                    break;
                case 3:
                   // lblPLC3.Text = isConnected ? "Connected" : "Disconnected";
                    lblPLC3.CheckBoxColor = isConnected ? connectedColor : disconnectedColor;
                    lblPLC3.ForeColor = isConnected ? connectedColor : disconnectedColor;
                    break;
                case 4:
                   // lblPLC4.Text = isConnected ? "Connected" : "Disconnected";
                    lblPLC4.CheckBoxColor = isConnected ? connectedColor : disconnectedColor;
                    lblPLC4.ForeColor = isConnected ? connectedColor : disconnectedColor;
                    break;
                default:
                    // Handle unexpected indexPLC values
                    break;
            }
            //switch (indexPLC)
            //{
            //    case 1:
            //        rdPLC1.Checked = isConnected;
            //        rdPLC1.Text = isConnected ? "CONNECTED" : "DISCONNECTED";
            //        rdPLC1.ForeColor = isConnected ? connectedColor : disconnectedColor;

            //        break;
            //    case 2:
            //        rdPLC2.Checked = isConnected;
            //        rdPLC2.Text = isConnected ? "CONNECTED" : "DISCONNECTED";
            //        rdPLC2.ForeColor = isConnected ? connectedColor : disconnectedColor;
            //        break;
            //    case 3:
            //        rdPLC3.Checked = isConnected;
            //        rdPLC3.Text = isConnected ? "CONNECTED" : "DISCONNECTED";
            //        rdPLC3.ForeColor = isConnected ? connectedColor : disconnectedColor;
            //        break;
            //    case 4:
            //        rdPLC4.Checked = isConnected;
            //        rdPLC4.Text = isConnected ? "CONNECTED" : "DISCONNECTED";
            //        rdPLC4.ForeColor = isConnected ? connectedColor : disconnectedColor;
            //        break;
            //    default:
            //        // Handle unexpected indexPLC values
            //        break;
            //}
        }
        private static readonly object lockObject = new object();
        public static void HandleReadAirTest(string sccm, int index)
        {

            float[] a;

            if (string.IsNullOrEmpty(sccm))
            {
                a = new float[1] { 10 };
            }
            else
            {
                a = new float[1] { float.Parse(sccm) };
            }

            switch (index)
            {
                case 0:
                    WriteToPLC("ZR302672", a);
                    WriteLog($"AirTest data:{index}-\"ZR302672\" - {sccm}");
                    break;
                case 1:
                    WriteToPLC("ZR302772", a);
                    WriteLog($"AirTest data:{index}-\"ZR302772\" - {sccm}");
                    break;
                case 2:
                    WriteToPLC("ZR302872", a);
                    WriteLog($"AirTest data:{index}-\"ZR302872\" - {sccm}");
                    break;
                case 3:
                    WriteToPLC("ZR302972", a);
                    WriteLog($"AirTest data:{index}-\"ZR302972\" - {sccm}");
                    break;
                case 4:
                    WriteToPLC("ZR303072", a);
                    WriteLog($"AirTest data:{index}-\"ZR303072\" - {sccm}");
                    break;
                case 5:
                    WriteToPLC("ZR303172", a);
                    WriteLog($"AirTest data:{index}-\"ZR303172\" - {sccm}");
                    break;
                case 6:
                    WriteToPLC("ZR303272", a);
                    WriteLog($"AirTest data:{index}-\"ZR303272\" - {sccm}");
                    break;
                case 7:
                    WriteToPLC("ZR303372", a);
                    WriteLog($"AirTest data:{index}-\"ZR303372\" - {sccm}");
                    break;
                case 8:
                    WriteToPLC("ZR303472", a);
                    WriteLog($"AirTest data:{index}-\"ZR303472\" - {sccm}");
                    break;
                case 9:
                    WriteToPLC("ZR303572", a);
                    WriteLog($"AirTest data:{index}-\"ZR303572\" - {sccm}");
                    break;
            }
        }

        private static void WriteToPLC(string address, float[] data)
        {
            lock (lockObject)
            {
                SingleTonPlcControl.Instance.WriteFloat(address, 1, 4, ref data);
            }
        }

        public bool endtray = false;
        private List<string> print = new List<string>();
        private void CountPrint(int IndexPLC)
        {
            if (SingleTonPlcControl.Instance.GetValueRegister(IndexPLC, "BOX4CountBarcode") == null) return;
            var QRcode = SingleTonPlcControl.Instance.GetValueRegister(IndexPLC, "BOX4CountBarcode").ToString().Substring(0, 23).Trim();
            //endtray = (bool)SingleTonPlcControl.Instance.GetValueRegister(IndexPLC, "EndTray");



            print.Add(QRcode);
            if (endtray)
            {
                var a = GetTraycode(print.Count);
                foreach (var qr in print)
                {
                    SqlLite.Instance.UpdateTrayQRcode(qr, a);

                }
                Print(a);
                print.Clear();
                SingleTonPlcControl.Instance.SetValueRegister(true, IndexPLC, "ReadComplete", true, EnumReadOrWrite.WRITE);
                endtray = false;
            }
            else if (print.Count == 36)
            {
                var a = GetTraycode(print.Count);
                foreach (var qr in print)
                {
                    SqlLite.Instance.UpdateTrayQRcode(qr, a);

                }

                Print(a);
                print.Clear();
                SingleTonPlcControl.Instance.SetValueRegister(true, IndexPLC, "ReadComplete", true, EnumReadOrWrite.WRITE);
            }


        }


#if !DEBUG
        private void ReadData1(string QRcode, string glue_amount, string glue_discharge_volume_vision, string insulator_bar_code, string glue_overflow_vision)
        {
#else
        private void ReadData1()
        {
            if (SingleTonPlcControl.Instance.GetValueRegister(1, "BOX1Barcode") == null) return;
            var QRcode = SingleTonPlcControl.Instance.GetValueRegister(1, "BOX1Barcode").ToString().Substring(0, 23).Trim();

            var glue_amount = SingleTonPlcControl.Instance.GetValueRegister(1, "BOX1GLUE_AMOUNT").ToString().Trim();

            var box1dispenser_status = SingleTonPlcControl.Instance.GetValueRegister(1, "BOX1DISPENSER_STATUS").ToString().Trim();
            var glue_discharge_volume_vision = SingleTonPlcControl.Instance.GetValueRegister(1, "BOX1GLUE_DISCHARGE_VOLUME_VISION").ToString().Trim();
            var insulator_bar_code = SingleTonPlcControl.Instance.GetValueRegister(1, "BOX1INSULATOR_BAR_CODE").ToString().Trim();
            Console.WriteLine(insulator_bar_code);
            var glue_overflow_vision = SingleTonPlcControl.Instance.GetValueRegister(1, "BOX1GLUE_OVERFLOW_VISION").ToString().Trim();
#endif



            string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            WriteLogBox(logFilePath1, $"Serialnumber:{QRcode};1st Glue Amount: {glue_amount}mg ; 1st Glue discharge volume Vision: {glue_discharge_volume_vision} ;Insulator bar code:{insulator_bar_code}; 1st Glue overflow vision: {glue_overflow_vision}; TestTime: {formattedDateTime} ###");
        }
#if !DEBUG
        private void ReadData2(string QRcode, string heated_air_curing, string glue_amount, string glue_discharge_volume_vision, string fpcb_bar_code, string glue_overflow_vision)

        {
#else
        private void ReadData2()
        {

            if (SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2Barcode") == null) return;
            var QRcode = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2Barcode").ToString().Substring(0, 23);

            var heated_air_curing = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2BOX1_HEATED_AIR_CURING").ToString().Trim();
            var heated_air_curing1 = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2BOX1_HEATED_AIR_CURING1").ToString().Trim();
            var heated_air_curing2 = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2BOX1_HEATED_AIR_CURING2").ToString().Trim();
            var heated_air_curing3 = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2BOX1_HEATED_AIR_CURING3").ToString().Trim();
            //var heigh = SingleTonPlcControl.Instance.GetValueRegister(2, "HEIGH").ToString().Trim();

            var box2dispenser_status = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2DISPENSER_STATUS").ToString().Trim();
            var glue_amount = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2GLUE_AMOUNT").ToString().Trim();
            var glue_discharge_volume_vision = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2GLUE_DISCHARGE_VOLUME_VISION").ToString().Trim();
            var fpcb_bar_code = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2FPCB_BAR_CODE").ToString().Trim();
            var glue_overflow_vision = SingleTonPlcControl.Instance.GetValueRegister(2, "BOX2GLUE_OVERFLOW_VISION").ToString().Trim();
#endif



            string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            WriteLogBox(logFilePath2, $"Serialnumber:{QRcode};1ST HEATED AIR CURING:{heated_air_curing}°C,{heated_air_curing1}°C,{heated_air_curing2}°C,{heated_air_curing3}°C ;2nd Glue Amount: {glue_amount}mg ; 2nd Glue discharge volume Vision: {glue_discharge_volume_vision} ;FPCB bar code:{fpcb_bar_code}; 2nd Glue overflow vision: {glue_overflow_vision};TestTime: {formattedDateTime}, ###");



        }
#if !DEBUG
        private void ReadData3(string QRcode, string heated_air_curing, string DISTANCE, string glue_amount, string glue_overflow_vision, string glue_discharge_volume_vision)
        //private void ReadData3()
        {
#else
        private void ReadData3()
        {
            if (SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3Barcode") == null) return;
            var QRcode = SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3Barcode").ToString().Substring(0, 23);

            string glue_overflow_vision = (bool)SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3_GLUE_OVERFLOW_VISION") ? "OK" : "NG";
            var heated_air_curing = SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3BOX2_HEATED_AIR_CURING").ToString().Trim();
            var heated_air_curing1 = SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3BOX2_HEATED_AIR_CURING1").ToString().Trim();
            var heated_air_curing2 = SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3BOX2_HEATED_AIR_CURING2").ToString().Trim();
            var heated_air_curing3 = SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3BOX2_HEATED_AIR_CURING3").ToString().Trim();
            var DISTANCE = SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3DISTANCE").ToString().Trim();
            var glue_amount = SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3GLUE_AMOUNT").ToString().Trim();
            string glue_discharge_volume_vision = (bool)SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3GLUE_DISCHARGE_VOLUME_VISION") ? "OK" : "NG";
#endif



            string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");




            WriteLogBox(logFilePath3, $"Serialnumber:{QRcode}; 2nd heated Air curing:{heated_air_curing}°C,{heated_air_curing1}°C,{heated_air_curing2}°C,{heated_air_curing3}°C ;DISTANCE:{DISTANCE}mm ;3ND Glue Amount: {glue_amount}mg ; 3ND Glue discharge volume Vision: {glue_discharge_volume_vision};3ND Glue overflow vision: {glue_overflow_vision} ;TestTime: {formattedDateTime}, ###");


        }

#if !DEBUG
        private async void ReadData4(string QRcode, string heated_air_curing,string heated_air_curing1,string heated_air_curing2,string heated_air_curing3, string tightness_and_location_vision, string height_parallelism_result, string height_parallelism_detail, string resistance, string air_leakage_test_result, string air_leakage_test_detail)
        
        {


#else
        private async void ReadData4()
        {

            if (SingleTonPlcControl.Instance.GetValueRegister(3, "BOX3Barcode") == null) return;
            var QRcode = SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4Barcode").ToString().Trim();

            var heated_air_curing = SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4BOX3_HEATED_AIR_CURING").ToString().Trim();
            var heated_air_curing1 = SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4BOX3_HEATED_AIR_CURING1").ToString().Trim();
            var heated_air_curing2 = SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4BOX3_HEATED_AIR_CURING2").ToString().Trim();
            var heated_air_curing3 = SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4BOX3_HEATED_AIR_CURING3").ToString().Trim();
            //var glue_amount = SingleTonPlcControl.Instance.GetValueRegister(4, "GLUE_AMOUNT").ToString().Trim();
            string tightness_and_location_vision = (bool)SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4TIGHTNESS_AND_LOCATION_VISION") ? "OK" : "NG";
            string height_parallelism_result = (bool)SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4HEIGHT_PARALLELISM_RESULT") ? "OK" : "NG";
            var height_parallelism_detail = SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4HEIGHT_PARALLELISM_DETAIL");
            var resistance = SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4resistance").ToString().Trim();
            //var location = SingleTonPlcControl.Instance.GetValueRegister(4, "location");
            var air_leakage_test_detail = SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4AIR_LEAKAGE_TEST_DETAIL").ToString().Trim();

            string air_leakage_test_result = (bool)SingleTonPlcControl.Instance.GetValueRegister(4, "BOX4AIR_LEAKAGE_TEST_RESULT") ? "OK" : "NG";
            WriteLog($"BOX4AIR_LEAKAGE_TEST_DETAIL:{air_leakage_test_detail}+ BOX4AIR_LEAKAGE_TEST_RESULT:{air_leakage_test_result} ");
#endif


            string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");




            if (!string.IsNullOrEmpty(QRcode.ToString().Trim()) && QRcode != "False")
            {
                //su dung sql khong can add list (list phuc vu tim kiem)
                //var havedata = list.Where(r => r.TOPHOUSING == QRcode.Trim()).ToList();
                //if (havedata != null && havedata.Count() > 0)
                //{
                //    WriteLog(QRcode + "da ton tai");
                //    return;
                //}



                WriteLogBox(logFilePath4, $"Serialnumber:{QRcode};3ND HEATED AIR CURING:{heated_air_curing}°C,{heated_air_curing1}°C,{heated_air_curing2}°C,{heated_air_curing3}°C  ; TIGHTNESS AND LOCATION VISION: {tightness_and_location_vision} ; HEIGHT PARALLELISM: {height_parallelism_detail}/{height_parallelism_result} ; resistance:{resistance}Ω;air leakage test result: {air_leakage_test_result}; air leakage test detail: {air_leakage_test_detail} SCCM;TestTime: {formattedDateTime}; ###");

                List<string> Box1results = ReadFilesAndSearch(PLClog1, QRcode.ToString());
                List<string> Box2results = ReadFilesAndSearch(PLClog2, QRcode.ToString());
                List<string> Box3results = ReadFilesAndSearch(PLClog3, QRcode.ToString());
                List<string> Box4results = ReadFilesAndSearch(PLClog4, QRcode.ToString());
                string lastrowdata1 = Box1results.LastOrDefault();
                string lastrowdata2 = Box2results.LastOrDefault();
                string lastrowdata3 = Box3results.LastOrDefault();
                string lastrowdata4 = Box4results.LastOrDefault();
                BOX1RESULT box1data = new BOX1RESULT();
                BOX2RESULT box2data = new BOX2RESULT();
                BOX3RESULT box3data = new BOX3RESULT();
                BOX4RESULT box4data = new BOX4RESULT();
                if (!string.IsNullOrEmpty(lastrowdata1))
                {
                    box1data = SpiltData1(lastrowdata1);
                }
                if (!string.IsNullOrEmpty(lastrowdata2))
                {
                    box2data = SpiltData2(lastrowdata2);
                }
                if (!string.IsNullOrEmpty(lastrowdata3))
                {
                    box3data = SpiltData3(lastrowdata3);
                }
                if (!string.IsNullOrEmpty(lastrowdata4))
                {
                    box4data = SpiltData4(lastrowdata4);
                }
                EXCELDATA data1 = new EXCELDATA
                {
                    NO = No,
                    TOPHOUSING = box4data.TOPHOUSING,
                    BOX1_GLUE_AMOUNT = box1data.GLUE_AMOUNT,
                    BOX1_GLUE_DISCHARGE_VOLUME_VISION = box1data.GLUE_DISCHARGE_VOLUME_VISION,
                    INSULATOR_BAR_CODE = box1data.INSULATOR_BAR_CODE,
                    BOX1_GLUE_OVERFLOW_VISION = box1data.GLUE_OVERFLOW_VISION,
                    BOX2_GLUE_AMOUNT = box2data.GLUE_AMOUNT,
                    BOX2_GLUE_DISCHARGE_VOLUME_VISION = box2data.GLUE_DISCHARGE_VOLUME_VISION,
                    FPCB_BAR_CODE = box2data.FPCB_BAR_CODE,
                    BOX2_GLUE_OVERFLOW_VISION = box2data.GLUE_OVERFLOW_VISION,
                    BOX1_HEATED_AIR_CURING = box2data.BOX1_HEATED_AIR_CURING,
                    BOX2_HEATED_AIR_CURING = box3data.BOX2_HEATED_AIR_CURING,
                    BOX3_DISTANCE = box3data.DISTANCE,
                    BOX3_GLUE_AMOUNT = box3data.GLUE_AMOUNT,
                    BOX3_GLUE_DISCHARGE_VOLUME_VISION = box3data.GLUE_DISCHARGE_VOLUME_VISION,
                    BOX3_GLUE_OVERFLOW_VISION = box3data.GLUE_DISCHARGE_VOLUME_VISION,
                    BOX4_AIR_LEAKAGE_TEST_DETAIL = box4data.AIR_LEAKAGE_TEST_DETAIL,
                    BOX3_HEATED_AIR_CURING = box4data.BOX3_HEATED_AIR_CURING,
                    BOX4_TIGHTNESS_AND_LOCATION_VISION = box4data.TIGHTNESS_AND_LOCATION_VISION,
                    BOX4_HEIGHT_PARALLELISM = box4data.HEIGHT_PARALLELISM,
                    BOX4_RESISTANCE = box4data.RESISTANCE,
                    BOX4_AIR_LEAKAGE_TEST_RESULT = box4data.AIR_LEAKAGE_TEST_RESULT,
                    BOX4_TestTime = DateTime.Now//box4data.TestTime
                };
                //su dung sql khong can add list (list phuc vu tim kiem)
                list.Add(data1);
                No++;

                Action gridviewaction = () =>
                {
                    dataGridView1.Rows.Add(No, data1.TOPHOUSING, data1.BOX1_GLUE_AMOUNT, data1.BOX1_GLUE_DISCHARGE_VOLUME_VISION, data1.INSULATOR_BAR_CODE, data1.BOX1_GLUE_OVERFLOW_VISION, data1.BOX1_HEATED_AIR_CURING, data1.BOX2_GLUE_AMOUNT, data1.BOX2_GLUE_DISCHARGE_VOLUME_VISION, data1.FPCB_BAR_CODE, data1.BOX2_GLUE_OVERFLOW_VISION, data1.BOX2_HEATED_AIR_CURING, data1.BOX3_DISTANCE, data1.BOX3_GLUE_AMOUNT, data1.BOX3_GLUE_DISCHARGE_VOLUME_VISION, data1.BOX3_HEATED_AIR_CURING, data1.BOX3_GLUE_OVERFLOW_VISION, data1.BOX4_TIGHTNESS_AND_LOCATION_VISION, data1.BOX4_HEIGHT_PARALLELISM, data1.BOX4_RESISTANCE, data1.BOX4_AIR_LEAKAGE_TEST_DETAIL, data1.BOX4_AIR_LEAKAGE_TEST_RESULT, formattedDateTime);
                    dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);

                };

                if (this.InvokeRequired)
                    this.Invoke(gridviewaction);
                else
                    gridviewaction();

                string pathcsvE = "";
                string pathcsvD = "";
                if (!SqlLite.Instance.CheckQRcode(QRcode))
                {

                    Color alertColor = ColorTranslator.FromHtml("#FFCCC7");

                    Action showLabelAction = () =>
                    {

                        lblalert.Enabled = true;
                        lblalert.BackColor = alertColor;
                        lblalert.ForeColor = Color.Black;
                        lblalert.Text = "BARCODE : " + QRcode + "\t Doublicated !!!";
                        lblalert.Visible = true;
                        MessageTimer.Start();
                    };

                    if (lblalert.InvokeRequired)
                    {
                        lblalert.Invoke(showLabelAction);
                    }
                    else
                    {
                        showLabelAction();
                    }


                    int rowIndex = dataGridView1.Rows.Count - 1;
                    dataGridView1.Rows[0].DefaultCellStyle.ForeColor = Color.Red;
                    SqlLite.Instance.InsertSEM_DATA(data1, "Doublicate");
                    pathcsvE = Path.Combine(CSV, data1.TOPHOUSING + "_d.CSV");
                    if (File.Exists(pathcsvE))
                    {
                        pathcsvE = Path.Combine(CSV, data1.TOPHOUSING + $"_d_{DateTime.Now.ToString("yyyyMMddHHmmss")}.CSV");

                    }


                    pathcsvD = Path.Combine(CSVD, data1.TOPHOUSING + "_d.CSV");
                    if (File.Exists(pathcsvD))
                    {
                        pathcsvD = Path.Combine(CSVD, data1.TOPHOUSING + $"_d_{DateTime.Now.ToString("yyyyMMddHHmmss")}.CSV");

                    }




                    CreateExcelFile(logFilePathALL, box1data, box2data, box3data, box4data, excelrow, true);
                }
                else
                {

                    if (box4data.AIR_LEAKAGE_TEST_RESULT.Trim() == "OK")
                    {
                        OK++;
                        Total++;
                    }
                    else if (box4data.AIR_LEAKAGE_TEST_RESULT.Trim() == "NG")
                    {
                        NG++;
                        Total++;
                    }
                    pieChart1.UpdateChartData(OK, NG);

                    SqlLite.Instance.InsertSEM_DATA(data1);
                    UpdateUI(data1);

                    pathcsvE = Path.Combine(CSV, data1.TOPHOUSING + ".CSV");
                    pathcsvD = Path.Combine(CSVD, data1.TOPHOUSING + ".CSV");
                    CreateExcelFile(logFilePathALL, box1data, box2data, box3data, box4data, excelrow, false);
                }

                string csv_header = "Top Housing QR코드,1st Glue Amount,1st  Glue discharge volume Vision,Insulator bar code,1st Glue overflow vision,1st heated Air curing,2nd Glue Amount,2nd  Glue discharge volume Vision ,FPCB bar code,2nd Glue overflow vision,2nd heated Air curing,동심도,3rd Glue Amount,3rd  Glue discharge volume Vision,3rd heated Air curing,3rd Glue overflow vision,Tightness and location vision,Height / Parallelism,저항값,Air Leakage Test ,생산일자,생산시간";
                //if (Directory.Exists(pathcsv)) { pathcsv = Path.Combine(CSV, data1.TOPHOUSING + data1.BOX4_TestTime + ".CSV"); }



                //\"{box4data.BOX3_HEATED_AIR_CURING}\"
                string csvString = $"{data1.TOPHOUSING},{box1data.GLUE_AMOUNT},{box1data.GLUE_DISCHARGE_VOLUME_VISION},{box1data.INSULATOR_BAR_CODE},{box1data.GLUE_OVERFLOW_VISION},\"{box2data.BOX1_HEATED_AIR_CURING}\","
                            + $"{box2data.GLUE_AMOUNT},{box2data.GLUE_DISCHARGE_VOLUME_VISION},{box2data.FPCB_BAR_CODE},{box2data.GLUE_OVERFLOW_VISION},\"{box3data.BOX2_HEATED_AIR_CURING}\",{box3data.DISTANCE},"
                            + $"{box3data.GLUE_AMOUNT},{box3data.GLUE_DISCHARGE_VOLUME_VISION},\"{box4data.BOX3_HEATED_AIR_CURING}\",{box3data.BOX3_GLUE_OVERFLOW_VISION},"
                            + $"{box4data.TIGHTNESS_AND_LOCATION_VISION},{box4data.HEIGHT_PARALLELISM},{box4data.RESISTANCE},{box4data.AIR_LEAKAGE_TEST_DETAIL},{box4data.TestTime.Substring(0, 10)},{box4data.TestTime.Substring(10)}";


                //WriteLogBox(pathcsvE, csv_header);
                //WriteLogBox(pathcsvE, csvString);
                //WriteLogBox(pathcsvD, csv_header);
                //WriteLogBox(pathcsvD, csvString);
                CreateCsvFile(pathcsvD, data1);
                CreateCsvFile(pathcsvE, data1);












            }

        }


        public void UpdateUI(EXCELDATA data1)
        {
            Action action = () =>
            {
                lbltotal.Text = Total.ToString();
                lblOK.Text = OK.ToString();
                lblNG.Text = NG.ToString();
                double percentOK = Math.Round(((double)OK / Total) * 100, 2);
                var percenNG = Math.Round(100 - percentOK, 2);
                lblperOK.Text = percentOK.ToString() + "%";
                lblperNG.Text = percenNG.ToString() + "%";

            };

            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();


        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == 3 || e.ColumnIndex == 5 || e.ColumnIndex == 8 || e.ColumnIndex == 10 || e.ColumnIndex == 14 || e.ColumnIndex == 16 || e.ColumnIndex == 17 || e.ColumnIndex == 21) && e.Value != null)
            {
                string cellValue = e.Value.ToString().Trim();

                if (cellValue.ToUpper() == "NG")
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Red;
                }
                else if (cellValue.ToUpper() == "OK")
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Green;
                }
            }
        }


        //DOC GHI VAO plc
        public void AddPLCI1(PLCIOCollection pLCIOs)
        {
            //Read

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 40000, "Alive", EnumRegisterType.BITINWORD, 15, true, true, 1));

            //interger 16 : Word ; interger 32 doubleword; 

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 34000, "ReadData", EnumRegisterType.BIT, 1, true, true, 1));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45100, "BOX1Barcode", EnumRegisterType.STRING, 27, true, false, 1));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45128, "BOX1DISPENSER_STATUS", EnumRegisterType.STRING, 2, true, false, 1));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45130, "BOX1GLUE_AMOUNT", EnumRegisterType.DWORD, 2, true, false, 1));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45138, "BOX1GLUE_DISCHARGE_VOLUME_VISION", EnumRegisterType.STRING, 2, true, false, 1));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45146, "BOX1INSULATOR_BAR_CODE", EnumRegisterType.STRING, 36, true, false, 1));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45182, "BOX1GLUE_OVERFLOW_VISION", EnumRegisterType.STRING, 2, true, false, 1));


            //note
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 10010, "HEATED_AIR_CURING", EnumRegisterType.STRING, 20, true, false, 1));


            //Write

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 34100, "WriteData", EnumRegisterType.BIT, 1, true, true, 1)); // On bit doc du lieu PLC  off khi plc off


        }
        public void AddPLCI2(PLCIOCollection pLCIOs)
        {
            //Read
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 40000, "Alive", EnumRegisterType.BITINWORD, 15, true, true, 2));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 34100, "WriteData", EnumRegisterType.BIT, 1, true, true, 2));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 34000, "ReadData", EnumRegisterType.BIT, 1, true, true, 2));//BIT => M34000

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45100, "BOX2Barcode", EnumRegisterType.STRING, 27, true, false, 2));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45128, "BOX2DISPENSER_STATUS", EnumRegisterType.STRING, 2, true, false, 2));




            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45197, "BOX2BOX1_HEATED_AIR_CURING", EnumRegisterType.WORD, 1, true, false, 2));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45198, "BOX2BOX1_HEATED_AIR_CURING1", EnumRegisterType.WORD, 1, true, false, 2));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45199, "BOX2BOX1_HEATED_AIR_CURING2", EnumRegisterType.WORD, 1, true, false, 2));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45200, "BOX2BOX1_HEATED_AIR_CURING3", EnumRegisterType.WORD, 1, true, false, 2));


            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45130, "BOX2GLUE_AMOUNT", EnumRegisterType.DWORD, 2, true, false, 2));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45138, "BOX2GLUE_DISCHARGE_VOLUME_VISION", EnumRegisterType.STRING, 2, true, false, 2));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45146, "BOX2FPCB_BAR_CODE", EnumRegisterType.STRING, 36, true, false, 2));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45182, "BOX2GLUE_OVERFLOW_VISION", EnumRegisterType.STRING, 2, true, false, 2));





            //Write
            // pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 34100, "BOX2WriteData", EnumRegisterType.BIT, 1, true, true, 2));




            //Write
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogX1+", EnumRegisterType.BITINWORD, 0, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogX1-", EnumRegisterType.BITINWORD, 1, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogY1+", EnumRegisterType.BITINWORD, 2, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogY1-", EnumRegisterType.BITINWORD, 3, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogZ1+", EnumRegisterType.BITINWORD, 4, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogZ1-", EnumRegisterType.BITINWORD, 5, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogR1+", EnumRegisterType.BITINWORD, 6, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogR1-", EnumRegisterType.BITINWORD, 7, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogX2+", EnumRegisterType.BITINWORD, 8, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogX2-", EnumRegisterType.BITINWORD, 9, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogY2+", EnumRegisterType.BITINWORD, 10, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogY2-", EnumRegisterType.BITINWORD, 11, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogZ2+", EnumRegisterType.BITINWORD, 12, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogZ2-", EnumRegisterType.BITINWORD, 13, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogR2+", EnumRegisterType.BITINWORD, 14, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogR2-", EnumRegisterType.BITINWORD, 15, false));


            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 150, "X1", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 152, "Y1", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 154, "Z1", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 156, "R1", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 158, "X2", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 160, "Y2", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 162, "Z2", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 164, "R2", EnumRegisterType.DWORD, 1, false, false));
        }
        public void AddPLCI3(PLCIOCollection pLCIOs)
        {
            //Read
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 40000, "Alive", EnumRegisterType.BITINWORD, 15, true, true, 3));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 34100, "WriteData", EnumRegisterType.BIT, 1, true, true, 3));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 34000, "ReadData", EnumRegisterType.BIT, 1, true, true, 3));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45100, "BOX3Barcode", EnumRegisterType.STRING, 28, true, false, 3));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45160, "BOX3BOX2_HEATED_AIR_CURING", EnumRegisterType.WORD, 1, true, false, 3));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45161, "BOX3BOX2_HEATED_AIR_CURING1", EnumRegisterType.WORD, 1, true, false, 3));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45162, "BOX3BOX2_HEATED_AIR_CURING2", EnumRegisterType.WORD, 1, true, false, 3));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45163, "BOX3BOX2_HEATED_AIR_CURING3", EnumRegisterType.WORD, 1, true, false, 3));

            //note
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45130, "BOX3DISTANCE", EnumRegisterType.FLOAT, 8, true, false, 3));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45140, "BOX3GLUE_AMOUNT", EnumRegisterType.DWORD, 2, true, false, 3));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45128, "BOX3GLUE_DISCHARGE_VOLUME_VISION", EnumRegisterType.BITINWORD, 0, true, false, 3));


            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45146, "BOX3_GLUE_OVERFLOW_VISION", EnumRegisterType.BITINWORD, 0, true, false, 3));




            //RE
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 34100, "BOX3WriteData", EnumRegisterType.BIT, 1, true, true, 3));



            //Write
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogX1+", EnumRegisterType.BITINWORD, 0, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogX1-", EnumRegisterType.BITINWORD, 1, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogY1+", EnumRegisterType.BITINWORD, 2, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogY1-", EnumRegisterType.BITINWORD, 3, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogZ1+", EnumRegisterType.BITINWORD, 4, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogZ1-", EnumRegisterType.BITINWORD, 5, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogR1+", EnumRegisterType.BITINWORD, 6, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogR1-", EnumRegisterType.BITINWORD, 7, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogX2+", EnumRegisterType.BITINWORD, 8, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogX2-", EnumRegisterType.BITINWORD, 9, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogY2+", EnumRegisterType.BITINWORD, 10, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogY2-", EnumRegisterType.BITINWORD, 11, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogZ2+", EnumRegisterType.BITINWORD, 12, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogZ2-", EnumRegisterType.BITINWORD, 13, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogR2+", EnumRegisterType.BITINWORD, 14, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 11, "JogR2-", EnumRegisterType.BITINWORD, 15, false));


            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 150, "X1", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 152, "Y1", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 154, "Z1", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 156, "R1", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 158, "X2", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 160, "Y2", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 162, "Z2", EnumRegisterType.DWORD, 1, false, false));
            //pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 164, "R2", EnumRegisterType.DWORD, 1, false, false));
        }
        public void AddPLCI4(PLCIOCollection pLCIOs)
        {
            //Read
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 40000, "Alive", EnumRegisterType.BITINWORD, 15, true, true, 4));


            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 34000, "ReadData", EnumRegisterType.BIT, 1, true, true, 4));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 34100, "WriteData", EnumRegisterType.BIT, 1, true, false, 4));


            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45100, "BOX4Barcode", EnumRegisterType.STRING, 28, true, false, 4));
            //note
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45180, "BOX4BOX3_HEATED_AIR_CURING", EnumRegisterType.WORD, 1, true, false, 4));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45181, "BOX4BOX3_HEATED_AIR_CURING1", EnumRegisterType.WORD, 1, true, false, 4));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45182, "BOX4BOX3_HEATED_AIR_CURING2", EnumRegisterType.WORD, 1, true, false, 4));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45183, "BOX4BOX3_HEATED_AIR_CURING3", EnumRegisterType.WORD, 1, true, false, 4));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45128, "BOX4TIGHTNESS_AND_LOCATION_VISION", EnumRegisterType.BITINWORD, 0, true, false, 4));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45140, "BOX4HEIGHT_PARALLELISM_RESULT", EnumRegisterType.BITINWORD, 0, true, false, 4));
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45142, "BOX4HEIGHT_PARALLELISM_DETAIL", EnumRegisterType.FLOAT, 2, true, false, 4));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45152, "BOX4resistance", EnumRegisterType.FLOAT, 8, true, false, 4));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45160, "BOX4AIR_LEAKAGE_TEST_RESULT", EnumRegisterType.BITINWORD, 0, true, false, 4));

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45162, "BOX4AIR_LEAKAGE_TEST_DETAIL", EnumRegisterType.FLOAT, 2, true, false, 4));

            ////////
            ///

            //printer 
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 34002, "ReadPrint", EnumRegisterType.BIT, 1, true, true, 4)); // doc qr dem de in tem
            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 45400, "BOX4CountBarcode", EnumRegisterType.STRING, 28, true, false, 4)); // lay qr code

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 34102, "WritePrint", EnumRegisterType.BIT, 1, true, false, 4));//ReadPrint xong 1 con thi on WritePrint

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.WRITE, 34103, "ReadComplete", EnumRegisterType.BIT, 1, true, false, 4)); // count du 36 thi on ReadComplete

            pLCIOs.Add(new PLCIO(EnumReadOrWrite.READ, 34004, "EndTray", EnumRegisterType.BIT, 1, true, true, 4));// end tray khi so luong chua du 36 hoac da in tem xong



        }

        //public void CreateExcelFile(string path, BOX1RESULT box1Data, BOX2RESULT box2Data, BOX3RESULT box3Data, BOX4RESULT box4Data, int currentRow)
        //{
        //    string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");



        //    using (var package = new ExcelPackage(new FileInfo(path)))
        //    {
        //        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //        if (worksheet == null)
        //        {
        //            worksheet = package.Workbook.Worksheets.Add("Results");

        //            worksheet.Cells["A1:X1"].Merge = true;
        //            worksheet.Cells["A1"].Value = "실제 작성시 추가/삭제 되는 항목 Update 요청 드립니다.";
        //            worksheet.Cells["T2:U2"].Merge = true;
        //            worksheet.Cells["T2"].Value = "Air Leakage Test";

        //            string[] headers = {
        //        "Top Housing QR코드", "1st Glue Amount", "1st  Glue discharge volume Vision", "Insulator bar code",
        //        "1st Glue overflow vision", "1st heated Air curing", "2nd Glue Amount", "2nd  Glue discharge volume Vision",
        //        "FPCB bar code", "2nd Glue overflow vision", "2nd heated Air curing", "동심도", "3rd Glue Amount",
        //        "3rd  Glue discharge volume Vision", "3rd heated Air curing", "3rd Glue overflow vision",
        //        "Tightness and location vision", "Height / Parallelism", "저항값", "Air Leakage Test", "Air Leakage Test Result", "생산일자", "생산일자"
        //    };

        //            for (int i = 0; i < headers.Length; i++)
        //            {
        //                worksheet.Cells[2, i + 1].Value = headers[i];
        //            }

        //            package.Save();
        //        }

        //        int rowIndex = currentRow + 2;
        //        worksheet.Cells[rowIndex, 1].Value = box4Data.TOPHOUSING;
        //        worksheet.Cells[rowIndex, 2].Value = box1Data.GLUE_AMOUNT;
        //        worksheet.Cells[rowIndex, 3].Value = box1Data.GLUE_DISCHARGE_VOLUME_VISION;
        //        worksheet.Cells[rowIndex, 4].Value = box1Data.INSULATOR_BAR_CODE;
        //        worksheet.Cells[rowIndex, 5].Value = box1Data.GLUE_OVERFLOW_VISION;
        //        worksheet.Cells[rowIndex, 6].Value = box2Data.BOX1_HEATED_AIR_CURING;
        //        worksheet.Cells[rowIndex, 7].Value = box2Data.GLUE_AMOUNT;
        //        worksheet.Cells[rowIndex, 8].Value = box2Data.GLUE_DISCHARGE_VOLUME_VISION;
        //        worksheet.Cells[rowIndex, 9].Value = box2Data.FPCB_BAR_CODE;
        //        worksheet.Cells[rowIndex, 10].Value = box2Data.GLUE_OVERFLOW_VISION;
        //        worksheet.Cells[rowIndex, 11].Value = box3Data.BOX2_HEATED_AIR_CURING;
        //        worksheet.Cells[rowIndex, 12].Value = box3Data.DISTANCE;
        //        worksheet.Cells[rowIndex, 13].Value = box3Data.GLUE_AMOUNT;
        //        worksheet.Cells[rowIndex, 14].Value = box3Data.GLUE_DISCHARGE_VOLUME_VISION;
        //        worksheet.Cells[rowIndex, 15].Value = box4Data.BOX3_HEATED_AIR_CURING;
        //        worksheet.Cells[rowIndex, 16].Value = box3Data.BOX3_GLUE_OVERFLOW_VISION;
        //        worksheet.Cells[rowIndex, 17].Value = box4Data.TIGHTNESS_AND_LOCATION_VISION;
        //        worksheet.Cells[rowIndex, 18].Value = box4Data.HEIGHT_PARALLELISM;
        //        worksheet.Cells[rowIndex, 19].Value = box4Data.RESISTANCE;
        //        worksheet.Cells[rowIndex, 20].Value = box4Data.AIR_LEAKAGE_TEST_DETAIL;
        //        worksheet.Cells[rowIndex, 21].Value = box4Data.AIR_LEAKAGE_TEST_RESULT;

        //        worksheet.Cells[rowIndex, 22].Value = formattedDateTime.Substring(0, 10);
        //        worksheet.Cells[rowIndex, 23].Value = formattedDateTime.Substring(11);
        //        //worksheet.Cells[rowIndex, 22].Value = box4Data.TestTime.Substring(0, 10);
        //        //worksheet.Cells[rowIndex, 23].Value = box4Data.TestTime.Substring(11);

        //        package.Save();
        //        excelrow++;
        //    }
        //}

        public void CreateExcelFile(string path, BOX1RESULT box1Data, BOX2RESULT box2Data, BOX3RESULT box3Data, BOX4RESULT box4Data, int currentRow, bool doublicate)
        {
            string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    worksheet = package.Workbook.Worksheets.Add("Results");

                    worksheet.Cells["A1:X1"].Merge = true;
                    worksheet.Cells["A1"].Value = "실제 작성시 추가/삭제 되는 항목 Update 요청 드립니다.";
                    worksheet.Cells["T2:U2"].Merge = true;
                    worksheet.Cells["T2"].Value = "Air Leakage Test";

                    string[] headers = {
                "Top Housing QR코드", "1st Glue Amount", "1st  Glue discharge volume Vision", "Insulator bar code",
                "1st Glue overflow vision", "1st heated Air curing", "2nd Glue Amount", "2nd  Glue discharge volume Vision",
                "FPCB bar code", "2nd Glue overflow vision", "2nd heated Air curing", "동심도", "3rd Glue Amount",
                "3rd  Glue discharge volume Vision", "3rd heated Air curing", "3rd Glue overflow vision",
                "Tightness and location vision", "Height / Parallelism", "저항값", "Air Leakage Test", "Air Leakage Test Result", "생산일자", "생산일자"
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[2, i + 1].Value = headers[i];
                    }

                    package.Save();
                }

                int rowIndex = currentRow + 2;
                worksheet.Cells[rowIndex, 1].Value = box4Data.TOPHOUSING;
                worksheet.Cells[rowIndex, 2].Value = box1Data.GLUE_AMOUNT;
                worksheet.Cells[rowIndex, 3].Value = box1Data.GLUE_DISCHARGE_VOLUME_VISION;
                worksheet.Cells[rowIndex, 4].Value = box1Data.INSULATOR_BAR_CODE;
                worksheet.Cells[rowIndex, 5].Value = box1Data.GLUE_OVERFLOW_VISION;
                worksheet.Cells[rowIndex, 6].Value = box2Data.BOX1_HEATED_AIR_CURING;
                worksheet.Cells[rowIndex, 7].Value = box2Data.GLUE_AMOUNT;
                worksheet.Cells[rowIndex, 8].Value = box2Data.GLUE_DISCHARGE_VOLUME_VISION;
                worksheet.Cells[rowIndex, 9].Value = box2Data.FPCB_BAR_CODE;
                worksheet.Cells[rowIndex, 10].Value = box2Data.GLUE_OVERFLOW_VISION;
                worksheet.Cells[rowIndex, 11].Value = box3Data.BOX2_HEATED_AIR_CURING;
                worksheet.Cells[rowIndex, 12].Value = box3Data.DISTANCE;
                worksheet.Cells[rowIndex, 13].Value = box3Data.GLUE_AMOUNT;
                worksheet.Cells[rowIndex, 14].Value = box3Data.GLUE_DISCHARGE_VOLUME_VISION;
                worksheet.Cells[rowIndex, 15].Value = box4Data.BOX3_HEATED_AIR_CURING;
                worksheet.Cells[rowIndex, 16].Value = box3Data.BOX3_GLUE_OVERFLOW_VISION;
                worksheet.Cells[rowIndex, 17].Value = box4Data.TIGHTNESS_AND_LOCATION_VISION;
                worksheet.Cells[rowIndex, 18].Value = box4Data.HEIGHT_PARALLELISM;
                worksheet.Cells[rowIndex, 19].Value = box4Data.RESISTANCE;
                worksheet.Cells[rowIndex, 20].Value = box4Data.AIR_LEAKAGE_TEST_DETAIL;
                worksheet.Cells[rowIndex, 21].Value = box4Data.AIR_LEAKAGE_TEST_RESULT;
                worksheet.Cells[rowIndex, 22].Value = formattedDateTime.Substring(0, 10);
                worksheet.Cells[rowIndex, 23].Value = formattedDateTime.Substring(11);

                if (doublicate)
                {
                    worksheet.Row(rowIndex).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Row(rowIndex).Style.Fill.BackgroundColor.SetColor(Color.Red);
                }

                package.Save();
                excelrow++;
            }
        }

        public void CreateCsvFile(string path, EXCELDATA data1)
        {
            string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            bool fileExists = File.Exists(path);
            using (var writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                if (!fileExists)
                {
                    string[] headers = {
    "Top Housing QRCode", "1st Glue Amount", "1st Glue discharge volume Vision", "Insulator bar code",
    "1st Glue overflow vision", "1st heated Air curing", "2nd Glue Amount", "2nd Glue discharge volume Vision",
    "FPCB bar code", "2nd Glue overflow vision", "2nd heated Air curing", "DISTANCE", "3rd Glue Amount",
    "3rd Glue discharge volume Vision", "3rd heated Air curing", "3rd Glue overflow vision",
    "Tightness and location vision", "Height / Parallelism", "RESISTANCE", "Air Leakage Test","Air Leakage Test", "Product Day", "Product Time"
};
                    writer.WriteLine(string.Join(",", headers));
                }

                //string airLeakageTestAndResult = $"{data1.BOX4_AIR_LEAKAGE_TEST_DETAIL}, {data1.BOX4_AIR_LEAKAGE_TEST_RESULT}";

                string[] data = {
            data1.TOPHOUSING,
            data1.BOX1_GLUE_AMOUNT,
            data1.BOX1_GLUE_DISCHARGE_VOLUME_VISION,
            data1.INSULATOR_BAR_CODE,
            data1.BOX1_GLUE_OVERFLOW_VISION,
            $"\"{data1.BOX1_HEATED_AIR_CURING}\"",
            data1.BOX2_GLUE_AMOUNT,
            data1.BOX2_GLUE_DISCHARGE_VOLUME_VISION,
            data1.FPCB_BAR_CODE,
            data1.BOX2_GLUE_OVERFLOW_VISION,
            $"\"{data1.BOX2_HEATED_AIR_CURING}\"",
            data1.BOX3_DISTANCE,
            data1.BOX3_GLUE_AMOUNT,
            data1.BOX3_GLUE_DISCHARGE_VOLUME_VISION,
            $"\"{data1.BOX3_HEATED_AIR_CURING}\"",
            data1.BOX3_GLUE_OVERFLOW_VISION,
            data1.BOX4_TIGHTNESS_AND_LOCATION_VISION,
            data1.BOX4_HEIGHT_PARALLELISM,
            data1.BOX4_RESISTANCE,
            data1.BOX4_AIR_LEAKAGE_TEST_DETAIL,
            data1.BOX4_AIR_LEAKAGE_TEST_RESULT,
            formattedDateTime.Substring(0, 10),
            formattedDateTime.Substring(11)
            };

                writer.WriteLine(string.Join(",", data));
            }
        }

        public static List<string> ReadFilesAndSearch(string directoryPath, string searchKeyword)
        {
            List<string> foundLines = new List<string>();

            try
            {
                string[] files = Directory.GetFiles(directoryPath);

                foreach (var file in files)
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains(searchKeyword))
                            {
                                foundLines.Add($"{line}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading files: {ex.Message}");
            }

            return foundLines;
        }

        public static BOX4RESULT SpiltData4(string input)
        {
            //.Split(new Char [] {' ', ',', '.', '-', '\n', '\t' } );
            string[] lines = input.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            List<string> strings = new List<string>();
            BOX4RESULT result = new BOX4RESULT();
            foreach (string line in lines)
            {
                //string[] spliline = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string[] parts = line.Split(new[] { ':' });
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim().ToUpper();
                    string value = parts[1].Trim(',');

                    switch (key)
                    {
                        case "SERIALNUMBER":
                            result.TOPHOUSING = value;
                            strings.Add(value);
                            break;
                        case "3ND HEATED AIR CURING":
                            result.BOX3_HEATED_AIR_CURING = value;
                            strings.Add(value);
                            break;

                        case "TIGHTNESS AND LOCATION VISION":
                            result.TIGHTNESS_AND_LOCATION_VISION = value;
                            strings.Add(value);
                            break;
                        case "HEIGHT PARALLELISM":
                            result.HEIGHT_PARALLELISM = value;
                            strings.Add(value);
                            break;
                        case "RESISTANCE":
                            result.RESISTANCE = value;
                            strings.Add(value);
                            break;
                        case "AIR LEAKAGE TEST DETAIL":
                            result.AIR_LEAKAGE_TEST_DETAIL = value;
                            strings.Add(value);
                            break;
                        case "AIR LEAKAGE TEST RESULT":
                            result.AIR_LEAKAGE_TEST_RESULT = value;
                            strings.Add(value);
                            break;
                        case "TESTTIME":
                            result.TestTime = value;
                            strings.Add(value);
                            break;
                        default:
                            break;
                    }
                }
                else if (parts.Length > 2)
                {
                    int index = line.IndexOf(": ");
                    string key = line.Substring(0, index); // Extract "TestTime"
                    string value = line.Substring(index + 2); // Extract "2024-06-29 12:56:14"


                    if (key == "TestTime")
                        result.TestTime = value;
                }
            }
            return result;

        }
        public static BOX3RESULT SpiltData3(string input)
        {
            //.Split(new Char [] {' ', ',', '.', '-', '\n', '\t' } );
            string[] lines = input.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            List<string> strings = new List<string>();
            BOX3RESULT result = new BOX3RESULT();
            foreach (string line in lines)
            {
                //string[] spliline = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string[] parts = line.Split(new[] { ':' });
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim().ToUpper();
                    string value = parts[1].Trim(',');

                    switch (key)
                    {
                        case "SERIALNUMBER":
                            result.TOPHOUSING = value;
                            strings.Add(value);
                            break;
                        case "2ND HEATED AIR CURING":
                            result.BOX2_HEATED_AIR_CURING = value;
                            break;
                        case "3ND GLUE AMOUNT":
                            result.GLUE_AMOUNT = value;
                            strings.Add(value);
                            break;
                        case "3ND GLUE DISCHARGE VOLUME VISION":
                            result.GLUE_DISCHARGE_VOLUME_VISION = value;
                            strings.Add(value);
                            break;

                        case "3ND GLUE OVERFLOW VISION":
                            result.BOX3_GLUE_OVERFLOW_VISION = value;
                            strings.Add(value);
                            break;
                        case "DISTANCE":
                            result.DISTANCE = value;
                            break;
                        case "TESTTIME":
                            result.TestTime = value;
                            strings.Add(value);
                            break;
                        default:
                            break;
                    }
                }
                else if (parts.Length > 2)
                {
                    int index = line.IndexOf(": ");
                    string key = line.Substring(0, index); // Extract "TestTime"
                    string value = line.Substring(index + 2); // Extract "2024-06-29 12:56:14"


                    if (key == "TestTime")
                        result.TestTime = value;
                }
            }
            return result;

        }
        public static BOX2RESULT SpiltData2(string input)
        {
            //.Split(new Char [] {' ', ',', '.', '-', '\n', '\t' } );
            string[] lines = input.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            BOX2RESULT result = new BOX2RESULT();
            foreach (string line in lines)
            {
                //string[] spliline = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string[] parts = line.Split(new[] { ':' });
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim().ToUpper();
                    string value = parts[1].Trim(',');

                    switch (key)
                    {
                        case "SERIALNUMBER":
                            result.TOPHOUSING = value;
                            break;
                        case "1ST HEATED AIR CURING":
                            result.BOX1_HEATED_AIR_CURING = value;
                            break;
                        case "2ND GLUE AMOUNT":
                            result.GLUE_AMOUNT = value;
                            break;
                        case "2ND GLUE DISCHARGE VOLUME VISION":
                            result.GLUE_DISCHARGE_VOLUME_VISION = value;
                            break;
                        case "FPCB BAR CODE":
                            result.FPCB_BAR_CODE = value;
                            break;
                        case "2ND GLUE OVERFLOW VISION":
                            result.GLUE_OVERFLOW_VISION = value;
                            break;
                        case "TESTTIME":
                            result.TestTime = value;
                            break;
                        default:
                            break;
                    }
                }
                else if (parts.Length > 2)
                {
                    int index = line.IndexOf(": ");
                    string key = line.Substring(0, index); // Extract "TestTime"
                    string value = line.Substring(index + 2); // Extract "2024-06-29 12:56:14"


                    if (key == "TestTime")
                        result.TestTime = value;
                }
            }
            return result;

        }
        public static BOX1RESULT SpiltData1(string input)
        {
            //.Split(new Char [] {' ', ',', '.', '-', '\n', '\t' } );

            string[] lines = input.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            BOX1RESULT result = new BOX1RESULT();
            foreach (string line in lines)
            {
                //string[] spliline = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string[] parts = line.Split(new[] { ':' });
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim().ToUpper();
                    string value = parts[1].Trim(',');

                    switch (key)
                    {
                        case "SERIALNUMBER":
                            result.TOPHOUSING = value;
                            break;
                        case "1ST GLUE AMOUNT":
                            result.GLUE_AMOUNT = value;
                            break;
                        case "1ST GLUE DISCHARGE VOLUME VISION":
                            result.GLUE_DISCHARGE_VOLUME_VISION = value;
                            break;
                        case "INSULATOR BAR CODE":
                            result.INSULATOR_BAR_CODE = value;
                            break;
                        case "1ST GLUE OVERFLOW VISION":
                            result.GLUE_OVERFLOW_VISION = value;
                            break;
                        //case "1ST HEATED AIR CURING":
                        //    result.HEATED_AIR_CURING = value;
                        //    strings.Add(value);
                        //    break;
                        case "TESTTIME":
                            result.TestTime = value;
                            break;
                        default:
                            break;
                    }
                }
                else if (parts.Length > 2)
                {
                    int index = line.IndexOf(": ");
                    string key = line.Substring(0, index); // Extract "TestTime"
                    string value = line.Substring(index + 2); // Extract "2024-06-29 12:56:14"


                    if (key == "TestTime")
                        result.TestTime = value;
                }
            }
            return result;

        }
        //static string GetValueFromLine(string line, string defaultValue)
        //{
        //    int colonIndex = line.IndexOf(':');
        //    if (colonIndex == -1)
        //    {
        //        return defaultValue;
        //    }
        //    string value = line.Substring(colonIndex + 1).Trim();
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        value = defaultValue;
        //    }
        //    return value;
        //}
        //public static void WriteLogALL(string logMessages)
        //{
        //    string logPath = PLCALL;
        //    string logFileName = DateTime.Now.ToString("yyyyMMdd") + ".CSV";
        //    string logFilePath = Path.Combine(logPath, logFileName);
        //    if (!Directory.Exists(logPath))
        //    {
        //        Directory.CreateDirectory(logPath);
        //        ///NOTE

        //    }
        //    string header = "Top Housing QR코드; 1st Glue Amount ;1st  Glue discharge volume Vision;Insulator bar code;1st Glue overflow vision;1st heated Air curing\t2nd Glue Amount;2nd  Glue discharge volume Vision bar code;2nd Glue overflow vision; 2nd heated Air curing;동심도\t;3rd Glue Amount;3rd Glue discharge volume Vision ;3rd heated Air curing;3rd Glue overflow vision;Tightness and location vision;Height / Parallelism;저항값;Air Leakage Test;생산일자;생산시간";
        //    if (excelrow == 0)
        //    {
        //        excelrow = 1;
        //        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        //        {
        //            //string logFormat = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ==> ";


        //            writer.WriteLine(header);
        //        }

        //    }
        //    else if (excelrow == 1)
        //    {



        //        try
        //        {
        //            using (StreamWriter writer = new StreamWriter(logFilePath, true))
        //            {
        //                //string logFormat = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ==> ";

        //                string logEntry = string.Join(",", logMessages);
        //                writer.WriteLine(logEntry);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error writing log: {ex.Message}");
        //        }
        //    }
        //}
        public static void WriteLog(string logMessage)
        {
            string logFormat = string.Empty;
            string logPath = "D:\\Logs\\CIM";
            logFormat = DateTime.Now.ToLongDateString().ToString() + " - " +
                    DateTime.Now.ToLongTimeString().ToString() + " ==> ";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
                ///NOTE

            }

            try
            {
                //create new log
                using (StreamWriter writer = File.AppendText(logPath +
               "\\" + "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"))
                {

                    writer.WriteLine(logFormat + logMessage);
                }
                //}


            }
            catch (Exception ex)
            {

            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            var a = txttest.Text;
            //string qrcode = $"HTHLI1F003AAPA84-00126G";
            //int qty = 36;
            //var a = GetTraycode(qty);
            //Print(a);
            //ReadData4(a, "", "", "", "", "", "", "", "", "", "");
            //DataSet ds = SqlLite.Instance.SearchData(qrcode);
            //return;

            //HandleReadAirTest("0.02",0);
            //return;
            //string qrcode = $"HTHLI1F003AAPA84-00116G";
            //SingleTonPlcControl.Instance.SetValueRegister(qrcode, 1, "Barcode", true, EnumReadOrWrite.READ);


            ////string sccm = "0.02";
            ////HandleReadAirTest(sccm, 0);
            //return;

            //Print("HTHLI1F00J94PC84-00");
            //return;
            //for (int i = 0; i <= 100; i++)
            //{
            // string qrcode = $"HTHLI1F003AAPA84-00116G";
            //if (qrcode.Length == 21)
            //{
            //    qrcode= $"HTHLI1F00J94PC84-0000{i}G";
            //}
            //else if(qrcode.Length == 22) qrcode = $"HTHLI1F00J94PC84-000{i}G";
            string b1_glue_amount = "80";
            string b1_glue_discharge_volume_vision = "OK";
            string b1_insulator_bar_code = "2024-008";
            string b1_glue_overflow_vision = "OK";
            string P2b1_heated_air_curing = "80℃, 89℃, 81℃, 80℃";
            string b2_heigh = "8.1";
            string b2_glue_amount = "85";
            string b2_glue_discharge_volume_vision = "OK";
            string b2_fpcb_bar_code = "20240008";
            string b2_glue_overflow_vision = "OK";
            string b2_heated_air_curing = "80℃, '89℃, '81℃, '80℃";
            string DISTANCE = "86.25";
            string b3_glue_amount = "70";
            string b3_glue_discharge_volume_vision = "OK";
            string b3_heated_air_curing = "53℃, '53℃, '53℃, '50℃";
            string b3_glue_overflow_vision = "OK";
            string B4_tightness_and_location_vision = "OK";
            string height_parallelism = "6.688mm,6.62mm, OK";
            string resistance = "75.25";
            //string location = "1L";
            string air_result = "NG";
            string air_detail = "0.05";

            //SingleTonPlcControl.Instance.SetValueRegister(qrcode, 1, "Barcode", true, EnumReadOrWrite.READ);

            //Task.Run(() =>
            //{
            //    SingleTonPlcControl.Instance.SetValueRegister("HTHLI5E01868PA84-00176J", 1, "BOX1Barcode", true, EnumReadOrWrite.READ);
            //    SingleTonPlcControl.Instance.SetValueRegister("HTHLI5E01868PA84-00176J", 2, "BOX2Barcode", true, EnumReadOrWrite.READ);
            //    SingleTonPlcControl.Instance.SetValueRegister("HTHLI5E01868PA84-00176J", 3, "BOX3Barcode", true, EnumReadOrWrite.READ);
            //    //SingleTonPlcControl.Instance.SetValueRegister(true, 4, "BOX4TIGHTNESS_AND_LOCATION_VISION", true, EnumReadOrWrite.READ);
            //    // SingleTonPlcControl.Instance.SetValueRegister(0.22, 4, "BOX4AIR_LEAKAGE_TEST_DETAIL", true, EnumReadOrWrite.READ);
            //});


            //string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string selectedValue = comboBox1.SelectedItem.ToString().Trim();
            if (selectedValue == "ALL")
            {
                var listdata = list.OrderByDescending(r => r.NO);
                foreach (var data in listdata)
                {
                    dataGridView1.Rows.Add(data.NO, data.TOPHOUSING, data.BOX1_GLUE_AMOUNT, data.BOX1_GLUE_DISCHARGE_VOLUME_VISION, data.INSULATOR_BAR_CODE, data.BOX1_GLUE_OVERFLOW_VISION, data.BOX1_HEATED_AIR_CURING, data.BOX2_GLUE_AMOUNT, data.BOX2_GLUE_DISCHARGE_VOLUME_VISION, data.FPCB_BAR_CODE, data.BOX2_GLUE_OVERFLOW_VISION, data.BOX2_HEATED_AIR_CURING, data.BOX3_DISTANCE, data.BOX3_GLUE_AMOUNT, data.BOX3_GLUE_DISCHARGE_VOLUME_VISION, data.BOX3_HEATED_AIR_CURING, data.BOX3_GLUE_OVERFLOW_VISION,  data.BOX4_TIGHTNESS_AND_LOCATION_VISION, data.BOX4_HEIGHT_PARALLELISM, data.BOX4_RESISTANCE, data.BOX4_AIR_LEAKAGE_TEST_DETAIL, data.BOX4_AIR_LEAKAGE_TEST_RESULT, data.BOX4_TestTime);

                }
            }
            else
            {
                var listdata = list.Where(r => r.BOX4_AIR_LEAKAGE_TEST_RESULT.Trim() == selectedValue);
                foreach (var data in listdata)
                {
                    dataGridView1.Rows.Add(data.NO, data.TOPHOUSING, data.BOX1_GLUE_AMOUNT, data.BOX1_GLUE_DISCHARGE_VOLUME_VISION, data.INSULATOR_BAR_CODE, data.BOX1_GLUE_OVERFLOW_VISION, data.BOX1_HEATED_AIR_CURING, data.BOX2_GLUE_AMOUNT, data.BOX2_GLUE_DISCHARGE_VOLUME_VISION, data.FPCB_BAR_CODE, data.BOX2_GLUE_OVERFLOW_VISION, data.BOX2_HEATED_AIR_CURING, data.BOX3_DISTANCE, data.BOX3_GLUE_AMOUNT, data.BOX3_GLUE_DISCHARGE_VOLUME_VISION, data.BOX3_HEATED_AIR_CURING, data.BOX3_GLUE_OVERFLOW_VISION, data.BOX4_TIGHTNESS_AND_LOCATION_VISION, data.BOX4_HEIGHT_PARALLELISM, data.BOX4_RESISTANCE, data.BOX4_AIR_LEAKAGE_TEST_DETAIL, data.BOX4_AIR_LEAKAGE_TEST_RESULT, data.BOX4_TestTime);

                }
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("dddd, MMM dd, yyyy | HH:mm:ss");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SearchForm fs = new SearchForm();
            fs.ShowDialog();

        }

        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            MessageTimer.Stop();
            lblalert.Text = "";
            lblalert.Visible = false;
        }

        public static void WriteLogBox(string logFilePath, params string[] logMessages)
        {


            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true, new UTF8Encoding(true)))
                {
                    //string logFormat = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ==> ";

                    string logEntry = string.Join(";", logMessages);
                    writer.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing log: {ex.Message}");
            }
        }
        public static string model = ConfigurationManager.AppSettings["MODEL"].ToString();
        public static string GetTraycode(int qty)
        {
            string result = "";
            switch (model.ToString())
            {
                case "ASSY HOUSING_Wing":
                    result = "PA84-00176J";
                    break;
                case "ASSY HOUSING_Guide Pin":
                    result = "PA84-00176H";
                    break;
                case "ASSY HOUSING":
                    result = "PA84-00176G";
                    break;
            }
            result += "U";
            DateMap mapper = new DateMap();

            string ymd = mapper.GetValue(DateTime.Today);

            result += ymd;


            return result;
        }
    }
}
