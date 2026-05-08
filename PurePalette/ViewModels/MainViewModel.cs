using PurePalette.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PurePalette.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<ImageItemViewModel> ImageItems { get; }

        private int _activeDownloadsCount;
        public int ActiveDownloadsCount
        {
            get => _activeDownloadsCount;
            private set => SetProperty(ref _activeDownloadsCount, value);
        }

        public ICommand LoadAllCommand { get; }

        public MainViewModel()
        {
            var downloader = new ImageDownloader();
            ImageItems = new ObservableCollection<ImageItemViewModel>();

            for (int i = 0; i < 3; i++)
            {
                var item = new ImageItemViewModel(downloader);
                item.LoadingStateChanged += OnItemLoadingStateChanged;

                ImageItems.Add(item);
            }

            LoadAllCommand = new RelayCommand(ExecuteLoadAll, CanExecuteLoadAll);
        }

        private void OnItemLoadingStateChanged()
        {
            ActiveDownloadsCount = ImageItems.Count(item => item.IsLoading);
        }

        private bool CanExecuteLoadAll(object? parameter)
        {
            return ImageItems.Any(item => !string.IsNullOrWhiteSpace(item.Url) && !item.IsLoading);
        }

        private void ExecuteLoadAll(object? parameter)
        {
            foreach (var item in ImageItems)
            {
                if (!string.IsNullOrWhiteSpace(item.Url) && !item.IsLoading)
                    item.StartCommand.Execute(null);
            }
        }
    }
}
