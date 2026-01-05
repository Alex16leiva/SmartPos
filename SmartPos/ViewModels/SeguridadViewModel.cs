using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.DTOs.Seguridad;
using Aplicacion.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartPos.Comunes.CommonServices;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartPos.ViewModels
{
    public partial class SeguridadViewModel : ObservableObject
    {
        private readonly ISecurityApplicationService _securityService;
        private readonly ICommonService _commonService;

        public SeguridadViewModel(ISecurityApplicationService securityService, ICommonService commonService)
        {
            _securityService = securityService;
            _commonService = commonService;

            _usuarios = new ObservableCollection<UsuarioDTO>();
            _roles = new ObservableCollection<RolDTO>();
            _matrizPermisos = new ObservableCollection<PermisosDTO>();

            _ = LoadDataAsync();
        }

        [ObservableProperty] private ObservableCollection<UsuarioDTO> _usuarios;
        [ObservableProperty] private ObservableCollection<RolDTO> _roles;
        [ObservableProperty] private ObservableCollection<PermisosDTO> _matrizPermisos;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private RolDTO _rolSeleccionado;
        [ObservableProperty] private string _textoBusqueda = string.Empty;
        private CancellationTokenSource _searchCts;
        [ObservableProperty] private int _paginaActual = 1;
        [ObservableProperty] private int _registrosPorPagina = 10;
        [ObservableProperty] private UsuarioDTO _usuarioSeleccionado;
        [ObservableProperty] private bool _isNuevoUsuario;
        [ObservableProperty] private bool _isEditFlyoutUsuarioOpen;

        async partial void OnTextoBusquedaChanged(string value)
        {
            // Cancelar la búsqueda pendiente si el usuario sigue escribiendo
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            try
            {
                // Esperar 400ms de silencio antes de disparar la consulta
                await Task.Delay(400, _searchCts.Token);

                PaginaActual = 1;
                await LoadDataAsync();
            }
            catch (TaskCanceledException) { /* Ignorar cancelación */ }
        }

        private QueryInfo ObtenerQueryInfoArticulo()
        {
            var queryInfo = new QueryInfo
            {
                PageIndex = PaginaActual - 1,
                PageSize = RegistrosPorPagina, 
                SortFields = new List<string> { "FechaTransaccion" },
                Ascending = false,
                Predicate = string.Empty,
                ParamValues = []
            };

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                // Ajustado para buscar por Nombre o Id del Artículo
                queryInfo.Predicate = "UsuarioId.ToString().Contains(@0) OR Nombre.Contains(@0)";
                queryInfo.ParamValues = new object[] { TextoBusqueda };
            }

            return queryInfo;
        }

        // Carga inicial (Equivalente a useEffect en React)
        [RelayCommand]
        public async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                // Carga de Roles (Asíncrono en tu backend)
                var rolesList = await _securityService.ObtenerRoles();
                Roles = new ObservableCollection<RolDTO>(rolesList);

                // Carga de Usuarios (Síncrono en tu backend, lo envolvemos en Task)
                await Task.Run(() => {
                    var userRequest = new GetUserRequest
                    {
                        QueryInfo = ObtenerQueryInfoArticulo()
                    };
                    var result = _securityService.ObtenerUsuario(userRequest);
                    Usuarios = new ObservableCollection<UsuarioDTO>(result.Items);
                });
            }
            catch (Exception ex)
            {
                _commonService.ShowError($"Error: {ex.Message}");
            }
            finally { IsBusy = false; }
        }

        // Lógica de Matriz (Inspirada en fetchPantallas + mapeo de permisos de React)
        partial void OnRolSeleccionadoChanged(RolDTO value)
        {
            if (value == null) { MatrizPermisos.Clear(); return; }

            var pantallas = _securityService.ObtenerPantallas();

            // Replicamos el mapeo que haces en el DataGrid de React (screens.map...)
            var matriz = pantallas.Select(s => new PermisosDTO
            {
                PantallaId = s.PantallaId,
                RolId = value.RolId,
                // Buscamos si el rol ya tiene este permiso
                Ver = value.Permisos?.FirstOrDefault(p => p.PantallaId == s.PantallaId)?.Ver ?? false,
                Editar = value.Permisos?.FirstOrDefault(p => p.PantallaId == s.PantallaId)?.Editar ?? false,
                Eliminar = value.Permisos?.FirstOrDefault(p => p.PantallaId == s.PantallaId)?.Eliminar ?? false
            }).ToList();

            MatrizPermisos = new ObservableCollection<PermisosDTO>(matriz);
        }

        [RelayCommand]
        private async Task GuardarPermisosAsync()
        {
            if (RolSeleccionado == null) return;

            IsBusy = true;
            try
            {
                // Mismo objeto request que usas en handleSavePermissions de React
                var request = new EdicionPermisosRequest
                {
                    RolId = RolSeleccionado.RolId,
                    Permisos = MatrizPermisos.ToList(),
                    RequestUserInfo = _commonService.GetRequestInfo()
                };

                await Task.Run(() => _securityService.EdicionPermisos(request));
                _commonService.ShowSuccess("Permisos actualizados correctamente");
                await LoadDataAsync();
            }
            catch (Exception ex) { _commonService.ShowError(ex.Message); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private void NuevoUsuario()
        {
            // Preparamos un DTO vacío para el formulario
            UsuarioSeleccionado = new UsuarioDTO
            {
                UsuarioId = string.Empty,
                Nombre = string.Empty,
                RolId = string.Empty,
                Contrasena = string.Empty
            };
            IsNuevoUsuario = true;
            IsEditFlyoutUsuarioOpen = true;
        }

        [RelayCommand]
        private async Task GuardarUsuarioAsync(object parameter)
        {
            // 1. Extraer la contraseña del control de forma segura
            if (parameter is System.Windows.Controls.PasswordBox passwordBox)
            {
                string contrasenaClara = passwordBox.Password;

                // Si es nuevo usuario y no hay contraseña, podrías validar aquí
                if (IsNuevoUsuario && string.IsNullOrEmpty(contrasenaClara))
                {
                    _commonService.ShowError("La contraseña es obligatoria para nuevos usuarios.");
                    return;
                }

                // Asignar la contraseña al DTO antes de enviar
                UsuarioSeleccionado.Contrasena = contrasenaClara;
            }

            if (string.IsNullOrEmpty(UsuarioSeleccionado.UsuarioId)) return;

            try
            {
                var request = new EdicionUsuarioRequest
                {
                    Usuario = UsuarioSeleccionado,
                    RequestUserInfo = _commonService.GetRequestInfo()
                };

                
                if (IsNuevoUsuario) _securityService.CrearUsuario(request);
                else _securityService.EditarUsuario(request);
            
                _commonService.ShowSuccess("Operación exitosa");
                IsNuevoUsuario = false;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _commonService.ShowError(ex.Message);
            }
        }
    }
}