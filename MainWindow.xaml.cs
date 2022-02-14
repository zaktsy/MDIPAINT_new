using Microsoft.Win32;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int x;
        public int y;
        private ObservableCollection<DockItem> manager;
        public ObservableCollection<DockItem> Manager { get { return manager; } set { manager = value; } }
        public DockItem Tools;

        public Tool selectedTool = Tool.pen;

        private DockItem activeItem;
        public DockItem ActiveItem { get { return activeItem; } set { activeItem = value; } }

        private int WindowNumber = 0;

        public MainWindow()
        {
            InitializeComponent();
            SfSkinManager.SetTheme(this, new Theme("FluentDark"));


            dockManager.ActiveWindowChanged += new PropertyChangedCallback(Docking_ActiveWindowChanged);
            Manager = new ObservableCollection<DockItem>();

            Tools = new() { Content = new Tools(this), SideInDockedMode= DockSide.Right, Header = "Инструменты", Name = "Tools", CanMaximize = false, CanResizeHeightInFloatState = false };
            Manager.Add(Tools);

            dockManager.ItemsSource = Manager;
            this.DataContext = this;
        }

        public bool SaveAS()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpg)|*.jpg| Файлы PNG (*.png)|*.png";
            if (dlg.ShowDialog() == true)
            {
                Uri uri = new Uri(dlg.FileName);
                ExportTo(uri);
                var el = (Drawing)ActiveItem.Content;
                el.filepath = dlg.FileName;
                save.IsEnabled = true;
                var fInfo = new FileInfo(dlg.FileName);
                ActiveItem.Header = fInfo.Name;
                el.isSaved = true;
                return true;
            }
            return false;
        }
        public void Save()
        {
            var el = (Drawing)ActiveItem.Content;
            var fInfo = new FileInfo(el.filepath);
            ActiveItem.Header = fInfo.Name;
            ExportTo(new Uri(el.filepath));
            el.isSaved = true;
        }


        void Docking_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((dockManager.ActiveWindow != null) && (dockManager.ActiveWindow.Name != Tools.Name))
            {
                string name = dockManager.ActiveWindow.Name;
                foreach (DockItem item in Manager)
                {
                    if (item.Name == name) { ActiveItem = item; }
                }
                saveas.IsEnabled = true;
                resize.IsEnabled = true;
                focus.Text = name;
                var dr = (Drawing)ActiveItem.Content;
                if (dr.filepath != String.Empty) { save.IsEnabled = true; }
            }
            else
            {
                ActiveItem = null;
                save.IsEnabled = false;
                saveas.IsEnabled = false;
                resize.IsEnabled = false;
                focus.Text = string.Empty;
                x = 0;
                y = 0;
                Coordinates.Text = string.Format("{0} : {1}", x, y);
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (ActiveItem != null)
            {
                var el = (Drawing)ActiveItem.Content;
                var canvas = el.canvas;
                x = (int)Mouse.GetPosition(canvas).X;
                y = (int)Mouse.GetPosition(canvas).Y;
                Coordinates.Text = string.Format("{0} : {1}", x, y);
            }

        }

        #region Buttons
        private void NewFileClick(object sender, RoutedEventArgs e)
        {
            DockItem dItem = new();
            dItem.Name = string.Format("doc{0}", WindowNumber + 1);
            dItem.Header = string.Format("Новый документ");
            WindowNumber++;
            dItem.CanClose = false;
            dItem.Content = new Drawing(this);
            Manager.Add(dItem);
            saveas.IsEnabled = true;
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg| Файлы PNG (*.png)|*.png";
            if (dlg.ShowDialog() == true)
            {
                FileInfo info = new FileInfo(dlg.FileName);
                var filename = info.Name;
                var ext = info.Extension;
                DockItem dItem = new();
                dItem.Header = filename;
                dItem.Name = string.Format("doc{0}", WindowNumber + 1);
                dItem.CanClose = false;
                dItem.Content = new Drawing(this, dlg.FileName);
                Manager.Add(dItem);
            }

        }

        private void saveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAS();
        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
        private void tools_Click(object sender, RoutedEventArgs e)
        {
            if (Tools.State == DockState.Hidden)
            {
                Tools.State = DockState.Dock;
            }
            else
            {
                Tools.State = DockState.Hidden;
            }
        }

        private void resize_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ResizeDialog();
            var el = (Drawing)ActiveItem.Content;
            dlg.h = (int)el.canvas.Height;
            dlg.w = (int)el.canvas.Width;
            if (dlg.ShowDialog() == true)
            {
                
                if(dlg.h!=0 && dlg.w != 0)
                {
                    el.canvas.Height = dlg.h;
                
                    el.canvas.Width = dlg.w;
                    el.canvas.UpdateLayout();
                    el.isSaved = false;
                }
                
            }
            el.isSaved = false;
            if (ActiveItem.Header.Contains('*') == false) { ActiveItem.Header += "*"; }
        }
        #endregion


        public enum Tool
        {
            line, elipse, eraser, rectangle, pouring, pen
        }


        public Color color = Colors.Black;
        public int LineHeith = 1;
        public int LineWidth = 1;

        
        public void ExportTo(Uri path)
        {
            if (path == null) return;
            var el = (Drawing)ActiveItem.Content;
            var canvas = el.canvas;
            Transform transform = canvas.LayoutTransform;
            canvas.LayoutTransform = null;

            Size size = new Size(canvas.Width, canvas.Height);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);

            var fInfo = new FileInfo(path.ToString());
            var ext = fInfo.Extension;
            switch (ext)
            {
                case ".png":
                    using (FileStream outStream = new FileStream(path.LocalPath, FileMode.OpenOrCreate))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                        encoder.Save(outStream);
                    }
                    break;
                case ".jpg":
                    using (FileStream outStream = new FileStream(path.LocalPath, FileMode.OpenOrCreate))
                    {
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                        encoder.Save(outStream);
                    }
                    break;
                case ".jpeg":
                    using (FileStream outStream = new FileStream(path.LocalPath, FileMode.OpenOrCreate))
                    {
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                        encoder.Save(outStream);
                    }
                    break;
                case ".bmp":
                    using (FileStream outStream = new FileStream(path.LocalPath, FileMode.OpenOrCreate))
                    {
                        BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                        encoder.Save(outStream);
                    }
                    break;
            }
            
            canvas.LayoutTransform = transform;
        }

        private void dockManager_DockStateChanged(FrameworkElement sender, DockStateEventArgs e)
        {
            FrameworkElement deletedElement = new FrameworkElement();
            foreach (FrameworkElement felement in dockManager.Children)
            {
                if (DockingManager.GetState(felement) == DockState.Hidden)
                {
                    deletedElement = felement;
                }

            }
            if (deletedElement != null)
            {
                int number = -1;
                for (int i = 0; i < Manager.Count; i++)
                {
                    if (Manager[i].Name == deletedElement.Name) { number = i; }
                }
                if (number != -1)
                {
                    Manager.RemoveAt(number);
                    dockManager.ActiveWindow = null;

                }
            }
        }
    }
}
