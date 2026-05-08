using System;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PurePalette.Services;

namespace PurePalette.ViewModels
{
    public class ImageItemViewModel : ObservableObject
    {
        private readonly ImageDownloader _downloader;
        private CancellationTokenSource? _cts;

        private string _url = string.Empty;

        public string Url
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        private BitmapImage? _image;

        public BitmapImage? Image
        {
            get => _image;
            private set => SetProperty(ref _image, value);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }


        public event Action? LoadingStateChanged;

        public ImageItemViewModel(ImageDownloader downloader)
        {
            _downloader = downloader;
            StartCommand = new RelayCommand(ExecuteStart, CanExecuteStart);
            StopCommand = new RelayCommand(ExecuteStop, CanExecuteStop);
        }

        private bool CanExecuteStart(object? parameter) => !string.IsNullOrWhiteSpace(Url) && !IsLoading;
        private bool CanExecuteStop(object? parameter) => IsLoading;

        private void ExecuteStop(object? parameter)
        {
            _cts?.Cancel();
        }

        private async void ExecuteStart(object? parameter)
        {
            IsLoading = true;
            LoadingStateChanged?.Invoke();
            Image = null;

            _cts = new CancellationTokenSource();

            var imageBytes = await _downloader.DownloadImageAsync(Url, _cts.Token);

            if (imageBytes != null)
            {
                Image = CreateBitmapImage(imageBytes);
            }

            IsLoading = false;
            LoadingStateChanged?.Invoke();
        }

        private static BitmapImage? CreateBitmapImage(byte[] bytes)
        {
            try
            {
                using var stream = new MemoryStream(bytes);
                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }

            catch
            {
                return null;
            }
        }
    }
}