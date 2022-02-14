using System.Windows;
using System.Windows.Controls;

namespace MDIPAINT_new
{
    /// <summary>
    /// Логика взаимодействия для Tools.xaml
    /// </summary>
    public partial class Tools : ContentControl
    {
        private MainWindow parent;

        public Tools(MainWindow window)
        {
            InitializeComponent();
            parent = window;
        }


        private void ColorPalett_ColorChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            parent.color = ColorPalett.Color;
            foreach (var el in parent.Manager)
            {
                if (el.Content.GetType() == typeof(Drawing))
                {
                    var item = (Drawing)el.Content;
                }

            }
        }


        private void width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            parent.LineWidth = (int)width.Value;
            foreach (var el in parent.Manager)
            {
                if (el.Content.GetType() == typeof(Drawing))
                {
                    var item = (Drawing)el.Content;
                }

            }
        }



        private void pen_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var el in parent.Manager)
            {
                if (el.Content.GetType() == typeof(Drawing))
                {
                    var el1 = (Drawing)el.Content;
                }
            }
            pen.IsChecked = true;
            parent.selectedTool = MainWindow.Tool.pen;
        }

        private void eraser_Checked(object sender, RoutedEventArgs e)
        {

            foreach (var el in parent.Manager)
            {
                if (el.Content.GetType() == typeof(Drawing))
                {
                    var el1 = (Drawing)el.Content;
                }

            }
            eraser.IsChecked = true;
            parent.selectedTool = MainWindow.Tool.eraser;

        }

        private void pouring_Checked(object sender, RoutedEventArgs e)
        {

            foreach (var el in parent.Manager)
            {
                if (el.Content.GetType() == typeof(Drawing))
                {
                    var el1 = (Drawing)el.Content;

                }

            }
            pouring.IsChecked = true;
            parent.selectedTool = MainWindow.Tool.pouring;

        }


        private void line_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var el in parent.Manager)
            {
                if (el.Content.GetType() == typeof(Drawing))
                {
                    var el1 = (Drawing)el.Content;
                }

            }
            line.IsChecked = true;
            parent.selectedTool = MainWindow.Tool.line;
        }

        private void rectangle_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var el in parent.Manager)
            {
                if (el.Content.GetType() == typeof(Drawing))
                {
                    var el1 = (Drawing)el.Content;
                }

            }
            rectangle.IsChecked = true;
            parent.selectedTool = MainWindow.Tool.rectangle;
        }

        private void elipse_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var el in parent.Manager)
            {
                if (el.Content.GetType() == typeof(Drawing))
                {
                    var el1 = (Drawing)el.Content;
                }

            }
            elipse.IsChecked = true;
            parent.selectedTool = MainWindow.Tool.elipse;
        }
    }
}
