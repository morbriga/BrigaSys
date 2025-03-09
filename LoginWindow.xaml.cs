using BrigaSys;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrigaSys
{
    public partial class LoginWindow : Window
    {
        private string passwordPath = Path.Combine("C:\\BrigaSys", "admin_password.txt");
        private MainWindow mainWindow;
        private string companyLogoPath = Path.Combine("C:\\BrigaSys", "company_logo.png");

        public LoginWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            PasswordBox.KeyDown += PasswordBox_KeyDown;
            LoadImage();
        }

        public LoginWindow()
        {
            InitializeComponent();
            PasswordBox.KeyDown += PasswordBox_KeyDown;
            LoadImage();
        }

        private void LoadImage()
        {
            if (File.Exists(companyLogoPath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(Path.GetFullPath(companyLogoPath), UriKind.Absolute);
                bitmap.EndInit();

                ImageBrush brush = new ImageBrush(bitmap);
                brush.Stretch = Stretch.Uniform;
                CompanyLogoPlaceholder.Background = brush;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Authenticate();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Authenticate();
            }
        }

        private void Authenticate()
        {
            string enteredPassword = PasswordBox.Password.Trim();
            string storedPassword = File.ReadAllText(passwordPath).Trim();

            if (enteredPassword == storedPassword)
            {
                EditMode editWindow = new EditMode();
                editWindow.ShowDialog();
                mainWindow.LoadButtons();
                mainWindow.LoadImages();
                mainWindow.LoadTitle();
                this.Close();
            }
            else
            {
                MessageBox.Show("סיסמה שגויה!", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}