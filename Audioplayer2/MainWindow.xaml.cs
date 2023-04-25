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
                GetConfig(PathConfig);
            }
            else
            {
                Directory.CreateDirectory(PathConfig);
                SetConfig(PathConfig);
            }



        }
        MediaPlayer Player = new MediaPlayer();


        void SetConfig(string path)
        {


        }
        void GetConfig(string path)
        {
            string[] files = Directory.GetFiles(path, "*.cfg");
            /// LastPlayed; MediaLubrury; Queue
            





        }

        private void AddMediaButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog filediag = new Microsoft.Win32.OpenFileDialog();
            filediag.Multiselect = true;
            filediag.DefaultExt = ".mp3";
            filediag.Filter = "AudioFiles | *.mp3";
            Nullable<bool> result = filediag.ShowDialog();
            
            if (result == true)
            {
                string[] filenames = filediag.FileNames;

                Player.Open(new Uri(filenames[0]));
                Player.Play();
                TextB.Text=Player.Clock;
            }
        }

    }
}
