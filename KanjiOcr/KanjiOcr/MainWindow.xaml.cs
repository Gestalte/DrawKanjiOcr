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

namespace KanjiOcr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Drawing.Strokes.Clear();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string savePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "image.jpg") ;

            FileStream fs = new FileStream(savePath, FileMode.Create);

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Drawing.ActualWidth, (int)Drawing.ActualHeight, 96d, 96d, PixelFormats.Default);
            rtb.Render(Drawing);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            encoder.Save(fs);
            fs.Close();

            MessageBox.Show($"File saved to: {savePath}");
        }

        private void drawing_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Drawing.Children.Clear();

            double x = e.GetPosition(Drawing).X;
            double y = e.GetPosition(Drawing).Y;

            Ellipse elipsa = new Ellipse
            {
                StrokeThickness = 3,
                Stroke = Brushes.Red,
                Margin = new Thickness(x, y, 0, 0),
                Width = 20,
                Height = 20
            };

            Drawing.Children.Add(elipsa);
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            Drawing.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            Drawing.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }
    }
}
