using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using Gma.System.MouseKeyHook;
using System.Windows.Automation;
using System.Runtime.InteropServices;

namespace ProcessCapture
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        #endregion

        public static string GetActiveProcessFileName()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return p.ProcessName;
        }
        
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private static AutomationElement ElementFromCursor(double Xaxis, double Yaxis)
        {
            // Convert mouse position from System.Drawing.Point to System.Windows.Point.
            System.Windows.Point point = new System.Windows.Point(Xaxis, Yaxis);
            AutomationElement element = AutomationElement.FromPoint(point);
            return element;
        }

        public static void ListenForMouseEvents()
        {

            Debug.WriteLine("Listening to mouse clicks.");

            //When a mouse button is pressed 
            Hook.GlobalEvents().MouseDown += async (sender, e) =>
            {
                Debug.WriteLine($"Mouse {e.Button} Down");
                string BaseFolder = ConfigurationManager.AppSettings["BaseFolder"];
                string dbConStr = ConfigurationManager.ConnectionStrings["ProcessDiscoveryDB"].ConnectionString;
                double Xaxis = GetMousePosition().X;
                double Yaxis = GetMousePosition().Y;
                
                Debug.WriteLine("{0} ---- {1}", Xaxis, Yaxis);

                new Thread(new ThreadStart(() =>{                     
                    string CurrEleName = ElementFromCursor(Xaxis, Yaxis).Current.Name;
                    Debug.WriteLine(CurrEleName);
                    string ProcessName = GetActiveProcessFileName();
                    Debug.WriteLine(ProcessName);
                    using (SqlConnection con = new SqlConnection(dbConStr)) 
                     { 
                        using (SqlCommand cmd = new SqlCommand("uspProcessCapture", con)) 
                        {   
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@CurrTime", SqlDbType.DateTime).Value = DateTime.Now;
                            cmd.Parameters.Add("@ProcessName", SqlDbType.NVarChar).Value = ProcessName.ToString();
                            cmd.Parameters.Add("@ProcessSteps", SqlDbType.NVarChar).Value = CurrEleName.ToString();
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                     }
                })).Start();

                Recorder rec = new Recorder(BaseFolder, Xaxis, Yaxis);
                rec.Start();
            };
            //When a double click is made
           /* Hook.GlobalEvents().MouseDoubleClick += async (sender, e) =>
            {
                Debug.WriteLine($"Mouse {e.Button} button double clicked.");                
            };*/
        }

    }
}
