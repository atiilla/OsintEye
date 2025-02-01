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
        private readonly AuthService _authService;
        private string _email;
        private string _password;
        private bool _isBusy;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
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

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                LoginCommand.ChangeCanExecute();
            }
        }

        public Command LoginCommand { get; }
        public Command GoToRegisterCommand { get; }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await OnLoginClicked(), () => !IsBusy);
            GoToRegisterCommand = new Command(OnRegisterClicked);
        }

        private async Task OnLoginClicked()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter email and password", "OK");
                    return;
                }

                var result = await _authService.Login(Email, Password);

                if (result)
                {
                    await Shell.Current.GoToAsync("///MainPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid email or password", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "An error occurred while logging in", "OK");
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
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