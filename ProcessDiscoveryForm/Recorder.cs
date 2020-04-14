using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace ProcessCapture
{
    public class Recorder
    {
        private readonly string savePath;       
        private readonly Thread recorderThread;
        private  double X;
        private  double Y;

        public Recorder(string savePath, double X, double Y)
        {
            this.X = X;
            this.Y = Y;
            this.savePath = savePath;
            recorderThread = new Thread(StartRecordingAction);
        }
        
        public void Start()
        {       
            recorderThread.Start();
        }
        
        public void Stop()
        {           
        }
        
        private void StartRecordingAction()
        {
            string SCpathWithSubfolder = string.Empty;
            string LabelpathWithSubfolder = string.Empty;
            DateTime CurrTime = DateTime.Now;
            SCpathWithSubfolder = savePath + @"Captures\"+ @"ScreenCapture\" ;
            LabelpathWithSubfolder = savePath + @"Captures\"+@"LabelCapture\";
            CheckFolder(SCpathWithSubfolder);
            CheckFolder(LabelpathWithSubfolder);

            switch (X)
            {
                case double n when (n <= 100):
                    X = Convert.ToDouble(0);
                    break;
                case double n when (n > 100):
                    X = X - Convert.ToDouble(100);
                    break;
            }

            switch (Y)
            {
                case double n when (n <= 100):
                    Y = Convert.ToDouble(0);
                    break;
                case double n when (n > 100):
                    Y -= Convert.ToDouble(100);
                    break;
            }

            Rectangle rect = new Rectangle(Convert.ToInt32(X), Convert.ToInt32(Y), 200, 100);            
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            bmp.Save(LabelpathWithSubfolder + CurrTime.ToString("hhmmss") + "-Label" + ".png", ImageFormat.Png);

            using (Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
            {
                using (Graphics graphic = Graphics.FromImage(bmpScreenCapture))
                {
                    graphic.CopyFromScreen(
                      Screen.PrimaryScreen.Bounds.X,
                      Screen.PrimaryScreen.Bounds.Y,
                      0, 0,
                      bmpScreenCapture.Size,
                      CopyPixelOperation.SourceCopy);
                }                    
                bmpScreenCapture.Save(SCpathWithSubfolder + CurrTime.ToString("MMddyy-hhmmss") + "-FullScreen" + ".png", ImageFormat.Png);
            }         
        }
        
        private void CheckFolder(String path)
        {
            bool exists = System.IO.Directory.Exists(path);
            if (exists == false)
            {
                System.IO.Directory.CreateDirectory(path);           
            }
        }
    }
}