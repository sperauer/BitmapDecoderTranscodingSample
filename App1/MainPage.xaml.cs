using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public static async Task<IRandomAccessStream> ResizeImageStream(IRandomAccessStream imageStream)
        {
            var decoder = await BitmapDecoder.CreateAsync(imageStream);

            var resizedStream = new InMemoryRandomAccessStream();

            var encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);

            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;

            // ********** InvalidCastException happens here for heif images from iPhones ***********************
            await encoder.FlushAsync();

            resizedStream.Seek(0);
            return resizedStream;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            };

            var types = new List<string> { ".jpg", ".heic", ".heif", ".png", ".gif" };
            foreach (var type in types)
                openPicker.FileTypeFilter.Add(type);

            var file = await openPicker.PickSingleFileAsync();

            var imageStream = await file.OpenAsync(FileAccessMode.ReadWrite);

            var resizedStream = await ResizeImageStream(imageStream);

            var wb = new WriteableBitmap(1, 1);
            await wb.SetSourceAsync(resizedStream);

            TheImage.Source = wb;
        }
    }
}
