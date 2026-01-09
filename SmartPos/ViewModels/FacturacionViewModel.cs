using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.DTOs.Factura;
using Aplicacion.DTOs.Finanzas;
using Aplicacion.DTOs.Vendedores;
using Aplicacion.Services.ArticuloServices;
using Aplicacion.Services.Factura;
using Aplicacion.Services.Finanzas;
using Aplicacion.Services.VendedorSevices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dominio.Context.Entidades;
using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.Comunes.CommonServices;
using SmartPos.DTOs.Articulos;
using SmartPos.Views;
using System.Collections.ObjectModel;

namespace SmartPos.ViewModels
{
    public partial class FacturacionViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<FacturaDetalleDTO> _facturaDetalle = new();
        [ObservableProperty] private FacturaEncabezadoDTO _encabezado = new();
        [ObservableProperty] private string _busquedaArticulo = string.Empty;

        [ObservableProperty] private ObservableCollection<ArticulosDTO> _articulosBusqueda = new();
        [ObservableProperty] private ArticulosDTO? _articuloBusquedaSeleccionado;
        [ObservableProperty] private BatchDTO _batchActual;
        [ObservableProperty] private string _textoBusquedaModal = string.Empty;
        [ObservableProperty] private bool _isBusy;

        [ObservableProperty] private ObservableCollection<VendedorDTO> _vendedores = new();
        [ObservableProperty] private VendedorDTO _vendedorSeleccionado = new();
        

        // Propiedades de Paginación
        [ObservableProperty] private int _paginaActual = 1;
        [ObservableProperty] private int _totalPaginas = 0;
        [ObservableProperty] private int _registrosPorPagina = 10;
        private CancellationTokenSource? _searchCts;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICommonService _commonService;
        public FacturacionViewModel(IServiceScopeFactory scopeFactory, ICommonService commonService)
        {
            _commonService = commonService;
            _scopeFactory = scopeFactory;

            CargarVendedoresAsync();
            ObtenerBatch();
        }

        public void CargarVendedoresAsync()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var vendedorService = scope.ServiceProvider.GetRequiredService<IVendedorApplicationService>();

                    // Reutilizamos tu lógica de QueryInfo si el servicio es paginado, 
                    // o un método simple de "ObtenerTodos"
                    var request = new ObtenerVendedor
                    {
                        QueryInfo = new QueryInfo { PageSize = 100, Ascending = true, SortFields = new() { "Nombre" } }
                    };

                    var result = vendedorService.ObtenerVendedores(request);

