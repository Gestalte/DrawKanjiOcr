using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Google.Cloud.Vision.V1;
using IronOcr;

namespace KanjiOcr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly string savePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "image.png");

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
            Output.Text = "";

            //string savePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "image.png") ;

            FileStream fs = new FileStream(savePath, FileMode.Create);

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Drawing.ActualWidth, (int)Drawing.ActualHeight, 96d, 96d, PixelFormats.Default);
            rtb.Render(Drawing);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            encoder.Save(fs);
            fs.Close();

            //var Result = new IronTesseract().Read(savePath).Text;

            var Result = DetectText();

            Output.Text = Result;
        }

        public string DetectText()
        {
            var image = Google.Cloud.Vision.V1.Image.FromFile(savePath);

            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);

            string output = "";

            foreach (EntityAnnotation text in textAnnotations)
            {
                output += $"Description: {text.Description}";
            }

            return output;
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
            Output.Text = "";
        }
    }
}
