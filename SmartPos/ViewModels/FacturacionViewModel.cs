using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.DTOs.Factura;
using Aplicacion.DTOs.Vendedores;
using Aplicacion.Services.ArticuloServices;
using Aplicacion.Services.Factura;
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
                var articuloAppService = scope.ServiceProvider.GetRequiredService<IFacturaApplicationService>();
                AgregarArticuloRequest request = new AgregarArticuloRequest
                {
                    ArticuloId = BusquedaArticulo.Trim(),
                    Vendedor = VendedorSeleccionado,
                    FacturasDetalle = FacturaDetalle.ToList()
                };
                var response = articuloAppService.AgregarArticuloAFactura(request);

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

            // 2. Recalcular totales (Importante para que la factura se actualice)
            ActualizarTotales();

            // 3. Devolver el foco al cuadro de búsqueda para seguir vendiendo
            // Esto lo manejamos usualmente con un mensaje o propiedad si es necesario,
            // pero al estar en el mismo VM, la UI se actualiza sola.
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

        private void ActualizarTotales()
        {
            Encabezado.Total = FacturaDetalle.Sum(x => x.Total);
            Encabezado.Impuesto = FacturaDetalle.Sum(x => x.Impuesto);
            
            OnPropertyChanged(nameof(Encabezado)); // Notifica a la UI
        }
    }
}
