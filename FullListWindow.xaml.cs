using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SoftphoneKiosk
{
    public partial class FullListWindow : Window
    {
        private List<ButtonData> allButtons;
        private List<ButtonData> filteredButtons;
        private CallHandler callHandler;


        public FullListWindow(List<ButtonData> buttons)
        {
            InitializeComponent();
            allButtons = buttons;
            filteredButtons = new List<ButtonData>(allButtons);
            LoadFullList();
        }

        private void LoadFullList()
        {
            ButtonListPanel.Children.Clear();
            foreach (var btn in filteredButtons)
            {
                Button button = new Button
                {
                    Content = btn.Name,
                    Width = 250,
                    Height = 50,
                    Margin = new Thickness(5),
                    Tag = btn.Number
                };
                button.Click += DialNumber;
                ButtonListPanel.Children.Add(button);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            filteredButtons = allButtons.Where(b => b.Name.ToLower().Contains(query)).ToList();
            LoadFullList();
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

        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
