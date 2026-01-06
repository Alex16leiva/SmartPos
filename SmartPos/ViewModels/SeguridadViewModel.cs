using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.DTOs.Seguridad;
using Aplicacion.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dominio.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.Comunes.CommonServices;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartPos.ViewModels
{
    public partial class SeguridadViewModel : ObservableObject
    {
        // Inyectamos la fábrica, no el servicio directamente
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICommonService _commonService;

        public SeguridadViewModel(IServiceScopeFactory scopeFactory, ICommonService commonService)
        {
            _scopeFactory = scopeFactory;
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
        [ObservableProperty] private int _totalPaginas = 0;
        [ObservableProperty] private int _registrosPorPagina = 10;
        [ObservableProperty] private UsuarioDTO _usuarioSeleccionado;
        [ObservableProperty] private bool _isNuevoUsuario;
        [ObservableProperty] private bool _isNuevoRol;
        [ObservableProperty] private bool _isEditFlyoutUsuarioOpen;
        [ObservableProperty] private bool _isEditFlyoutRolOpen;
        public List<int> TamañosPagina { get; } = new() { 10, 20, 30, 50, 100 };

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
                using (var scope = _scopeFactory.CreateScope())
                {
                    var securityService = scope.ServiceProvider.GetRequiredService<ISecurityApplicationService>();
                    // Carga de Roles (Asíncrono en tu backend)
                    var rolesList = await securityService.ObtenerRoles();
                    Roles = new ObservableCollection<RolDTO>(rolesList);

                    // Carga de Usuarios (Síncrono en tu backend, lo envolvemos en Task)
                    await Task.Run(() =>
                    {
                        var userRequest = new GetUserRequest
                        {
                            QueryInfo = ObtenerQueryInfoArticulo()
                        };
                        var result = securityService.ObtenerUsuario(userRequest);
                        Usuarios = new ObservableCollection<UsuarioDTO>(result.Items);
                        TotalPaginas = result.PageCount;
                    });
                }
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
            using (var scope = _scopeFactory.CreateScope())
            {
                var securityService = scope.ServiceProvider.GetRequiredService<ISecurityApplicationService>();
                var pantallas = securityService.ObtenerPantallas();


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
        }

        [RelayCommand]
        private async Task GuardarPermisosAsync()
        {
            if (RolSeleccionado == null) return;

            IsBusy = true;
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var securityService = scope.ServiceProvider.GetRequiredService<ISecurityApplicationService>();
                    // Mismo objeto request que usas en handleSavePermissions de React
                    var request = new EdicionPermisosRequest
                    {
                        RolId = RolSeleccionado.RolId,
                        Permisos = MatrizPermisos.ToList(),
                        RequestUserInfo = _commonService.GetRequestInfo()
                    };

                    await Task.Run(() => securityService.EdicionPermisos(request));
                    _commonService.ShowSuccess("Permisos actualizados correctamente");
                }
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
                Contrasena = string.Empty,
                EditarContrasena = true,
            };
            IsNuevoUsuario = true;
            IsEditFlyoutUsuarioOpen = true;
        }

        [RelayCommand]
        private void NuevoRol()
        {
            RolSeleccionado = new RolDTO
            {
                Descripcion = string.Empty,
                RolId = string.Empty
            };
            IsNuevoRol = true;
            IsEditFlyoutRolOpen = true;
        }

        [RelayCommand]
        private void EditarRol(RolDTO rol)
        {
            RolSeleccionado = new RolDTO
            {
                RolId = rol.RolId,
                Descripcion = rol.Descripcion,
            };
            IsEditFlyoutRolOpen = true;
            IsNuevoRol = false;
        }
        
        [RelayCommand]
        private void EditarUsuario(UsuarioDTO usuario)
        {
            UsuarioSeleccionado = usuario;
            IsNuevoUsuario = false;
            IsEditFlyoutUsuarioOpen = true;
        }

        [RelayCommand]
        public async Task GuardarRol()
        {
            try
            {
                if (RolSeleccionado == null) return;
                // Creamos un Scope único para esta transacción
                using (var scope = _scopeFactory.CreateScope())
                {
                    var securityService = scope.ServiceProvider.GetRequiredService<ISecurityApplicationService>();

                    var request = new EdicionRolRequest
                    {
                        Rol = RolSeleccionado,
                        RequestUserInfo = _commonService.GetRequestInfo()
                    };

                    RolDTO resultado;
                    // Lógica para decidir si es creación o edición
                    if (IsNuevoRol)
                    {
                        resultado = await Task.Run(() => securityService.CrearRol(request));
                    }
                    else
                    {
                        // Asumiendo que existe este método en tu servicio
                        resultado = await Task.Run(() => securityService.EditarRol(request));
                    }

                    if (resultado.Message.IsMissingValue())
                    {
                        // Notificar éxito (puedes usar tu CommonService)
                        _commonService.ShowSuccess("Rol guardado correctamente");
                        IsEditFlyoutRolOpen = false; // Cerrar el panel
                        await LoadDataAsync(); // Recargar la lista
                    }
                    else
                    {
                        _commonService.ShowError(resultado.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.ShowError("Error al procesar el rol: " + ex.Message);
            }
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
                UsuarioSeleccionado.EditarContrasena = UsuarioSeleccionado.Contrasena.HasValue();
                using (var scope = _scopeFactory.CreateScope())
                {
                    var securityService = scope.ServiceProvider.GetRequiredService<ISecurityApplicationService>();
                    var request = new EdicionUsuarioRequest
                    {
                        Usuario = UsuarioSeleccionado,
                        RequestUserInfo = _commonService.GetRequestInfo()
                    };

                    UsuarioDTO response = null;

                    if (IsNuevoUsuario)
                    {
                        response = securityService.CrearUsuario(request);
                    }
                    else
                    {
                        response = securityService.EditarUsuario(request);
                    }

                    if (response.HasValidationMessage())
                    {
                        _commonService.ShowWarning(response.Message);
                        return;
                    }

                    _commonService.ShowSuccess("Operación exitosa");
                    IsNuevoUsuario = false;
                    IsEditFlyoutUsuarioOpen = false;
                }
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _commonService.ShowError(ex.Message);
            }
        }

        partial void OnRegistrosPorPaginaChanged(int value)
        {
            // Reiniciamos a la página 1
            PaginaActual = 1;

            // Ejecutamos la carga de datos
            // Usamos el método directamente ya que estamos en el mismo VM
            _ = LoadDataAsync();
        }

        [RelayCommand]
        private async Task SiguientePagina()
        {
            if (PaginaActual < TotalPaginas)
            {
                PaginaActual++;
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task AnteriorPagina()
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                await LoadDataAsync();
            }
        }
    }
}