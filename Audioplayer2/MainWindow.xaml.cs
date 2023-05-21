using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;



namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly MediaPlayer Player = new MediaPlayer();
        string medialist = "";
        bool playing = false;
        int counter = 0;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            string PathConfig = @"C:\NTPlayer";
            if (!Directory.Exists(PathConfig))
            {
                Directory.CreateDirectory(PathConfig);
            }
            GetConfig();
            MediaListBox.Width = 201;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_tick;
            Player.MediaEnded += Player_MediaEnded;

        }

        private void TryToGetTime()
        {
            try
            {
                while (!Player.NaturalDuration.HasTimeSpan)
                {

                }
                double dur = TimeSpanToDouble(Player.NaturalDuration.TimeSpan);
                Current_Time.Maximum = dur;
                MaxTime.Text = SecToTime(dur, false);
            }
            catch
            { 
            }
        }

        void GetConfig()
        { 
            if (File.Exists(@"C:\NTPlayer\MediaLibrary.cfg"))
            {
                bool deletefiles = false;
                StreamReader copyCheck = new StreamReader(@"C:\NTPlayer\MediaLibrary.cfg");
                medialist = copyCheck.ReadToEnd();
                copyCheck.Close();
                string[] mlist = medialist.Split('\n');
                foreach (string a in mlist)
                {
                    if (a.Length > 1)
                    {
                        if (File.Exists(a))
                        {
                            AddToList(a);
                        }
                        else
                        {
                            medialist = medialist.Remove(medialist.IndexOf(a), a.Length + 1);
                            deletefiles = true;
                        }

                    }
                }
                if (deletefiles)
                {
                    Refresh();
                }

            }

        }

        void Refresh()
        {
            StreamWriter rem = new StreamWriter(@"C:\NTPlayer\MediaLibrary.cfg", false);
            rem.Write(medialist);
            rem.Close();

        }

        void AddFiles(string[] path)
        {

            StreamWriter addfile = new StreamWriter(@"C:\NTPlayer\MediaLibrary.cfg", true);
            foreach (string a in path)
            {
                if (!medialist.Contains(a))
                {
                    /// Запись в файл
                    addfile.WriteLine(a);
                    medialist += a + "\n";
                    AddToList(a);
                }
            }
            addfile.Close();
        }

        void AddToList(string a)
        {

            TextBlock meta = new TextBlock
            {
                Width = 150,
                TextWrapping = TextWrapping.Wrap
            };
            string[] aboba = a.Split('\\');
            meta.Text = aboba[aboba.Length - 1];

            Grid grid = new Grid();
            ColumnDefinition colDef = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();

            colDef.Width = new GridLength(150, GridUnitType.Pixel);
            colDef2.Width = new GridLength(20, GridUnitType.Pixel);

            grid.ColumnDefinitions.Add(colDef);
            grid.ColumnDefinitions.Add(colDef2);

            Button button = new Button
            {
                Width = 20,
                Height = 20,
                Background = null,
                BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x3E, 0x3B, 0x3B)),
                Foreground = Brushes.White,
                Content = "X",
                Tag = counter
            };
            button.Click += RemoveItem;

            grid.Children.Add(meta);
            grid.Children.Add(button);

            Grid.SetColumn(meta, 0);
            Grid.SetColumn(button, 1);

            ListBoxItem item = new ListBoxItem
            {
                Tag = (counter++).ToString() + "|" + a,
                Content = grid
            };

            MediaListBox.Items.Add(item);


        }

        string SecToTime(double value, bool b)
        {
            // b - TimeSpan format
            double h = Math.Floor(value / 360);
            if (h > 0)
            {
                return h.ToString() + ':' + (Math.Floor(value / 60) % 60).ToString() + ':' + Math.Round(value % 60).ToString(); ;
            }
            if (b)
            {
                return "00:" + (Math.Floor(value / 60) % 60).ToString() + ':' + Math.Round(value % 60).ToString();
            }
            else
            {
                return (Math.Floor(value / 60) % 60).ToString() + ':' + Math.Round(value % 60).ToString();
            }
        }


        double TimeSpanToDouble(TimeSpan time)
        {
            double seconds = 0;
            double num;
            int del = 3600;
            foreach (string t in time.ToString().Split(':'))
            {
                
                double.TryParse(t.Replace('.',','), out num);
                seconds += num * del;
                del /= 60;
            }
            return seconds;
        }

        /// Events

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            MediaListBox.SelectedIndex += 1;
        }

        private void Timer_tick(object sender, EventArgs e)
        {
            Current_Time.Value +=1;
        }

        private void AddMediaButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog filediag = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                DefaultExt = ".mp3",
                Filter = "AudioFiles | *.mp3"
            };
            Nullable<bool> result = filediag.ShowDialog();

            if (result == true)
            {
                string[] filenames = filediag.FileNames;

                AddFiles(filenames);

            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Volume = VolumeSlider.Value;
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            Current_Time.Value = TimeSpanToDouble(Player.Position);
            if (playing)
            {
                Player.Pause();
                _timer.Stop();
                playing = false;
                (PlayPause.Template.FindName("Im", PlayPause) as Ellipse).Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Audioplayer2;component/play.png")));

            }
            else
            {
                Player.Play();
                _timer.Start();
                playing = true;
                PlayPause.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Audioplayer2;component/pause-button.png")));
                (PlayPause.Template.FindName("Im", PlayPause) as Ellipse).Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Audioplayer2;component/pause-button.png")));
            }

        }

        private void MediaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int index = MediaListBox.SelectedIndex;
                if (index > -1)
                {
                    string path = medialist.Split('\n')[index];
                    if (path.Length < 2)
                    {
                        try
                        {
                            path = medialist.Split('\n')[index + 1];
                        }
                        catch
                        {

                        }

                    }
                    if (File.Exists(path) && !(Player.Source?.ToString().Contains(path.Replace('\\', '/')) ?? false))
                    {
                        Player.Open(new Uri(path));

                        Player.Play();
                        _timer.Start();
                        playing = true;
                        Current_Time.Value = 0;
                        CTime.Text = "0:00";
                        (PlayPause.Template.FindName("Im", PlayPause) as Ellipse).Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Audioplayer2;component/pause-button.png")));
                        TryToGetTime();
                    }
                    else
                    {
                        if (!File.Exists(path))
                        {
                            medialist = medialist.Remove(medialist.IndexOf(path), path.Length + 1);
                            MediaListBox.Items.Remove(MediaListBox.SelectedItem);
                            MediaListBox.SelectedIndex = -1;
                            Refresh();

                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListBox.SelectedIndex > 0)
            {
                MediaListBox.SelectedIndex -= 1;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListBox.SelectedIndex < MediaListBox.Items.Count-1)
            {
                MediaListBox.SelectedIndex += 1;
            }
            
        }

        private void RemoveItem(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            foreach (var item in MediaListBox.Items)
            {
                var lbi = MediaListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                string path = lbi.Tag.ToString().Split('|')[1];
                int tag = int.Parse(lbi.Tag.ToString().Split('|')[0]);
                if (lbi != null && tag == (int)btn.Tag)
                {
                    try
                    {
                        medialist = medialist.Remove(medialist.IndexOf(path), path.Length + 1);
                    }
                    catch
                    {
                        medialist = medialist.Remove(medialist.IndexOf(path), path.Length);
                    }
                    
                    MediaListBox.Items.Remove(item);
                    Refresh();
                    break;
                }

            }
        }

        private void Current_Time_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Player.Source != null)
            {
                Player.Position = TimeSpan.Parse(SecToTime(Current_Time.Value, true));
            }
            if (playing)
            {
                _timer.Start();
            }
        }

        private void Current_Time_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CTime.Text = SecToTime(Current_Time.Value, false);
        }

        private void Current_Time_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _timer.Stop();
        }
    }
}
