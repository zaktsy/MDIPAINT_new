using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MDIPAINT_new
{
    /// <summary>
    /// Логика взаимодействия для Drawning.xaml
    /// </summary>
    public partial class Drawing : ContentControl
    {
        private MainWindow parent;

        public string filepath = String.Empty;
        
        public bool isSaved = false;


        public Drawing(MainWindow window)
        {
            InitializeComponent();
            canvas.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            canvas.Height = 300;
            canvas.Width = 300;
            parent = window;
            DataContext = this;
        }

        public Drawing(MainWindow window, string File)
        { 
            InitializeComponent();
            using (FileStream inStream = new FileStream(new Uri(File).LocalPath, FileMode.OpenOrCreate))
            {
                using (var memoryStream = new MemoryStream())
                {
                    inStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                    canvas.Height = bitmapImage.Height;
                    canvas.Width = bitmapImage.Width;
                    canvas.Background = new ImageBrush(bitmapImage);
                    bitmapImage.Freeze();
                }
            }
            filepath = File;
            parent = window;
            DataContext = this;
        }



        public Point prev;

        private Line oldLine;

        private Rectangle oldRect;

        private Ellipse oldEl;

        private bool isPaint = false;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Ellipse dot;
                switch (parent.selectedTool)
                {
                    case MainWindow.Tool.pen:
                        if (isPaint) return;
                        isPaint = true;
                        prev = Mouse.GetPosition(canvas);
                        dot = new Ellipse { Width = parent.LineWidth, Height = parent.LineHeith, Fill = new SolidColorBrush(parent.color) };
                        dot.SetValue(Canvas.LeftProperty, prev.X - parent.LineWidth/2);
                        dot.SetValue(Canvas.TopProperty, prev.Y - parent.LineHeith/2);
                        canvas.Children.Add(dot);
                        break;
                    case MainWindow.Tool.eraser:
                        if (isPaint) return;
                        isPaint = true;
                        prev = Mouse.GetPosition(canvas);
                        dot = new Ellipse { Width = parent.LineWidth, Height = parent.LineHeith, Fill = canvas.Background };
                        dot.SetValue(Canvas.LeftProperty, prev.X - parent.LineWidth / 2);
                        dot.SetValue(Canvas.TopProperty, prev.Y - parent.LineHeith / 2);
                        canvas.Children.Add(dot);
                        break;
                    case MainWindow.Tool.pouring:
                        canvas.Background = new SolidColorBrush(parent.color);
                        break;
                    case MainWindow.Tool.line:
                        prev = Mouse.GetPosition(canvas);
                        isPaint = true;
                        oldLine = new Line
                        {
                            Stroke = new SolidColorBrush(parent.color),
                            StrokeThickness = parent.LineWidth,
                            X1 = prev.X,
                            Y1 = prev.Y,
                            X2 = prev.X,
                            Y2 = prev.Y,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };
                        canvas.Children.Add(oldLine);
                        break;

                    case MainWindow.Tool.rectangle:
                        prev = Mouse.GetPosition(canvas);
                        isPaint = true;
                        oldRect = new Rectangle
                        {
                            Stroke = new SolidColorBrush(parent.color),
                            StrokeThickness = parent.LineWidth,
                        };
                        Canvas.SetLeft(oldRect, prev.X);
                        Canvas.SetTop(oldRect,prev.Y);
                        canvas.Children.Add(oldRect);
                        break;

                    case MainWindow.Tool.elipse:
                        prev = Mouse.GetPosition(canvas);
                        isPaint = true;
                        oldEl = new Ellipse
                        {
                            Stroke = new SolidColorBrush(parent.color),
                            StrokeThickness = parent.LineWidth,
                        };
                        Canvas.SetLeft(oldEl, prev.X);
                        Canvas.SetTop(oldEl, prev.Y);
                        canvas.Children.Add(oldEl);
                        break;
                }
            }
            
            
        }

       

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (parent.selectedTool)
                {
                    case MainWindow.Tool.pen:
                        if (!isPaint) return;
                        var point = Mouse.GetPosition(canvas);
                        var line = new Line
                            {
                                Stroke = new SolidColorBrush(parent.color),
                                StrokeThickness = parent.LineWidth,
                                X1 = prev.X,
                                Y1 = prev.Y,
                                X2 = point.X,
                                Y2 = point.Y,
                                StrokeStartLineCap = PenLineCap.Round,
                                StrokeEndLineCap = PenLineCap.Round
                            };
                        
                        prev = point;
                        canvas.Children.Add(line);
                        break;

                    case MainWindow.Tool.eraser:
                        if (!isPaint) return;
                        var point1 = Mouse.GetPosition(canvas);
                        var line1 = new Line
                        {
                            Stroke = canvas.Background,
                            StrokeThickness = parent.LineWidth,
                            X1 = prev.X,
                            Y1 = prev.Y,
                            X2 = point1.X,
                            Y2 = point1.Y,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };

                        prev = point1;
                        canvas.Children.Add(line1);
                        break;
                    case MainWindow.Tool.line:
                        if (!isPaint) return;
                        var point2 = Mouse.GetPosition(canvas);
                        canvas.Children.Remove(oldLine);
                        var newLine = new Line
                        {
                            Stroke = new SolidColorBrush(parent.color),
                            StrokeThickness = parent.LineWidth,
                            X1 = prev.X,
                            Y1 = prev.Y,
                            X2 = point2.X,
                            Y2 = point2.Y,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        };
                        oldLine = newLine;
                        canvas.Children.Add(newLine);
                        break;

                    case MainWindow.Tool.rectangle:
                        if (!isPaint) return;
                        var point3 = Mouse.GetPosition(canvas);
                        canvas.Children.Remove(oldRect);
                        var newRect = new Rectangle
                        {
                            Stroke = new SolidColorBrush(parent.color),
                            StrokeThickness = parent.LineWidth,
                            
                        };
                        double newXr = point3.X;
                        double newYr;
                        double oldXr = prev.X;
                        double oldYr;
                        if (Keyboard.IsKeyDown(Key.LeftShift))
                        {
                            newYr = newXr;
                            oldYr = oldXr;
                        }
                        else
                        {
                            newYr = point3.Y;
                            oldYr = prev.Y;
                        }
                        newRect.Width = Math.Abs(newXr - oldXr);
                        newRect.Height = Math.Abs(newYr - oldYr);
                        if (point3.X >= prev.X)
                        {
                            if(point3.Y >= prev.Y)
                            {
                                Canvas.SetLeft(newRect, prev.X);
                                Canvas.SetTop(newRect, prev.Y);
                            }
                            else
                            {
                                Canvas.SetLeft(newRect, prev.X);
                                Canvas.SetTop(newRect, point3.Y);
                            }
                        }
                        else
                        {
                            if (point3.Y >= prev.Y)
                            {
                                Canvas.SetLeft(newRect, point3.X);
                                Canvas.SetTop(newRect, prev.Y);
                            }
                            else
                            {
                                Canvas.SetLeft(newRect, point3.X);
                                Canvas.SetTop(newRect, point3.Y);
                            }
                        }
                        oldRect = newRect;
                        canvas.Children.Add(newRect);
                        break;

                    case MainWindow.Tool.elipse:
                        if (!isPaint) return;
                        var point4 = Mouse.GetPosition(canvas);
                        canvas.Children.Remove(oldEl);
                        var newEl = new Ellipse
                        {
                            Stroke = new SolidColorBrush(parent.color),
                            StrokeThickness = parent.LineWidth,
                        };
                        double newX = point4.X;
                        double newY;
                        double oldX = prev.X;
                        double oldY;
                        if (Keyboard.IsKeyDown(Key.LeftShift))
                        {
                            newY = newX;
                            oldY = oldX;
                        }
                        else
                        {
                            newY = point4.Y;
                            oldY = prev.Y;
                        }
                        newEl.Width = Math.Abs(newX - oldX);
                        newEl.Height = Math.Abs(newY - oldY);
                        if (point4.X >= prev.X)
                        {
                            if (point4.Y >= prev.Y)
                            {
                                Canvas.SetLeft(newEl, prev.X);
                                Canvas.SetTop(newEl, prev.Y);
                            }
                            else
                            {
                                Canvas.SetLeft(newEl, prev.X);
                                Canvas.SetTop(newEl, point4.Y);
                            }
                        }
                        else
                        {
                            if (point4.Y >= prev.Y)
                            {
                                Canvas.SetLeft(newEl, point4.X);
                                Canvas.SetTop(newEl, prev.Y);
                            }
                            else
                            {
                                Canvas.SetLeft(newEl, point4.X);
                                Canvas.SetTop(newEl, point4.Y);
                            }
                        }
                        oldEl = newEl;
                        canvas.Children.Add(newEl);
                        break;
                }
                var el = parent.ActiveItem;
                if(el.Header.Contains('*') == false) { el.Header += "*";}
                
            }  
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (parent.selectedTool)
            {
                case MainWindow.Tool.pen:
                    isPaint = false;
                    break;
                case MainWindow.Tool.eraser:
                    isPaint = false;
                    break;
                case MainWindow.Tool.line:
                    isPaint = false;
                    break;
                case MainWindow.Tool.rectangle:
                    isPaint = false;
                    break;
                case MainWindow.Tool.elipse:
                    isPaint = false;
                    break;
            }

        }


        private void CloseClick(object sender, RoutedEventArgs e)
        {
            if (isSaved)
            {
                parent.ActiveItem.CanClose = true;
                parent.ActiveItem.State = Syncfusion.Windows.Tools.Controls.DockState.Hidden;
            }
            else
            {
                MessageBoxResult res = MessageBox.Show("У вас есть несохраненные изменения. Сохранить?","Сохранение", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Yes)
                {
                    if (filepath != String.Empty) 
                    { 
                        parent.Save();
                        parent.ActiveItem.CanClose = true;
                        parent.ActiveItem.State = Syncfusion.Windows.Tools.Controls.DockState.Hidden;
                    }
                    else 
                    {
                        if (parent.SaveAS())
                        {
                            parent.ActiveItem.CanClose = true;
                            parent.ActiveItem.State = Syncfusion.Windows.Tools.Controls.DockState.Hidden;
                        } 
                    }
                    
                }
                else if (res == MessageBoxResult.No)
                {
                    parent.ActiveItem.CanClose = true;
                    parent.ActiveItem.State = Syncfusion.Windows.Tools.Controls.DockState.Hidden;
                }

            }
        }

    }
}
