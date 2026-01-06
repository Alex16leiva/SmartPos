using Aplicacion.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dominio.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.Comunes.CommonServices;
using System.Windows;

namespace SmartPos.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ISecurityApplicationService _securityAplicationService;
        private readonly ICommonService _commonService;

        public LoginViewModel(ISecurityApplicationService securityAplicationService,
            ICommonService commonService)
        {
            _securityAplicationService = securityAplicationService;
            _commonService = commonService;
        }

        [ObservableProperty] private string _username;
        [ObservableProperty] private string _password;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _errorMessage;

        [RelayCommand]
        private async Task Login(object parameter)
        {
            // Casteamos el parámetro al tipo PasswordBox de WPF
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;

            // Obtenemos el texto (esto es lo que antes te llegaba vacío)
            string passwordClara = passwordBox?.Password;

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(passwordClara))
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
                        Password = passwordClara
                    });

            if (result.IsNotNull() && result.UsuarioAutenticado)
            {
                _commonService.SetearRequestInfo(result, 1);

                // Obtenemos la MainWindow desde nuestro contenedor de dependencias
                var mainWindow = App.ServiceProvider.GetService<SmartPos.Views.MainWindow>();

                // 3. ¡PASO CLAVE!: Obtener el ViewModel de la MainWindow y cargar menús
                if (mainWindow.DataContext is MainViewModel mainVM)
                {
                    mainVM.CargarMenus();
                }

                mainWindow.Show();

                var loginWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is SmartPos.Views.LoginView);
                loginWindow?.Close();

            }
            else
            {
                _commonService.ShowWarning(result.Message);
            }

            IsBusy = false;
        }
    }
}
