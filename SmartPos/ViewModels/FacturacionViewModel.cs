using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using Aplicacion.DTOs.Clientes;
using Aplicacion.DTOs.ConfiTienda;
using Aplicacion.DTOs.Factura;
using Aplicacion.DTOs.Finanzas;
using Aplicacion.DTOs.FormasPagos;
using Aplicacion.DTOs.Vendedores;
using Aplicacion.Services.ArticuloServices;
using Aplicacion.Services.ClienteServices;
using Aplicacion.Services.ConfiTienda;
using Aplicacion.Services.Factura;
using Aplicacion.Services.Finanzas;
using Aplicacion.Services.FPagos;
using Aplicacion.Services.VendedorSevices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dominio.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SmartPos.Comunes;
using SmartPos.Comunes.CommonServices;
using SmartPos.DTOs.Articulos;
using SmartPos.Views;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmartPos.ViewModels
{
    public partial class FacturacionViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<FacturaDetalleDTO> _facturaDetalle = new();
        [ObservableProperty] private FacturaEncabezadoDTO _encabezado = new();

        [ObservableProperty] private BatchDTO _batchActual;
        [ObservableProperty] private bool _isBusy;

        [ObservableProperty] private ObservableCollection<VendedorDTO> _vendedores = new();
        [ObservableProperty] private VendedorDTO _vendedorSeleccionado = new();

        [ObservableProperty] private ConfiguracionTiendaDTO _configuracionTiendaSeleccionada = new();

        [ObservableProperty] private ObservableCollection<FormasPagoDTO> _formasPago = new();
        [ObservableProperty]private ObservableCollection<FormasPagoDTO> _formasDePagoOriginal = new();
        [ObservableProperty]private decimal _cambio;
        [ObservableProperty]private decimal _totalCobro;
        [ObservableProperty]public decimal _totalMostrar;

        // Propiedades de Paginación de Articulos
        [ObservableProperty] private ObservableCollection<ArticulosDTO> _articulosBusqueda = new();
        [ObservableProperty] private ArticulosDTO? _articuloBusquedaSeleccionado = new();
        [ObservableProperty] private string _busquedaArticulo = string.Empty;
        [ObservableProperty] private string _textoBusquedaModal = string.Empty;
        [ObservableProperty] private int _paginaActual = 1;
        [ObservableProperty] private int _totalPaginas = 0;
        [ObservableProperty] private int _registrosPorPagina = 10;
        private CancellationTokenSource? _searchCts;

        // Propiedades relacionadas al Cliente
        [ObservableProperty] private ObservableCollection<ClienteDTO> _clientes = new();
        [ObservableProperty] private ClienteDTO _clienteSeleccionado = new();
        [ObservableProperty] private string _nombreClienteActual = string.Empty;
        [ObservableProperty] private string _clienteBusqueda = string.Empty;
        [ObservableProperty] private int _paginaActualClientes = 1;
        [ObservableProperty] private int _totalPaginasClientes = 0;
        [ObservableProperty] private string _clientesBusqueda = string.Empty;
        [ObservableProperty] private bool _clienteTieneCredito = false;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICommonService _commonService;
        public FacturacionViewModel(IServiceScopeFactory scopeFactory, ICommonService commonService)
        {
            _commonService = commonService;
            _scopeFactory = scopeFactory;

            CargarVendedoresAsync();
            ObtenerBatch();
            CargarFormasPago();
            ObtenerConfiguracionTiendas();
            CargarClienteGenerico();
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
                    FacturaDetalle = FacturaDetalle.ToList()
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
                    FacturaDetalle = FacturaDetalle.ToList()
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
        public void SeleccionarCliente()
        {
            // 1. Verificación de seguridad
            if (ClienteSeleccionado == null) return;

            // 2. Asignación al encabezado de la factura
            // Asegúrate de que Encabezado no sea nulo
            if (Encabezado != null)
            {
                Encabezado.ClienteId = ClienteSeleccionado.NumeroCuenta;
                // Si tienes una propiedad para mostrar el nombre en la UI:
                NombreClienteActual = $"{ClienteSeleccionado.Nombre} {ClienteSeleccionado.Apellido}";
            }
        }

        public void CargarClienteGenerico()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var clienteService = scope.ServiceProvider.GetRequiredService<IClienteApplicationService>();
                var clienteGenerico = clienteService.ObtenerClienteGenerico();
                if (clienteGenerico != null)
                {
                    ClienteSeleccionado = clienteGenerico;
                    SeleccionarCliente();
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

        public async Task CargarFormasPago()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var maestroService = scope.ServiceProvider.GetRequiredService<IFormasPagoApplicationService>();
                var result = maestroService.ObtenerFormasDePago(new FormaPagoRequest { });
                FormasDePagoOriginal = new ObservableCollection<FormasPagoDTO>(result);
            }
        }

        private void ObtenerConfiguracionTiendas()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var configuracionTiendaApplication = scope.ServiceProvider.GetRequiredService<IConfiguracionTiendaApplicationService>();
                ConfiguracionTiendaSeleccionada = configuracionTiendaApplication.ObtenerConfiguracionTienda(new ConfiguracionTiendaRequest { EsAdminPos = true  });

                if (ConfiguracionTiendaSeleccionada.Message.HasValue())
                {
                    _commonService.ShowWarning(ConfiguracionTiendaSeleccionada.Message);
                }
            }
        }

        [RelayCommand]
        public async Task BuscarClientes()
        {
            PaginaActualClientes = 1; // Resetear al buscar
            await LoadDataClientesAsync();
        }

        [RelayCommand]
        public async Task AbrirBusquedaClientes()
        {
            // 1. Cargamos la primera página de clientes antes de mostrar la ventana
            PaginaActualClientes = 1;
            await LoadDataClientesAsync();

            // 2. Instanciamos la vista
            var ventanaClientes = new ClientesBusquedaView
            {
                // Importante: Compartimos el DataContext para que la ventana 
                // use las propiedades y comandos que ya definimos en este VM
                DataContext = this,

            };

            // 3. La mostramos como diálogo (bloquea la factura hasta elegir cliente)
            if (ventanaClientes.ShowDialog() == true)
            {
                if (ClienteSeleccionado != null)
                {
                    Encabezado.ClienteId = ClienteSeleccionado.NumeroCuenta;
                    
                }
            }
        }

        public async Task LoadDataClientesAsync()
        {
            IsBusy = true;

            var request = new ClienteRequest 
            {
                QueryInfo = new QueryInfo
                {
                    PageIndex = PaginaActualClientes - 1,
                    PageSize = RegistrosPorPagina,
                    SortFields = ["Nombre"],
                    Ascending = true,
                    Predicate = !string.IsNullOrWhiteSpace(TextoBusquedaModal)
                                ? "ClienteId.ToString().Contains(@0) OR Nombre.Contains(@0) OR Identificacion.Contains(@0)"
                                : string.Empty,
                    ParamValues = !string.IsNullOrWhiteSpace(TextoBusquedaModal)
                                  ? new object[] { TextoBusquedaModal }
                                  : Array.Empty<object>()
                }
            };

            using (var scope = _scopeFactory.CreateScope())
            {
                var clienteService = scope.ServiceProvider.GetRequiredService<IClienteApplicationService>();
                var result = clienteService.ObtenerCliente(request);

                Clientes = new ObservableCollection<ClienteDTO>(result.Items);
                TotalPaginasClientes = result.PageCount;
            }
            IsBusy = false;
        }

        [RelayCommand] 
        public void MostrarVentanaDePago()
        {
            if (EsUnCobroValido())
            {
                // 1. Cálculos finales antes de cobrar
                TotalMostrar = Math.Round(FacturaDetalle.Sum(r => r.Total), 2);

                ClienteTieneCredito = ClienteSeleccionado.IsNotNull() ? ClienteSeleccionado.TieneCredito : false;

                // 2. Filtrar formas de pago según el cliente
                ObtenerFormaPago(ClienteTieneCredito);

                // 3. Instanciar la vista de cobros
                var vistaCobro = new CobrosView();

                // 4. Compartir el DataContext (esto es lo que permite que funcione todo)
                vistaCobro.DataContext = this;

                // 5. Mostrar como diálogo modal
                var resultado = vistaCobro.ShowDialog();
            }
        }

        // Lógica para calcular el cambio cada vez que se escribe en el DataGrid
        partial void OnTotalCobroChanged(decimal value)
        {
            Cambio = TotalCobro - TotalMostrar;
        }

        // Este método se debe llamar cuando el cajero termina de escribir en una celda
        public void CalcularTotalRecibido()
        {
            TotalCobro = FormasPago.Sum(x => x.MostrarCobro);
        }

        [RelayCommand]
        public async Task GuardarPago()
        {
            // 1. Validación: ¿El monto recibido es suficiente?
            if (TotalCobro < TotalMostrar)
            {
                _commonService.ShowWarning("El monto recibido es menor al total de la venta.");
                return;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                var facturaService = scope.ServiceProvider.GetRequiredService<IFacturaApplicationService>();

                // Preparamos el objeto para enviar al backend
                var nuevaFactura = new FacturaRequest
                {
                    FacturaEncabezado = new FacturaEncabezadoDTO
                    {
                        BatchId = BatchActual.BatchId,
                        TipoFactura = ClienteSeleccionado.TieneCredito ? "CREDITO" : "CONTADO",
                        ClienteId = Encabezado.ClienteId,
                        SubTotal = Encabezado.SubTotal,
                        Impuesto = Encabezado.Impuesto,
                        Descuento = Encabezado.Descuento,
                        CampoPersonalizado1 = string.Empty, //CampoPersonalizado1,
                        CampoPersonalizado2 = string.Empty, //CampoPersonalizado2,
                        Comentario = string.Empty,
                        LlamadaTipo = string.Empty,
                        LLamadaId = string.Empty,
                        Cambio = Cambio,
                        Total = Encabezado.Total,
                        CajaId = _commonService.GetRequestInfo().Caja,
                    },
                    FacturaDetalle = FacturaDetalle.ToList(),
                    FormasPagos = FormasPago.ToList(),
                    RequestUserInfo = _commonService.GetRequestInfo()
                };

                // Llamada al servicio
                var result = await Task.Run(() => facturaService.CrearFactura(nuevaFactura));

                if (result.HasAnyMessage())
                {
                    _commonService.ShowWarning(result.Getmessage());
                    return;
                }

                
                _commonService.ShowSuccess("Venta realizada con éxito");

                Impresiones.ConfiguracionTienda = ConfiguracionTiendaSeleccionada;
                Impresiones.FacturaEncabezado = result.FacturaEncabezado;
                Impresiones.FacturaDetalle = result.FacturaDetalle;
                Impresiones.ClienteSeleccionado = ClienteSeleccionado;
                string errorImpresion = string.Empty;
                bool esVisor = false;
                Impresiones.ImprimirFacturaContado("", esVisor, out errorImpresion);

                LimpiarPantalla();
            }
        }

        private bool EsUnCobroValido()
        {
            if (FacturaDetalle.IsEmpty())
            {
                _commonService.ShowWarning("No hay articulos a facturar");
                return false;
            }
            var tieneCantidadConCero = FacturaDetalle.Where(r => r.Cantidad == 0);
            if (tieneCantidadConCero.HasItems())
            {
                _commonService.ShowWarning("Tiene articulos con cantidad cero");
                return false;
            }
            return true;
        }

        private void ObtenerFormaPago(bool esCobroCredito)
        {
            // Limpiamos y recargamos desde la lista original
            FormasPago.Clear();

            foreach (var fp in FormasDePagoOriginal)
            {
                // Si no tiene crédito, saltamos las formas de pago tipo 2 (Crédito)
                if (!esCobroCredito && fp.TipoPago == 2) continue;

                // Resetear valores de cobro para la nueva transacción
                fp.Cobro = 0;
                fp.MostrarCobro = 0;
                FormasPago.Add(fp);
            }
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

        [RelayCommand]
        private async Task GenerarReporteZ()
        {
            var request = new BatchRequest
            {
                Batch = BatchActual,
                RequestUserInfo = _commonService.GetRequestInfo()
            };

            using (var scope = _scopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IFinanzasApplicationService>();
                var result = await service.RealizarCierreZ(request);

                if (result.HasAnyMessage())
                {
                    _commonService.ShowWarning(result.Getmessage());
                    return;
                }

                BatchActual = new BatchDTO();

                var esVisor = false;
                string errorImpresion = string.Empty;
                Impresiones.Batch = result.BatchImpresion;
                Impresiones.ImprimirReporteZ(string.Empty, esVisor, out errorImpresion);
                _commonService.ShowSuccess("Cierre realizado.");
                
            }
        }
    }
}
