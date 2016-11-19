using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Emotion.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Emotion
{
    public class EmotionViewModel : INotifyPropertyChanged
    {
        private ImageSource _selectedImage;
        private bool _isBusy = false;
        private string _msgString;

        public string Message
        {
            get { return _msgString; }
            set
            {
                SetProperty(ref _msgString,value);
            }
        }
        public ImageSource SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                SetProperty(ref _selectedImage,value);
            }
        }
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                SetProperty(ref _isBusy,value);
            }
        }

        public EmotionViewModel()
        {
            this.TakePhotoCommand = new Command(async () => await TakePhoto());
            this.PickPhotoCommand = new Command(async () => await SelectPhoto());
        }

        public ICommand TakePhotoCommand { get; protected set; }
        public ICommand PickPhotoCommand { get; protected set; }
        public async Task TakePhoto()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                Message = ":(No camera avaialble.";
                IsBusy = false;
                return;
            }
            try
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "EmotionCheckPic.jpg",
                    SaveToAlbum = true
                });

                await ProcessPhoto(file);
            }
            catch (Exception ex)
            {
                Message = "Uh oh :( Something went wrong \n " + ex.Message;
                IsBusy = false;
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async Task SelectPhoto()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                Message = "Permission not granted to photos.";
                IsBusy = false;
                return;
            }
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);
                await ProcessPhoto(file);

            }
            catch (Exception ex)
            {
                Message = "Uh oh :( Something went wrong \n" + ex.Message;
                IsBusy = false;
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task ProcessPhoto(MediaFile file)
        {
            if (file == null) return;

            using (file)
            {
                SelectedImage = ImageSource.FromStream(() => file.GetStream());
                var selectedPicStream = file.GetStream();
                var emotion = await EmotionService.GetAverageHappinessScoreAsync(selectedPicStream);
                Message = EmotionService.GetHappinessMessage(emotion);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T value,T newvalue,[CallerMemberName] string name = "")
        {
            value = newvalue;
            OnPropertyChanged(name);
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}