using System;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using Launcher.Enums;
using Launcher.Views;
using Launcher.Classes;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new MainView(Window.GetWindow(this)));
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            WrapClass.Unwrap(this);
            this.WindowState = WindowState.Normal;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Minimized:
                    WrapClass.Wrap(this);
                    break;
            }
        }
    }
}
