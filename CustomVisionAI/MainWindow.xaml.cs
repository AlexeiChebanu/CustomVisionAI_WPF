using CustomVisionAI.Entities;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace CustomVisionAI
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog= new OpenFileDialog();
            dialog.Filter = "image files (*.png; *.jpg)|*.png;*.jpg;*.jpeg|All files(*.*)|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if(dialog.ShowDialog()==true)
            {
                string filename = dialog.FileName;

                selectedImage.Source = new BitmapImage(new Uri(filename));

                MakePredictionAsync(filename);
            }
        }

        private async void MakePredictionAsync(string filename)
        {
            string url = "https://westeurope.api.cognitive.microsoft.com/customvision/v3.0/Prediction/fbf0280d-4f8e-4031-a7c4-841a0c368603/classify/iterations/Iteration1/image";

            string prediction_key = "75d8c6c409af457d8d9c61fbaa6e65d8";

            string content_type = "application/octet-stream";

            var file = File.ReadAllBytes(filename);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", prediction_key);

                using(var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                    var response = await client.PostAsync(url, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction> predictions = (JsonConvert.DeserializeObject<CustomVision>(responseString)).predictions;

                    predictionsListView.ItemsSource = predictions;
                }
            }

        }
    }
}
