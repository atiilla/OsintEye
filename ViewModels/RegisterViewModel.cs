using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _username;
        private string _password;
        private string _email;


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

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public Command RegisterCommand { get; }

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            RegisterCommand = new Command(OnRegisterClicked);
        }


        private async void OnRegisterClicked()
        {
            var user = new User
            {
                Username = Username,
                Password = Password,
                Email = Email
            };

            if (await _authService.Register(user))
            {
                await Shell.Current.DisplayAlert("Success", "Registration successful!", "OK");
                await Shell.Current.GoToAsync("///MainPage");
            }

            else
            {
                await Shell.Current.DisplayAlert("Error", "Username or email already exists", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 