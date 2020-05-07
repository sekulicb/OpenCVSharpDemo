using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Window = System.Windows.Window;

namespace OpenCvSharpDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VideoCapture cap;
        WriteableBitmap wb;
        const int frameWidth = 1600;
        const int frameHeight = 1200;
        bool loop = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (InitWebCamera())
            {
                MessageBox.Show("Webcam is on.");
            }
            else
            {
                MessageBox.Show("There is a problem with the webcam.");
            }
        }

        private bool InitWebCamera()
        {
            try
            {
                cap = VideoCapture.FromCamera(0, VideoCaptureAPIs.DSHOW);
                cap.Open(0);
                cap.FrameWidth = frameWidth;
                cap.FrameHeight = frameHeight;

                wb = new WriteableBitmap(cap.FrameWidth, cap.FrameHeight, 96, 96, PixelFormats.Bgr24, null);
                image.Source = wb;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loop = false;
            if (cap.IsOpened())
            {
                cap.Dispose();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Mat frame = new Mat();
            Cv2.NamedWindow("1", WindowMode.AutoSize);
            loop = true;
            while (loop)
            {


                if (cap.Read(frame))
                {
                    Cv2.ImShow("1", frame);
                    var t = frame.ToBitmap();
                    var bitmap = t.ConvertToWpfImage();
                    image.Source = bitmap;
                }

                int c = Cv2.WaitKey(10); 
                if (c  != -1 )
                    break;

            }
        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            using (var fileStream = new FileStream(string.Format($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{Guid.NewGuid()}.png"), FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
               
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
                encoder.Save(fileStream);
            }
        }
    }
}
