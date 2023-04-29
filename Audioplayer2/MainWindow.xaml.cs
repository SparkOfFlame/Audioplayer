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
                GetConfig(PathConfig);
            }
            else
            {
                Directory.CreateDirectory(PathConfig);
                SetConfig(PathConfig);
            }



        }
        MediaPlayer Player = new MediaPlayer();
        string medialist = "";
        

        void SetConfig(string path)
        {


        }
        void GetConfig(string path)
        {
            /// LastPlayed; MediaLubrury; Queue
            
            if (File.Exists(@"C:\NTPlayer\MediaLibrary.cfg"))
            { 
                StreamReader copyCheck = new StreamReader(@"C:\NTPlayer\MediaLibrary.cfg");
                medialist = copyCheck.ReadToEnd();
                copyCheck.Close();
                string[] mlist = medialist.Split('\n');
                foreach (string a in mlist)
                {
                    if (File.Exists(a))
                    {
                        AddToList(a);
                    }
                }

            }

        }

        void AddFiles(string[] path)
        {

            ListBoxItem newItem = new ListBoxItem();
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
            meta.Width = 170;
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

                Player.Open(new Uri(filenames[0]));
                Player.Play();
                AddFiles(filenames);

            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Volume = VolumeSlider.Value;

        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            Player.Pause();
        }

        private void MediaListButton_Click(object sender, RoutedEventArgs e)
        {
            

        }
    }
}
