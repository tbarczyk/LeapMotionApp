using System.Windows;
using Caliburn.Micro;
using System.Threading;
using System.Drawing;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Windows.Media;
using LeapMotion.Navigation;
using System.Dynamic;
using LeapMotion.LeapControl;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using LeapMotion.Models;
namespace LeapMotion.ViewModels
{
    [Export(typeof(MainWindowViewModel))]
    public class MainWindowViewModel : Conductor<object>, IHandle<ColorEvent>
    {
        private int _count = 50;
        private string _leapInfo;
        private int[] _leapCoord;
        private Thread getInfo;

        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;

        private readonly IEventAggregator _eventAggregator; 

        private BitmapSource _imageSrc;

        private LeapListener _leapListener;
        private readonly IWindowManager _windowManager;
        

        
        [ImportingConstructor]
        public MainWindowViewModel(ColorViewModel colorVM, LeapConnectInfoViewModel leapConnectInfo, IEventAggregator events, IWindowManager windowMan, IEventAggregator eventAggregator, GameViewModel gameVM)
        {
            gameViewModel = gameVM;
            colorViewModel = colorVM;
            _eventAggregator = eventAggregator;
            _leapCoord = new int[3] { 0, 0, 0 };
            LeapCoord = new int[3] { 0, 0, 0 };
            _leapListener = new LeapListener();
            _windowManager = windowMan;
            
            this.leapConnectInfoViewModel = leapConnectInfo;
            events.Subscribe(this);
            getInfo = new Thread(getInfoFromLeap);
            getInfo.Start();
            
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
                Console.WriteLine("AAAA: " + Device.MonikerString);
            try
            {
                if(CaptureDevice.Count!=0)
                {
                    FinalFrame = new VideoCaptureDevice(CaptureDevice[0].MonikerString);
                    FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
                    FinalFrame.Start();
                }
            }
            catch
            {

            }
        }
        
        void timer_Tick(object sender, EventArgs e)
        {
            Text = DateTime.Now.ToLongTimeString();
        }

        private string _text;


        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        public BitmapSource ImageSrc
        {
            get { return _imageSrc; }
            set
            {
                _imageSrc = value;
                NotifyOfPropertyChange(() => ImageSrc);
            }
        }
        
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var bitmapSrc = Bitmap2BitmapImage(eventArgs.Frame);
            if (bitmapSrc.CanFreeze)
                bitmapSrc.Freeze();

            ImageSrc = bitmapSrc;
        }

        private BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
            return i;
        }   

        public MainWindowViewModel(ColorViewModel colorVM, IWindowManager windowMan)
        {
           
        }

        public void MakeAggregator()
        {
            _eventAggregator.Publish("this is a message");
        }   
        

        
        public void getInfoFromLeap()
        {
            while (true)
            {
                LeapInfo = _leapListener.LeapInfo.X + "," + _leapListener.LeapInfo.Y + "," + _leapListener.LeapInfo.Z;
                var pos= 150 + _leapListener.LeapInfo.X;
                if (0 > pos) LeapCoord[0] = 0;
                else if (pos >= 295) LeapCoord[0] = 295;
                else LeapCoord[0] = pos;

                pos = 300 - _leapListener.LeapInfo.Y;
                if (pos >= 300) LeapCoord[1] = 300;
                else if (pos < 5) LeapCoord[1] = 5;
                else LeapCoord[1] = pos;

                LeapCoord[2] = LeapCoord[0]+5;
                NotifyOfPropertyChange(() => LeapCoord);
                //Console.WriteLine("AAAAAAAAAAA" + LeapInfo);
                Thread.Sleep(100);
            }

        }

        public void Handle(ColorEvent message)
        {
            Color = message.Color;
        }

        public ColorViewModel colorViewModel { get; private set; }
        public GameViewModel gameViewModel { get; private set; }
        public LeapConnectInfoViewModel leapConnectInfoViewModel { get; private set; }

        private SolidColorBrush _Color;

        public void ExitButton()
        {
            getInfo.Abort();
            Application.Current.Shutdown();
        }

        public void OpenWindow()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.Manual;
            _windowManager.ShowWindow(new MainWindowViewModel(colorViewModel, _windowManager));
        }

        public SolidColorBrush Color
        {
            get { return _Color; }
            set
            {
                _Color = value;
                NotifyOfPropertyChange(() => Color);
            }
        }

        public string LeapInfo
        {
            get { return _leapInfo; }
            set
            {
                _leapInfo = value;
                NotifyOfPropertyChange(() => LeapInfo);
            }
        }
        
        public int[] LeapCoord
        {
            get { return _leapCoord; }
            set
            {
                _leapCoord = value;
                NotifyOfPropertyChange(() => LeapCoord);
            }
        }

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                NotifyOfPropertyChange(() => Count);
                NotifyOfPropertyChange(() => CanIncrementCount);
            }
        }

        public void OnButton()
        {
            ActivateItem(leapConnectInfoViewModel);
        }

        public void OffButton()
        {
            DeactivateItem(leapConnectInfoViewModel, true);
        }

        public void IncrementCount(int delta)
        {
            Count += delta;
        }

        public bool CanIncrementCount
        {
            get { return Count < 100; }
        }

        
    }
}
