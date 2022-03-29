﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using MessageBox = System.Windows.Forms.MessageBox;

namespace HuntHighLightBackUper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DispatcherTimer scanner;
        private XmlParser parser;

        public event PropertyChangedEventHandler PropertyChanged;
        public string TempFolderPath { get => parser.Settings.TempFolderPath; set { parser.Settings.TempFolderPath = value; RaisePropertyChanged(); } }
        public string SaveFolderPath { get => parser.Settings.SaveFolderPath; set { parser.Settings.SaveFolderPath = value; RaisePropertyChanged(); } }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            parser = new XmlParser();
            parser.LoadSettings();

            scanner = new DispatcherTimer();
            scanner.Tick += Scanner_Tick;
            scanner.Interval = TimeSpan.FromMilliseconds(2000);
            scanner.Start();
        }

        private void Scanner_Tick(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Temp\Highlights\Hunt  Showdown";
            if (Directory.Exists(path))
            {
                string[] paths = Directory.GetFiles(path);

                if (paths.Length == 0) return;
                else
                {
                    foreach (string p in paths)
                    {
                        string filename = p.Split('\\').Last();
                        if (File.Exists(SaveFolderPath + '\\' + filename)) continue;
                        File.Copy(p, SaveFolderPath + '\\' + filename);
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            scanner.Stop();
            parser.SaveSettings();
        }

        public void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void TempFolder_textBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TempFolderPath = dialog.SelectedPath;
            }
        }

        private void SaveFolder_textBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveFolderPath = dialog.SelectedPath;
            }
        }
    }

    public class XmlParser
    {
        public XmlSerializer Serializer;
        public Settings Settings;

        public XmlParser()
        {
            Serializer = new XmlSerializer(typeof(Settings));
        }

        public void SaveSettings()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.xml"))
                {
                    Serializer.Serialize(writer, Settings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadSettings()
        {
            try
            {
                if (File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.xml"))
                {
                    using (StreamReader reader = new StreamReader(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.xml"))
                    {
                        Settings = Serializer.Deserialize(reader) as Settings;
                    }
                }
                else
                {
                    Settings = new Settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class Settings
    {
        public string TempFolderPath { get; set; }
        public string SaveFolderPath { get; set; }
        public Settings()
        {
            TempFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Temp\Highlights\Hunt  Showdown";
            SaveFolderPath = @"D:\";
        }
    }
}
