using Launcher.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
using static System.Net.WebRequestMethods;

namespace Launcher.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Page
    {
        public SettingsView()
        {
            InitializeComponent();
            if(Properties.Settings.Default.ChooseServer == 0)
                VersionBox.SelectedIndex = 0;
            if(Properties.Settings.Default.ChooseServer == 1)
                VersionBox.SelectedIndex = 1;
        }

        private void AutoCheck_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoUpdate = AutoCheck.IsChecked ?? false;
            Settings.Default.Save();
            Settings.Default.Reload();
        }

        private void ArchiveCheck_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ArchiveUpdate = ArchiveCheck.IsChecked ?? false;
            Settings.Default.Save();
            Settings.Default.Reload();
        }

        private void VersionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(VersionBox.SelectedIndex == 0)
            {
                Properties.Settings.Default.ChooseServer = 0;
                Properties.Settings.Default.VersionServerPath = "https://drive.google.com/uc?export=download&id=1kN0i9akJJHIGmV1s2mH-Nt_o5Cm0YnTS";
                Properties.Settings.Default.UpdateServerPath = "https://drive.google.com/uc?export=download&id=1AmMd2EgmPVQzcIes6uPlieWm6OgUQJfe";
                Properties.Settings.Default.DescServerPath = "https://drive.google.com/uc?export=download&id=1yO8xBlb6ThIbtsPPPvD5jXgaq1UxqxKa";
                try
                {
                    WebClient webClient = new WebClient();
                    Classes.Version onlineVersion = new Classes.Version(webClient.DownloadString(Properties.Settings.Default.VersionServerPath));
                }
                catch { 
                    MessageBox.Show("Сервер недоступен!", "Ошибка!",MessageBoxButton.OK, MessageBoxImage.Error);                
                }
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
            if (VersionBox.SelectedIndex == 1)
            {
                Properties.Settings.Default.ChooseServer = 1;
                Properties.Settings.Default.VersionServerPath = "https://drive.google.com/uc?export=download&id=1D-pzmXBgrKr2bkFng-2aCDijPfCvkkLy";
                Properties.Settings.Default.UpdateServerPath = "https://drive.google.com/uc?export=download&id=1KNGM5aMBnn2paTjEZ9ySgAl1uD6ADT0I";
                Properties.Settings.Default.DescServerPath = "https://drive.google.com/uc?export=download&id=1oiwk_NK0QKHEpHKuq3lwZMZ0IJkpQpVR";
                try
                {
                    WebClient webClient = new WebClient();
                    Classes.Version onlineVersion = new Classes.Version(webClient.DownloadString(Properties.Settings.Default.VersionServerPath));
                }
                catch
                {
                    MessageBox.Show("Сервер недоступен!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
        }

        private void RollbackButton_Click(object sender, RoutedEventArgs e)
        {
            string exe = Properties.Settings.Default.PathToExe.Replace("\\update", "\\Archiv");
            ProcessStartInfo info = new ProcessStartInfo(exe);
            info.WorkingDirectory = exe.Replace("\\Practica.exe", "");
            Process.Start(info);
            var window = Window.GetWindow(this);
            window.Close();
        }
    }
}
