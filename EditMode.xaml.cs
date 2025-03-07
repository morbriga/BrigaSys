using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace SoftphoneKiosk
{
    public partial class EditMode : Window
    {
        private List<ButtonData> buttons = new List<ButtonData>();
        private string filePath = Path.Combine("C:\\SoftphoneKiosk", "config.csv");

        public EditMode()
        {
            InitializeComponent();
            LoadButtons();
        }

        private void LoadButtons()
        {
            buttons.Clear();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        buttons.Add(new ButtonData { Name = parts[0].Trim(), Number = parts[1].Trim() });
                    }
                }
            }
            RefreshButtonList();
        }

        private void SaveButtons()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    foreach (var btn in buttons)
                    {
                        writer.WriteLine($"{btn.Name},{btn.Number}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving buttons.csv: " + ex.Message);
            }
        }

        private void RefreshButtonList()
        {
            ButtonListPanel.Children.Clear();
            foreach (var btn in buttons)
            {
                StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                TextBox nameBox = new TextBox { Text = btn.Name, Width = 150, Margin = new Thickness(5) };
                TextBox numberBox = new TextBox { Text = btn.Number, Width = 100, Margin = new Thickness(5) };
                Button deleteButton = new Button { Content = "🗑️", Width = 30, Height = 30, Margin = new Thickness(5) };
                deleteButton.Click += (s, e) => { buttons.Remove(btn); RefreshButtonList(); };
                stackPanel.Children.Add(nameBox);
                stackPanel.Children.Add(numberBox);
                stackPanel.Children.Add(deleteButton);
                ButtonListPanel.Children.Add(stackPanel);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            buttons.Add(new ButtonData { Name = "חדש", Number = "0000" });
            RefreshButtonList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButtons();
            this.Close();
        }
    }
}
