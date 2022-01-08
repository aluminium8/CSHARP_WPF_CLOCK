using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Runtime.InteropServices;
using System.Windows.Interop;

using System.Windows.Threading;//タイマー用



namespace clock_cSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        protected const int GWL_EXSTYLE = (-20);
        protected const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32")]
        protected static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        protected static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwLong);

        double analogclock_size = (SystemParameters.PrimaryScreenWidth / 24),digital_size=0.5;
        Point analog_center;
        Line h_hand = new Line(), m_hand = new Line(), s_hand = new Line();
        SolidColorBrush mySolidColorBrush_hhand = new SolidColorBrush();
        SolidColorBrush mySolidColorBrush_mhand = new SolidColorBrush();
        SolidColorBrush mySolidColorBrush_shand = new SolidColorBrush();
    
        public MainWindow()
        {
            InitializeComponent();

            Canvas canvas1 = new Canvas();
        ///    canvas1.Background = Brushes.LightBlue;
            this.Grid1.Children.Add(canvas1);
            Rectangle rect1 = new Rectangle();

            Ellipse ellipse1 = new Ellipse();
    
            DateTime dt = DateTime.Now;
            this.time_string.Text = dt.ToString("yyyy/MM/dd HH:mm:ss");

            /*
 * Windowの表示位置をマニュアル指定
 */
            mySolidColorBrush_hhand.Color = Color.FromArgb(202, 60, 60, 60);
            mySolidColorBrush_mhand.Color = Color.FromArgb(202, 100, 100, 200);
            mySolidColorBrush_shand.Color = Color.FromArgb(202, 200, 100, 100);
            // TransformGroupを作成(大きさを3倍にして、右に10pixcelずらす)
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var tfg = new TransformGroup(); 
                tfg.Children.Add(new ScaleTransform(digital_size,digital_size ));
                this.time_string.RenderTransform = tfg;

                tfg.Children.Add(new TranslateTransform(SystemParameters.PrimaryScreenWidth - this.time_string.ActualWidth*digital_size- analogclock_size, SystemParameters.PrimaryScreenHeight - this.time_string.ActualHeight * digital_size));
                this.time_string.RenderTransform = tfg;

                rect1.Width = analogclock_size;
                rect1.Height = analogclock_size;
                rect1.StrokeThickness = 0;
                ellipse1.Width = analogclock_size;
                ellipse1.Height = analogclock_size;
                ellipse1.StrokeThickness = 2.5;
                SolidColorBrush mySolidColorBrush_ell = new SolidColorBrush();
                mySolidColorBrush_ell.Color = Color.FromArgb(102, 100, 100, 100);
                ellipse1.Stroke = mySolidColorBrush_ell;
                Canvas.SetLeft(rect1, SystemParameters.PrimaryScreenWidth - analogclock_size);
                Canvas.SetTop(rect1, SystemParameters.PrimaryScreenHeight - analogclock_size);
                Canvas.SetLeft(ellipse1, SystemParameters.PrimaryScreenWidth - analogclock_size);
                Canvas.SetTop(ellipse1, SystemParameters.PrimaryScreenHeight - analogclock_size);
                rect1.StrokeThickness = 0;
                canvas1.Children.Add(rect1);
                canvas1.Children.Add(ellipse1);
                SolidColorBrush mySolidColorBrush2 = new SolidColorBrush();
                mySolidColorBrush2.Color = Color.FromArgb(102, 204, 238, 238);
                rect1.Fill = mySolidColorBrush2;
                analog_center.X = SystemParameters.PrimaryScreenWidth - analogclock_size / 2;
                analog_center.Y = SystemParameters.PrimaryScreenHeight - analogclock_size / 2;

                h_hand.X1 = analog_center.X;
                h_hand.Y1 = analog_center.Y;
                m_hand.X1 = analog_center.X;
                m_hand.Y1 = analog_center.Y;
                s_hand.X1 = analog_center.X;
                s_hand.Y1 = analog_center.Y;
                h_hand.X2 = analog_center.X;
                h_hand.Y2 = analog_center.Y;
                m_hand.X2 = analog_center.X;
                m_hand.Y2 = analog_center.Y;
                s_hand.X2 = analog_center.X;
                s_hand.Y2 = analog_center.Y;
                //      h_hand.Stroke = System.Windows.Media.Brushes.Silver;
                h_hand.StrokeThickness = 4;
                //h_hand.StrokeDashArray = new DoubleCollection { 2, 2 };
                h_hand.StrokeDashCap = PenLineCap.Round;
                h_hand.SnapsToDevicePixels = true;
               // m_hand.Stroke = System.Windows.Media.Brushes.Silver;
                m_hand.StrokeThickness = 2;
               // m_hand.StrokeDashArray = new DoubleCollection { 2, 2 };
                m_hand.StrokeDashCap = PenLineCap.Round;
                m_hand.SnapsToDevicePixels = true;
            //    s_hand.Stroke = System.Windows.Media.Brushes.Silver;
                s_hand.StrokeThickness = 1.5;
             //   s_hand.StrokeDashArray = new DoubleCollection { 2, 2 };
                s_hand.StrokeDashCap = PenLineCap.Round;
                s_hand.SnapsToDevicePixels = true;
               
                canvas1.Children.Add(h_hand);
                canvas1.Children.Add(m_hand);
                canvas1.Children.Add(s_hand);
            }), DispatcherPriority.Loaded);

            //タイマーの設定、一定時間ごとにマウスカーソルの状態を見る
            var timer = new DispatcherTimer(DispatcherPriority.Normal);
            //timer.Interval = new TimeSpan(100000);//ナノ秒
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);//第4変数，ミリ秒単位
            timer.Start();
            timer.Tick += Timer_Tick;
            
       
       
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            this.time_string.Text = dt.ToString("yyyy/MM/dd HH:mm:ss");
            //this.time_string.Text += Math.Cos(Math.PI / 2 - 2 * Math.PI * (dt.Hour / 12)) * analogclock_size / 3;
           // this.time_string.Text += Math.Cos(Math.PI / 2 - 2 * Math.PI * (dt.Minute/ 12)) * analogclock_size / 3;
           // this.time_string.Text += (dt.Hour / 12.0);

            //  this.time_string.Text += SystemParameters.WorkArea.Width;
            // TransformGroupを作成(大きさを3倍にして、右に10pixcelずらす)

            h_hand.X2 =analog_center.X + Math.Cos(Math.PI / 2 - 2 * Math.PI*((dt.Hour / 12.0)+dt.Minute/(60.0*12.0)))* analogclock_size / 3;
            h_hand.Y2 = analog_center.Y - Math.Sin(Math.PI / 2 - 2 * Math.PI * ((dt.Hour / 12.0) + dt.Minute / (60.0 * 12.0))) * analogclock_size / 3;
            m_hand.X2 = analog_center.X + Math.Cos(Math.PI / 2 - 2 * Math.PI * (dt.Minute / 60.0))* analogclock_size / 2.5;
            m_hand.Y2 = analog_center.Y - Math.Sin(Math.PI / 2 - 2 * Math.PI * (dt.Minute / 60.0))* analogclock_size / 2.5;
            s_hand.X2 = analog_center.X + Math.Cos(Math.PI / 2 - 2 * Math.PI * (dt.Second / 60.0))* analogclock_size / 2.1;
            s_hand.Y2 = analog_center.Y - Math.Sin(Math.PI / 2 - 2 * Math.PI * (dt.Second / 60.0)) * analogclock_size / 2.1;

            h_hand.Stroke = mySolidColorBrush_hhand;
            m_hand.Stroke = mySolidColorBrush_mhand;
            s_hand.Stroke = mySolidColorBrush_shand;

        }



        protected override void OnSourceInitialized(EventArgs e)
        {

            base.OnSourceInitialized(e);

            //WindowHandle(Win32) を取得
            var handle = new WindowInteropHelper(this).Handle;

            //クリックをスルー
            int extendStyle = GetWindowLong(handle, GWL_EXSTYLE);
            extendStyle |= WS_EX_TRANSPARENT; //フラグの追加
            SetWindowLong(handle, GWL_EXSTYLE, extendStyle);

        }
    }




}
