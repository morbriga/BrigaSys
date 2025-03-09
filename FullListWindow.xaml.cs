using BrigaSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BrigaSys
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
                    Tag = btn.Number,
                    Background = new SolidColorBrush(Colors.Gold),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };

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
                button.Template = template;

                button.Click += DialNumber;
                ButtonListPanel.Children.Add(button);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            filteredButtons = allButtons.Where(b => b.Name.ToLower().Contains(query)).ToList();
            LoadFullList();

            SearchPlaceholder.Visibility = string.IsNullOrEmpty(query) ? Visibility.Visible : Visibility.Hidden;
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
                CallWindow callWindow = new CallWindow(targetName, callHandler, extension); // Corrected line
                callWindow.Show();
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}