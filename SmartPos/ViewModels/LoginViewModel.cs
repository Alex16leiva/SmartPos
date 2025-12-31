using Aplicacion.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dominio.Context.Entidades.Seguridad;
using Dominio.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace SmartPos.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly SecurityAplicationService _securityAplicationService;

        public LoginViewModel(SecurityAplicationService securityAplicationService)
        {
            _securityAplicationService = securityAplicationService;
        }

        [ObservableProperty] private string _username;
        [ObservableProperty] private string _password;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _errorMessage;

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Por favor, ingrese sus credenciales.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            var result = _securityAplicationService.IniciarSesion(
                    new Aplicacion.DTOs.Seguridad.UserRequest 
                    {
                        UsuarioId = Username,
                        Password = Password
                    });

            if (result.IsNotNull())
            {
                // Obtenemos la MainWindow desde nuestro contenedor de dependencias
                var mainWindow = App.ServiceProvider.GetService<SmartPos.Views.MainWindow>();
                mainWindow.Show();

                var loginWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is SmartPos.Views.LoginView);
                loginWindow?.Close();

            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
            }

            IsBusy = false;
        }
    }
}
