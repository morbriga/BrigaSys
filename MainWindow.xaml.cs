using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.VisualBasic;
using BrigaSys;
using System.Windows.Media.Animation;

namespace BrigaSys
{
    public partial class MainWindow : Window
    {
        private CallHandler callHandler;
        private string configPath = Path.Combine("C:\\BrigaSys", "config.csv");
        private string passwordPath = Path.Combine("C:\\BrigaSys", "admin_password.txt");
        private string companyLogoPath = Path.Combine("C:\\BrigaSys", "company_logo.png");
        private string clientLogoPath = Path.Combine("C:\\BrigaSys", "client_logo.png");
        private string titlePath = Path.Combine("C:\\BrigaSys", "title.txt");

        public MainWindow()
        {
            InitializeComponent();
            callHandler = new CallHandler();
            EnsureConfigFilesExist();
            LoadButtons();
            LoadImages();
            LoadTitle();
            this.PreviewMouseRightButtonDown += OpenEditMode;
            InitializeClock();
        }

        private void InitializeClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Clock_Tick;
            timer.Start();
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            Clock.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void EnsureConfigFilesExist()
        {
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, "שלוחה1,100\nשלוחה2,101\nשלוחה3,102", Encoding.UTF8);
            }
            if (!File.Exists(passwordPath))
            {
                File.WriteAllText(passwordPath, "1234", Encoding.UTF8); // סיסמת ברירת מחדל
            }
            if (!File.Exists(titlePath))
            {
                File.WriteAllText(titlePath, "מערכת כריזה");
            }
        }

        public void LoadButtons()
        {
            List<ButtonData> extensions = LoadExtensionsFromFile();
            ButtonGrid.Children.Clear();

            int row = 0, col = 0;

            foreach (var extension in extensions)
            {
                Button btn = new Button
                {
                    Content = extension.Name,
                    Width = 200,
                    Height = 100,
                    Margin = new Thickness(10),
                    Tag = extension.Number,
                    Background = new SolidColorBrush(Colors.Gold),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 24,
                    FontWeight = FontWeights.Bold
                };

                btn.Click += DialNumber;

                ControlTemplate template = new ControlTemplate(typeof(Button));
                FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
                border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
                border.SetValue(Border.BorderBrushProperty, Brushes.Black);
                border.SetValue(Border.BorderThicknessProperty, new Thickness(2));
                border.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
                FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
                contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
                border.AppendChild(contentPresenter);
                template.VisualTree = border;
                btn.Template = template;

                Grid.SetRow(btn, row);
                Grid.SetColumn(btn, col);
                ButtonGrid.Children.Add(btn);

                col++;
                if (col > 2)
                {
                    col = 0;
                    row++;
                }
            }
        }

        public void LoadImages()
        {
            LoadImage(companyLogoPath, CompanyLogoPlaceholder);
            LoadImage(clientLogoPath, ClientLogoPlaceholder);
        }

        private void LoadImage(string path, Border placeholder)
        {
            if (File.Exists(path))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(Path.GetFullPath(path), UriKind.Absolute);
                bitmap.EndInit();

                ImageBrush brush = new ImageBrush(bitmap);
                brush.Stretch = Stretch.Uniform;
                placeholder.Background = brush;
            }
        }

        public void LoadTitle()
        {
            if (File.Exists(titlePath))
            {
                TitleTextBlock.Text = File.ReadAllText(titlePath);
            }
        }

        private List<ButtonData> LoadExtensionsFromFile()
        {
            List<ButtonData> extensions = new List<ButtonData>();
            if (File.Exists(configPath))
            {
                var lines = File.ReadAllLines(configPath, Encoding.UTF8);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        extensions.Add(new ButtonData { Name = parts[0].Trim(), Number = parts[1].Trim() });
                    }
                }
            }
            return extensions;
        }

        private void DialNumber(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string extension)
            {
                if (callHandler == null)
                {
                    callHandler = new CallHandler();
                }
                string targetName = button.Content.ToString();
                CallWindow callWindow = new CallWindow(targetName, callHandler, extension);
                callWindow.Show();
            }
        }

        private void OpenFullList(object sender, RoutedEventArgs e)
        {
            List<ButtonData> extensions = LoadExtensionsFromFile();
            FullListWindow fullListWindow = new FullListWindow(extensions);
            fullListWindow.ShowDialog();
        }

        private void OpenEditMode(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            LoginWindow loginWindow = new LoginWindow(this);
            loginWindow.ShowDialog();
        }
    }
}