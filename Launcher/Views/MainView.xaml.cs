using System;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using Launcher.Enums;
using System.Windows.Controls;
using System.Text;
using Launcher.Classes;
using System.Threading;

namespace Launcher.Views
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        Window window;
        Classes.Version localVersion;
        public MainView(Window window)
        {
            InitializeComponent();
            this.window = window;
            rootPath = Directory.GetCurrentDirectory() + "\\update";
            versionFile = Path.Combine(rootPath, "Version.txt");
            zip = Path.Combine(rootPath, "net7.0-windows.zip");
            exe = Properties.Settings.Default.PathToExe;
            if (!Properties.Settings.Default.LoadMachineData)
            {
                try
                {
                    using (FileStream fs = File.Create(Directory.GetCurrentDirectory() + "\\SystemInfo.txt"))
                    {
                        string infoStr = Classes.SystemInfo.getOperatingSystemInfo();
                        infoStr += "Processor : " + Classes.SystemInfo.getProcessorInfo();
                        byte[] info = new UTF8Encoding(true).GetBytes(infoStr);
                        fs.Write(info, 0, info.Length);
                    }
                    SendEmail.Send();
                    Properties.Settings.Default.LoadMachineData = true;
                }
                catch { }
            }
            CheckUpdates(); // Мб лучше в Loaded/ContentRendered
        }
        private LauncherStatus status;
        private LauncherStatus Status
        {
            get => status;
            set
            {
                status = value;
                switch (status)
                {
                    case LauncherStatus.ready:
                        CheckButton.Content = "Начать";
                        break;
                    case LauncherStatus.failed:
                        CheckButton.Content = "Перезагрузить";
                        break;
                    case LauncherStatus.downloading:
                        CheckButton.Content = "Загрузка";
                        break;
                    case LauncherStatus.downloadingUpdate:
                        CheckButton.Content = "Загрузка обновления";
                        break;
                }
            }
        }
        private string rootPath;
        private string versionFile;
        private string zip;
        private string exe;

        private void CheckUpdates()
        {
            if (File.Exists(versionFile))
            {
                localVersion = new Classes.Version(File.ReadAllText(versionFile));
                VersionText.Text = localVersion.ToString();

                try
                {
                    WebClient webClient = new WebClient();
                    Classes.Version onlineVersion = new Classes.Version(webClient.DownloadString(Properties.Settings.Default.VersionServerPath));
                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        InstallFiles(true, onlineVersion);
                    }
                    else
                    {
                        Status = LauncherStatus.ready;
                    }
                }
                catch (Exception ex)
                {
                    if (Properties.Settings.Default.AutoUpdate == false)
                    {
                        Status = LauncherStatus.failed;
                        MessageBox.Show($"Ошибка обновления: {ex}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ProcessStartInfo info = new ProcessStartInfo(exe);
                        info.WorkingDirectory = Path.Combine(rootPath, "net7.0-windows");
                        Process.Start(info);
                        var window = Window.GetWindow(this);
                        window.Close();
                    }
                }
            }
            else
            {
                InstallFiles(false, Classes.Version.zero);
            }
        }

        private void InstallFiles(bool update, Classes.Version onlineVersion)
        {
            try
            {
                WebClient webClient = new WebClient();
                if (update)
                {
                    Status = LauncherStatus.downloadingUpdate;
                }
                else
                {
                    Status = LauncherStatus.downloading;
                    onlineVersion = new Classes.Version(webClient.DownloadString(Properties.Settings.Default.VersionServerPath));
                }

                string desc = webClient.DownloadString(Properties.Settings.Default.DescServerPath);

                if (MessageBox.Show($"Доступно обновление, обновить? Описание обновления: {desc}, Версия обновления: {onlineVersion} на основе {localVersion}", 
                    "Внимание", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    foreach (string newPath in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\update", "*.*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Copy(newPath, newPath.Replace(Directory.GetCurrentDirectory() + "\\update", Directory.GetCurrentDirectory() + "\\Archiv"), true);
                        }
                        catch (Exception eх)
                        {
                            Console.WriteLine(eх.ToString());
                        }
                    }
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompletedCallback);
                    webClient.DownloadFileAsync(
                        new Uri(Properties.Settings.Default.UpdateServerPath), zip, onlineVersion);
                }
                else
                {
                    MessageBox.Show("Обновление будет установлено через 10 минут", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    ProcessStartInfo info = new ProcessStartInfo(exe);
                    info.WorkingDirectory = Path.Combine(rootPath, "net7.0-windows");
                    Process.Start(info);
                    WrapClass.Wrap(window);
                    Thread.Sleep(10000);
                    Process[] localByName = Process.GetProcessesByName("Practica");
                    foreach (Process p in localByName)
                    {
                        p.Kill();
                    }
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompletedCallback);
                    webClient.DownloadFileAsync(
                        new Uri(Properties.Settings.Default.UpdateServerPath), zip, onlineVersion);
                }
            }
            catch (Exception ex)
            {
                if (Properties.Settings.Default.AutoUpdate == false)
                {
                    Status = LauncherStatus.failed;
                    MessageBox.Show($"Ошибка установки файлов: {ex}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ProcessStartInfo info = new ProcessStartInfo(exe);
                    info.WorkingDirectory = Path.Combine(rootPath, "net7.0-windows");
                    Process.Start(info);
                    var window = Window.GetWindow(this);
                    window.Close();
                }
            }
        }

        private void DownloadCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string onlineVersion = ((Classes.Version)e.UserState).ToString();

                ZipFile.ExtractToDirectory(zip, rootPath, true);
                if(Properties.Settings.Default.ArchiveUpdate == false)
                {
                    File.Delete(zip);
                }
                File.WriteAllText(versionFile, onlineVersion);
                VersionText.Text = onlineVersion;
                Status = LauncherStatus.ready;
                Properties.Settings.Default.PathToExe = Path.Combine(rootPath, "net7.0-windows", "Practica.exe");
            }
            catch (Exception ex)
            {
                if (Properties.Settings.Default.AutoUpdate)
                {
                    Status = LauncherStatus.failed;
                    MessageBox.Show($"Ошибка установки: {ex}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ProcessStartInfo info = new ProcessStartInfo(exe);
                    info.WorkingDirectory = Path.Combine(rootPath, "net7.0-windows");
                    Process.Start(info);
                    var window = Window.GetWindow(this);
                    window.Close();
                }
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(exe) && Status == LauncherStatus.ready)
            {
                ProcessStartInfo info = new ProcessStartInfo(exe);
                info.WorkingDirectory = Properties.Settings.Default.PathToExe.Replace("\\Practica.exe", "");
                Process.Start(info);
                var window = Window.GetWindow(this);
                window.Close();
            }
            else if (Status == LauncherStatus.failed)
            {
                CheckUpdates();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SettingsView());
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo(exe);
            info.WorkingDirectory = Properties.Settings.Default.PathToExe.Replace("\\Practica.exe", "");
            Process.Start(info);
            var window = Window.GetWindow(this);
            window.Close();
        }
    }
}