                    if (result.HasItems())
                    {
                        Vendedores = new ObservableCollection<VendedorDTO>(result);

                        // Opcional: Seleccionar el primer vendedor por defecto
                        VendedorSeleccionado = Vendedores.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.ShowError($"Error al cargar vendedores: {ex.Message}");
            }
        }

        [RelayCommand]
        private void AgregarArticulo(string codigo)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var facturaAppService = scope.ServiceProvider.GetRequiredService<IFacturaApplicationService>();
                FacturaRequest request = new FacturaRequest
                {
                    ArticuloId = BusquedaArticulo.Trim(),
                    Vendedor = VendedorSeleccionado ?? new VendedorDTO(),
                    FacturasDetalle = FacturaDetalle.ToList()
                };
                var response = facturaAppService.AgregarArticuloAFactura(request);

                if (response.Message.HasValue())
                {
                    _commonService.ShowWarning(response.Message);
                    return;
                }
                FacturaDetalle = new ObservableCollection<FacturaDetalleDTO>(response.FacturaDetalle);
                BusquedaArticulo = string.Empty;
                ActualizarTotales();
            }
        }

        [RelayCommand]
        private void EliminarArticulo(FacturaDetalleDTO detalle)
        {
            if (detalle == null) return;

            // 1. Remover de la colección observable
            FacturaDetalle.Remove(detalle);

            CalcularFacturaDetalle();

            ActualizarTotales();
        }

        public void ActualizacionDeDataGrid()
        {
            CalcularFacturaDetalle();

            ActualizarTotales();
        }

        private void CalcularFacturaDetalle()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var facturaAppService = scope.ServiceProvider.GetRequiredService<IFacturaApplicationService>();
                FacturaRequest request = new FacturaRequest
                {
                    FacturasDetalle = FacturaDetalle.ToList()
                };

                var response = facturaAppService.CalcularFacturaDetalle(request);
                FacturaDetalle = new ObservableCollection<FacturaDetalleDTO>(response.FacturaDetalle);
            }

        }

        [RelayCommand]
        public async Task LoadDataBusquedaAsync()
        {
            IsBusy = true;

            var request = new ArticuloRequest
            {
                QueryInfo = new QueryInfo
                {
                    PageIndex = PaginaActual - 1,
                    PageSize = RegistrosPorPagina,
                    SortFields = new List<string> { "Descripcion" },
                    Ascending = true,
                    Predicate = !string.IsNullOrWhiteSpace(TextoBusquedaModal)
                                ? "ArticuloId.ToString().Contains(@0) OR Descripcion.Contains(@0)"
                                : string.Empty,
                    ParamValues = !string.IsNullOrWhiteSpace(TextoBusquedaModal)
                                  ? new object[] { TextoBusquedaModal }
                                  : Array.Empty<object>()
                }
            };

            using (var scope = _scopeFactory.CreateScope())
            {
                var articuloService = scope.ServiceProvider.GetRequiredService<IArticuloApplicationService>();
                var result = articuloService.ObtenerArticulos(request);

                ArticulosBusqueda = new ObservableCollection<ArticulosDTO>(result.Items);
                TotalPaginas = result.PageCount;
            }
            IsBusy = false;
        }

        // Debouncing para la búsqueda en el modal
        async partial void OnTextoBusquedaModalChanged(string value)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(400, _searchCts.Token);
                PaginaActual = 1;
                await LoadDataBusquedaAsync();
            }
            catch (TaskCanceledException) { }
        }


        [RelayCommand]
        private void AbrirBuscadorArticulos()
        {
            // Reiniciar búsqueda al abrir
            TextoBusquedaModal = string.Empty;
            PaginaActual = 1;
            _ = LoadDataBusquedaAsync();

            var vistaBusqueda = new BusquedaArticuloView();
            vistaBusqueda.DataContext = this;

            if (vistaBusqueda.ShowDialog() == true)
            {
                if (ArticuloBusquedaSeleccionado != null)
                {
                    BusquedaArticulo = ArticuloBusquedaSeleccionado.ArticuloId;
                    AgregarArticulo(BusquedaArticulo);
                }
            }
        }

        [RelayCommand]
        private void LimpiarPantalla()
        {
            // 1. Vaciar la colección de detalles
            FacturaDetalle.Clear();

            // 2. Reiniciar el encabezado (esto limpia Subtotal, Impuesto y Total)
            Encabezado = new FacturaEncabezadoDTO();

            // 3. Resetear el vendedor y cliente seleccionado si es necesario
            VendedorSeleccionado = null;
            BusquedaArticulo = string.Empty;

            // 4. Notificar a la UI que los totales ahora son cero
            OnPropertyChanged(nameof(Encabezado));

            // Opcional: Mostrar un mensaje breve en la barra de estado
            // _commonService.ShowMessage("Pantalla limpia para nueva venta.");
        }

        public void ActualizarTotales()
        {
            Encabezado.SubTotal = FacturaDetalle.Sum(x => x.SubTotal);
            Encabezado.Total = FacturaDetalle.Sum(x => x.Total);
            Encabezado.Impuesto = FacturaDetalle.Sum(x => x.Impuesto);
            Encabezado.Descuento = FacturaDetalle.Sum(x => x.Descuento);
            
            OnPropertyChanged(nameof(Encabezado)); // Notifica a la UI
        }

        private async Task ObtenerBatch()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var finanzasService = scope.ServiceProvider.GetRequiredService<IFinanzasApplicationService>();

                var request = new BatchRequest
                {
                    
                    RequestUserInfo = _commonService.GetRequestInfo()
                };

                // El servicio se encarga de buscar el actual o crear el "BC0000000X"
                var _batchActual = await finanzasService.ObtenerBatch(request);

                // Guardamos el BatchId para usarlo en la facturación y el Diario
                BatchActual = _batchActual;
            }
        }
    }
}
