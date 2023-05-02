using System;
using System.IO;
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



namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string PathConfig = @"C:\NTPlayer";
            if (Directory.Exists(PathConfig))
            {
                GetConfig();
            }
            else
            {
                Directory.CreateDirectory(PathConfig);
                SetConfig();
                
            }
            MediaListBox.Width = 201;


        }
        MediaPlayer Player = new MediaPlayer();
        string medialist = "";
        int nowplaying=-1;
        bool playing = true;
        

        void SetConfig()
        {


        }
        void GetConfig()
        {
            /// LastPlayed; MediaLubrury; Queue
            
            if (File.Exists(@"C:\NTPlayer\MediaLibrary.cfg"))
            {
                ///var clock=Player.Clock;
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
                    medialist += "\n" + a;
                    AddToList(a);
                }
            }
            addfile.Close();
        }

        void AddToList(string a)
        {

            var meta = new TextBlock();
            meta.Width = 120;
            meta.TextWrapping = TextWrapping.Wrap;
            string[] aboba = a.Split('\\');
            meta.Text = aboba[aboba.Length - 1];    
            MediaListBox.Items.Add(meta);

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
            if (playing)
            {
                Player.Pause();
                playing = false;
                (PlayPause.Template.FindName("Im", PlayPause) as Ellipse).Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Audioplayer2;component/play.png")));

            }
            else
            {
                Player.Play();
                playing = true;
                PlayPause.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Audioplayer2;component/pause-button.png")));
                (PlayPause.Template.FindName("Im", PlayPause) as Ellipse).Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Audioplayer2;component/pause-button.png")));
            }
            
        }

        private void MediaListButton_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void MediaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = MediaListBox.SelectedIndex;
            TextB.Text = index.ToString();
            if (index > -1)
            {
                string path = medialist.Split('\n')[index];
                if (File.Exists(path) && nowplaying !=index)
                {
                    nowplaying = index;
                    Player.Open(new Uri(path));
                    
                    Player.Play();
                   
                }
                else
                {
                    if (!File.Exists(path))
                    {
                        medialist=medialist.Remove(medialist.IndexOf(path), path.Length+1);
                        MediaListBox.Items.Remove(MediaListBox.SelectedItem);
                        Refresh();
                        TextB.Text += "Файл удалён";
                    }
                }
                

            }
            
            
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
