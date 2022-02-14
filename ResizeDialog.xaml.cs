using Syncfusion.SfSkinManager;
using System;
using System.Windows;
using System.Windows.Input;

namespace MDIPAINT_new
{
    /// <summary>
    /// Логика взаимодействия для ResizeDialog.xaml
    /// </summary>
    public partial class ResizeDialog : Window
    {
        public ResizeDialog()
        {
            InitializeComponent();
            SfSkinManager.SetTheme(this, new Theme("FluentDark"));
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) < 0;
        }

        private void TextBox_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789".IndexOf(e.Text) < 0;
        }

        public int h
        {
            get { return Convert.ToInt32(height.Text); }
            set { height.Text = value.ToString(); }
        }
        public int w
        {
            get { return Convert.ToInt32(width.Text); }
            set { width.Text = value.ToString(); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
