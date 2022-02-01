using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<MainWindow>()
                .Build();

            var credPath = config.GetSection("CredentialsFilePath").Value;

            System.Environment.SetEnvironmentVariable
                ("GOOGLE_APPLICATION_CREDENTIALS"
                , credPath
                );
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Drawing.Strokes.Clear();
            Output.Text = "";
        }

        private void MakeImage()
        {
            FileStream fs = new FileStream(savePath, FileMode.Create);

            RenderTargetBitmap rtb = new RenderTargetBitmap
                ((int)Drawing.ActualWidth
                , (int)Drawing.ActualHeight
                , 96d
                , 96d
                , PixelFormats.Default
                );

            rtb.Render(Drawing);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            encoder.Save(fs);
            fs.Close();
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            HightlightButton(btnSubmit);

            Output.Text = "Searching, please wait...";

            MakeImage();

            try
            {
                var Result = await DetectText();
                Output.Text = "";
                Output.Text = Result;
            }
            catch (Exception ex)
            {
                Output.Text = "";
                MessageBox.Show(ex.Message, "Error:");
            }

            HightlightButton(btnWrite);
        }

        public async Task<string> DetectText()
        {
            var image = await Google.Cloud.Vision.V1.Image.FromFileAsync(savePath);

            ImageAnnotatorClient client = await ImageAnnotatorClient.CreateAsync();

            IReadOnlyList<EntityAnnotation> textAnnotations = await client.DetectTextAsync(image);

            string output = null;

            //foreach (EntityAnnotation text in textAnnotations)
            //{
            //    output += (text.Description == "" ? "FAIL" : text.Description) + "\n";
            //}

            output = textAnnotations.FirstOrDefault()?.Description;

            return output ?? "No matches";
        }

        public void HightlightButton(Button btn)
        {
            btnWrite.BorderBrush = Brushes.LightPink;
            btnClear.BorderBrush = Brushes.LightPink;
            btnErase.BorderBrush = Brushes.LightPink;
            btnSubmit.BorderBrush = Brushes.LightPink;

            btn.BorderBrush = Brushes.DeepPink;
        }

        private void drawing_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Drawing.Children.Clear();

            double x = e.GetPosition(Drawing).X;
            double y = e.GetPosition(Drawing).Y;

            var penTip = new Ellipse
            {
                StrokeThickness = 3,
                Stroke = Brushes.Red,
                Margin = new Thickness(x, y, 0, 0),
                Width = 20,
                Height = 20
            };

            Drawing.Children.Add(penTip);
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            HightlightButton(btnWrite);

            Drawing.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            HightlightButton(btnErase);

            Drawing.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }
    }
}
