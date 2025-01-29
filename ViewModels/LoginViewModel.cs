using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using MauiApp1.Services;

namespace MauiApp1.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly MockAuthService _authService;
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public Command LoginCommand { get; }
        public Command GoToRegisterCommand { get; }

        public LoginViewModel(MockAuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(OnLoginClicked);
            GoToRegisterCommand = new Command(OnRegisterClicked);
        }

        private async void OnLoginClicked()
        {
            if (_authService.Login(Username, Password))
            {
                await Shell.Current.GoToAsync("///MainPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Invalid username or password", "OK");
            }
        }

        private async void OnRegisterClicked()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 