using System;
//using System.IO;
using System.Timers;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Documents;

namespace ThaiClock
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Color
            neonBlue = Color.FromRgb(21, 244, 238),
            cornflowerBlue = Color.FromRgb(100, 149, 237),
            violet = Color.FromRgb(91, 10, 145),
            neonGreen = Color.FromRgb(57, 255, 20),
            neonYellow = Color.FromRgb(250, 237, 39);

        public MainWindow() {
            InitializeComponent();

            //loadFonts();
            initFonts();

            t.Elapsed += Elapsed;


            Background = Brushes.Black;

            lblClock.Content = "กำลังเริ่ม...";
            lblClock.FontWeight = light;
            lblClock.FontFamily = mitr;
            lblClock.Foreground = new SolidColorBrush(neonBlue);

            (lblClock.Effect as DropShadowEffect).Color = violet;


            initRuns();

            txbSentence.Background = Brushes.Transparent;
            //txbSentence.Background = new SolidColorBrush(Color.FromArgb(96, 0, 0, 0));

            txbSentence.Inlines.AddRange(new dynamic[] {
                itIsRun, new LineBreak(), hourRun, new LineBreak(), minuteRun
            });

            txbSentence_Copy.Background = Brushes.Transparent;
            txbSentence_Copy.Inlines.AddRange(new dynamic[] {
                itIsRunCopy, new LineBreak(), hourRunCopy, new LineBreak(), minuteRunCopy
            });
            txbSentence_Copy.Foreground = new SolidColorBrush(violet);
        }

        private FontFamily mitr;
        private FontWeight light, medium, bold;

        private void initFonts() {
            mitr = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Mitr");
            (light, medium, bold) = (
                FontWeight.FromOpenTypeWeight(300),
                FontWeight.FromOpenTypeWeight(400),
                FontWeight.FromOpenTypeWeight(700)
            );
        }

        private Run getItIsRun(bool useNeon) => new Run()
        {
            Text = itIs,
            FontSize = 24,
            FontFamily = mitr,
            FontWeight = light,

            Foreground = useNeon ? new SolidColorBrush(neonGreen) : Brushes.Green,
        };

        private Run getHourRun(bool useNeon) => new Run()
        {
            FontSize = 36,
            FontFamily = mitr,
            FontWeight = medium,

            Foreground = useNeon ? new SolidColorBrush(neonYellow) : Brushes.Olive
        };

        private Run getMinuteRun(bool useNeon) => new Run()
        {
            FontSize = 36,
            FontFamily = mitr,
            FontWeight = medium,

            Foreground = useNeon ? new SolidColorBrush(neonYellow) : Brushes.Olive
        };

        private void initRuns() {
            itIsRun = getItIsRun(true);
            hourRun = getHourRun(true);
            minuteRun = getMinuteRun(true);

            itIsRunCopy = getItIsRun(false);
            hourRunCopy = getHourRun(false);
            minuteRunCopy = getMinuteRun(false);
        }


        private readonly Timer t = new Timer(1000);
        // for use in the display
        private Run
            itIsRun, hourRun, minuteRun,
            itIsRunCopy, hourRunCopy, minuteRunCopy;

        private int formalToggleTicksLeft = 30;
        private bool
            isFormal = false, // only for display
            isVisible = true;

        private void btnVisibility_Click(object sender, RoutedEventArgs e)
        {
            isVisible = !isVisible;

            btnVisibility.Opacity = isVisible ? 1 : 0.25;

            lblClock.Visibility = txbSentence.Visibility = txbSentence_Copy.Visibility =
                isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                formalToggleTicksLeft--;
                if (formalToggleTicksLeft < 0) {
                    formalToggleTicksLeft += 30;
                    isFormal = !isFormal;
                }

                // necessary for WPF
                Dispatcher.Invoke(() =>
                {
                    updateClockDisplay();
                    updateClockSentence();
                });
            }
            catch (Exception ex) {
                Debug.Print(ex.Message);
            }
        }

        private void updateClockDisplay() {
            lblClock.Content = DateTime.Now.ToString("HH:mm:ss");
        }

        private void updateClockSentence() {
            var now = DateTime.Now;
            var (hour, minute) = (now.Hour, now.Minute);

            hourRun.Text = hourRunCopy.Text = isFormal ? getFormalHourPhrase(hour) : getHourPhrase(hour);
            minuteRun.Text = minuteRunCopy.Text = getMinutePhrase(minute);
        }

        //private PrivateFontCollection mitr = new PrivateFontCollection();

        //private void loadFonts() {
        //    var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    Debug.Print(path);

        //    var filenames = Directory.GetFiles(path);

        //    foreach (var filename in filenames)
        //        mitr.AddFontFile(filename);

        //    // Todo: load the font files
        //}

        private readonly string[] units = { "", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า" };
        private const string
            zero = "ศูนย์",
            one = "เอ็ด", // only for numbers 11 and above
            ten = "สิบ", // for multipliers
            twenty = "ยี่สิบ", // special case

            itIs = "มันเป็นเวลา",

            clock = "นาฬิกา",
            half = "ครึ่ง",
            noon = "เที่ยงวัน",
            midnight = "เที่ยงคืน",
            hour = "โมง",
            morning = "เช้า",
            afternoon = "บ่าย",
            evening = "เย็น",
            thum = "ทุ่ม",
            ti = "ตี";

        /// <param name="hour">24 hour format (0-23)</param>
        private string getFormalHourPhrase(int hour) {
            if (hour < 0 || hour > 23)
                throw new ArgumentOutOfRangeException("num", "Only the numbers 0-23 are allowed");

            return $"{getNumberStr(hour)}{clock}";
        }

        /// <param name="hour">24 hour format (0-23)</param>
        private string getHourPhrase(int hour) {
            if (hour < 0 || hour > 23)
                throw new ArgumentOutOfRangeException("num", "Only the numbers 0-23 are allowed");

            var numberStr = hour >= 18 && hour < 24
                ? getNumberStr(hour - 18)
                : hour > 13 && hour <= 15
                ? getNumberStr(hour - 12)
                : getNumberStr(hour);

            return hour == 0
                ? midnight
                : hour > 0 && hour <= 6
                ? $"{ti}{numberStr}"
                : hour < 12
                ? $"{numberStr}{MainWindow.hour}({morning})"
                : hour == 12
                ? noon

                : hour == 13
                ? $"{afternoon}{MainWindow.hour}"
                : hour <= 15
                ? $"{afternoon}{numberStr}({MainWindow.hour})"

                : hour <= 18
                ? $"{numberStr}{MainWindow.hour}({evening})"

                : $"{numberStr}{thum}";
        }

        private string getMinutePhrase(int minute) {
            if (minute < 0 || minute >= 60)
                throw new ArgumentOutOfRangeException("minute", "Only the numbers 0-59 are allowed");

            return minute == 0
                ? ""
                : minute == 30
                ? half
                : getNumberStr(minute) + "นาที";
        }

        private string getNumberStr(int num) {
            // Translate the numbers to a readable format
            var sb = new StringBuilder();
            var unit = num % 10;

            if (num == 0)
                sb.Append(zero);

            var tens = num / 10;
            sb.Append(tens == 2 ? twenty : tens > 1 ? units[tens] : "");
            if (tens == 1 || tens > 2)
                sb.Append(ten);

            sb.Append(
                num > 10
                ? (unit == 1 ? one : units[unit])
                : units[unit]
            );

            return $"{sb}";
        }

        private void loadDemoText() {
            var sb = new StringBuilder();

            sb.AppendLine("Hours:");

            for (int a = 0; a < 24; a++)
                sb.AppendLine($"{a}: {getHourPhrase(a)}");

            sb.AppendLine();

            sb.AppendLine("Minutes:");
            for (int a = 0; a < 60; a++)
                sb.AppendLine($"{a}: {getMinutePhrase(a)}");

            txbSentence.Text = $"{sb}";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //loadDemoText();
            t.Start();
        }
    }
}
