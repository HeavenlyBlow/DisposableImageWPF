using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DisposableImageWPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private DisposableImage _image;
        private Command _onLoadedCommand;
        private Command _dispose;
        private Command _updateCommand;
        private Command _initImageCommand;

        public DisposableImage Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        
        public MainViewModel()
        {
            Image = new DisposableImage("Resources/Images/Diev.jpg", 255, 360);
        }
        
        
        public ICommand DisposeCommand => _dispose ??= new Command(a =>
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                Image.Dispose();
            });
            
        });
        
        
        public ICommand InitImageCommand => _initImageCommand ??= new Command(a =>
        {
            Image = new DisposableImage("Resources/Images/Diev.jpg", 255, 360);
        });
        

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