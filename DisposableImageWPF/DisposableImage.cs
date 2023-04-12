using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;
using Size = System.Drawing.Size;

namespace DisposableImageWPF
{
    public class DisposableImage : IDisposable, INotifyPropertyChanged
    {
        
        private Stream _mediaStream;
        private Stream _previewStream;
        private BitmapImage _source;
        private ImageSource _preview;


        /// <summary>
        /// Уничтожаемая картинка
        /// </summary>
        /// <param name="path">Путь до картинки</param>
        public DisposableImage(string path)
        {
            Convert(path);
        }
        
        /// <summary>
        /// Уничтожаемая картинка с превью
        /// </summary>
        /// <param name="path">путь до картикни</param>
        /// <param name="pWidth">Ширина превью</param>
        /// <param name="pHeight">Высота превью</param>
        public DisposableImage(string path, int pWidth, int pHeight)
        {
            Convert(path, pWidth, pHeight);
        }
        
        
        /// <summary>
        /// Картинка
        /// </summary>
        public BitmapImage Source
        {
            get => _source;
            private set
            {
                _source = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Превью картинки
        /// </summary>
        public ImageSource Preview
        {
            get => _preview;
            set
            {
                _preview = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Добавляем картинку
        /// </summary>
        /// <param name="path">путь</param>
        private void Convert(string path)
        {
            if (path == null) return;
            
            var bitmap = new BitmapImage();
                
            _mediaStream = path.Contains("pack://application:,,,") ? Application.GetResourceStream(new Uri(path))?.Stream : File.OpenRead(path);

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.StreamSource = _mediaStream;
            bitmap.EndInit();
            bitmap.Freeze();
            Source = bitmap;
        }

        private void Convert(string path, int pWidth, int pHeight)
        {
            if (path == null) return;
            
            _previewStream = path.Contains("pack://application:,,,") ? Application.GetResourceStream(new Uri(path))?.Stream : File.OpenRead(path);

            using (var img = Image.FromStream(_previewStream!))
            {
                using (var preview = new Bitmap(img, new Size(pWidth, pHeight)))
                {
                    Preview = (BitmapSource)Imaging.CreateBitmapSourceFromHBitmap(
                        preview.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
            
            var bitmap = new BitmapImage();
            
            _mediaStream = path.Contains("pack://application:,,,") ? Application.GetResourceStream(new Uri(path))?.Stream : File.OpenRead(path);

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.StreamSource = _mediaStream;
            bitmap.EndInit();
            bitmap.Freeze();
            Source = bitmap;
        }
        
        

        /// <summary>
        /// Уничтожить картинку
        /// </summary>
        public void Dispose()
        {
            if (_mediaStream == null) return;
            Source = null;
            Preview = null;
            _mediaStream.Close();
            _mediaStream.Dispose();
            _previewStream?.Close();
            _previewStream?.Dispose();
            _mediaStream = null;
            _previewStream = null;

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}