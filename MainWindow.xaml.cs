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

namespace SoftphoneKiosk
{
    public partial class MainWindow : Window
    {
        private CallHandler callHandler;
        private string configPath = Path.Combine("C:\\SoftphoneKiosk", "config.csv");
        private string passwordPath = Path.Combine("C:\\SoftphoneKiosk", "admin_password.txt");
        private string companyLogoPath = Path.Combine("C:\\SoftphoneKiosk", "company_logo.png");
        private string clientLogoPath = Path.Combine("C:\\SoftphoneKiosk", "client_logo.png");

        public MainWindow()
        {
            InitializeComponent();
            callHandler = new CallHandler();
            EnsureConfigFilesExist();
            LoadButtons();
            LoadImages();
            this.PreviewMouseRightButtonDown += OpenEditMode;
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
        }

        private void LoadButtons()
        {
            List<ButtonData> extensions = LoadExtensionsFromFile();
            ButtonGrid.Children.Clear();
            ButtonGrid.RowDefinitions.Clear();
            ButtonGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < 4; i++)
                ButtonGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int i = 0; i < 3; i++)
                ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            int row = 0, col = 0;

            foreach (var extension in extensions)
            {
                Button btn = new Button
                {
                    Content = extension.Name,
                    Width = 180,
                    Height = 50,
                    Margin = new Thickness(5),
                    Tag = extension.Number
                };
                btn.Click += DialNumber;

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

        private void LoadImages()
        {
            LoadImage(companyLogoPath, CompanyLogo);
            LoadImage(clientLogoPath, ClientLogo);
        }

        private void LoadImage(string path, Image imageControl)
        {
            if (File.Exists(path))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(Path.GetFullPath(path), UriKind.Absolute);
                bitmap.EndInit();
                imageControl.Source = bitmap;
            }
        }

        private void ChangeCompanyLogo(object sender, RoutedEventArgs e)
        {
            ChangeImage(companyLogoPath);
        }

        private void ChangeClientLogo(object sender, RoutedEventArgs e)
        {
            ChangeImage(clientLogoPath);
        }

        private void ChangeImage(string path)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Images (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                File.Copy(openFileDialog.FileName, path, true);
                LoadImages();
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
                callHandler.DialNumber(extension);
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
            e.Handled = true; // מונע העברת האירוע הלאה
            string storedPassword = File.ReadAllText(passwordPath).Trim();
            string enteredPassword = Microsoft.VisualBasic.Interaction.InputBox("הזן סיסמה:", "כניסה לניהול", "");

            if (enteredPassword == storedPassword)
            {
                EditMode editWindow = new EditMode();
                editWindow.ShowDialog();
                LoadButtons();
                LoadImages(); // טען מחדש את התמונות
            }
            else
            {
                MessageBox.Show("סיסמה שגויה!", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
