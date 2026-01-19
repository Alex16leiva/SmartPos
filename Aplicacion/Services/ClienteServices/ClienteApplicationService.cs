using Aplicacion.DTOs;
using Aplicacion.DTOs.Clientes;
using Aplicacion.Helpers;
using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Finanzas;
using Dominio.Core;
using Dominio.Core.Extensions;
using Infraestructura.Context;
using ServicioAplicacion.DTOs.TipoCuenta;

namespace Aplicacion.Services.ClienteServices
{
    public class ClienteApplicationService : IClienteApplicationService
    {
        IGenericRepository<IDataContext> _genericRepository;
        public ClienteApplicationService(IGenericRepository<IDataContext> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public SearchResult<ClienteDTO> ObtenerCliente(ClienteRequest request)
        {
            DynamicFilter dynamicFilter = DynamicFilterFactory.CreateDynamicFilter(request.QueryInfo);
            dynamicFilter.Includes = ["TipoCuenta"];

            PagedCollection cliente = _genericRepository.GetPagedAndFiltered<Cliente>(dynamicFilter);

            return new SearchResult<ClienteDTO>
            {
                PageCount = cliente.PageCount,
                ItemCount = cliente.ItemCount,
                PageIndex = cliente.PageIndex,
                TotalItems = cliente.TotalItems,
                Items = (from qry in cliente.Items as IEnumerable<Cliente>
                         select MapEntidadToDTO(qry)
                             ).ToList()
            };
        }

        public ClienteDTO CrearCliente(ClienteRequest request)
        {
            Cliente cliente = _genericRepository.GetSingle<Cliente>(r => r.NumeroCuenta == request.Cliente.NumeroCuenta);

            if (cliente != null)
            {
                return new ClienteDTO
                {
                    Message = $"El Numero de cuenta {request.Cliente.NumeroCuenta} ya existe"
                };
            }

            DateTime fecha = DateTime.Now;
            Cliente nuevoCliente = new Cliente
            {
                NumeroCuenta = request.Cliente.NumeroCuenta,
                Nombre = request.Cliente.Nombre,
                Apellido = request.Cliente.Apellido,
                NumeroTelefono1 = request.Cliente.NumeroTelefono1.ValueOrEmpty(),
                NumeroTelefono2 = request.Cliente.NumeroTelefono2.ValueOrEmpty(),
                NumeroFax = request.Cliente.NumeroFax.ValueOrEmpty(),
                CorreoElectronico = request.Cliente.CorreoElectronico.ValueOrEmpty(),
                Compania = request.Cliente.Compania.ValueOrEmpty(),
                Empleado = request.Cliente.Empleado,
                DescuentoActual = request.Cliente.DescuentoActual,
                NivelPrecio = request.Cliente.NivelPrecio,
                TipoCuentaID = request.Cliente.TipoCuentaID,
                LimiteCredito = request.Cliente.LimiteCredito,
                Pais = request.Cliente.Pais,
                Departamento = request.Cliente.Departamento,
                Ciudad = request.Cliente.Ciudad,
                Direccion = request.Cliente.Direccion,
                Direccion2 = request.Cliente.Direccion2.ValueOrEmpty(),
                Notas = request.Cliente.Notas.ValueOrEmpty(),
                AperturaCuenta = fecha,
                NumeroPersonalizado1 = request.Cliente.NumeroPersonalizado1,
                NumeroPersonalizado2 = request.Cliente.NumeroPersonalizado2,
                NumeroPersonalizado3 = request.Cliente.NumeroPersonalizado3,
                NumeroPersonalizado4 = request.Cliente.NumeroPersonalizado4,
                NumeroPersonalizado5 = request.Cliente.NumeroPersonalizado5,
                TextoPersonalizado1 = request.Cliente.TextoPersonalizado1.ValueOrEmpty(),
                TextoPersonalizado2 = request.Cliente.TextoPersonalizado2.ValueOrEmpty(),
                TextoPersonalizado3 = request.Cliente.TextoPersonalizado3.ValueOrEmpty(),
                TextoPersonalizado4 = request.Cliente.TextoPersonalizado4.ValueOrEmpty(),
                TextoPersonalizado5 = request.Cliente.TextoPersonalizado5.ValueOrEmpty(),
                FechaPersonalizada1 = request.Cliente.FechaPersonalizada1.GetDateOrDefault(),
                FechaPersonalizada2 = request.Cliente.FechaPersonalizada2.GetDateOrDefault(),
                FechaPersonalizada3 = request.Cliente.FechaPersonalizada3.GetDateOrDefault(),
                FechaPersonalizada4 = request.Cliente.FechaPersonalizada4.GetDateOrDefault(),
                FechaPersonalizada5 = request.Cliente.FechaPersonalizada5.GetDateOrDefault(),
                UltimaVisita = fecha,
                FotoRuta = string.Empty,
                Zip = request.Cliente.Zip.ValueOrEmpty(),
            };

            _genericRepository.Add(nuevoCliente);
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("CrearCliente");
            _genericRepository.UnitOfWork.Commit(transactionInfo);
            
            return request.Cliente;
        }

        public ClienteDTO ActualizarCliente(ClienteRequest request)
        {
            Cliente cliente = _genericRepository.GetSingle<Cliente>(r => r.NumeroCuenta == request.Cliente.NumeroCuenta);

            if (cliente == null)
            {
                return new ClienteDTO
                {
                    Message = $"El Numero de cuenta {request.Cliente.NumeroCuenta} no existe"
                };
            }

            cliente.NumeroCuenta = request.Cliente.NumeroCuenta;
            cliente.Nombre = request.Cliente.Nombre;
            cliente.Apellido = request.Cliente.Apellido;
            cliente.Ciudad = request.Cliente.Ciudad;
            cliente.Compania = request.Cliente.Compania;
            cliente.CorreoElectronico = request.Cliente.CorreoElectronico;
            cliente.Departamento = request.Cliente.Departamento;
            cliente.DescuentoActual = request.Cliente.DescuentoActual;
            cliente.Direccion = request.Cliente.Direccion;
            cliente.Direccion2 = request.Cliente.Direccion2;
            cliente.Empleado = request.Cliente.Empleado;
            cliente.LimiteCredito = request.Cliente.LimiteCredito;
            cliente.NivelPrecio = request.Cliente.NivelPrecio;
            cliente.Notas = request.Cliente.Notas;
            cliente.NumeroFax = request.Cliente.NumeroFax;
            cliente.NumeroPersonalizado1 = request.Cliente.NumeroPersonalizado1;
            cliente.NumeroPersonalizado2 = request.Cliente.NumeroPersonalizado2;
            cliente.NumeroPersonalizado3 = request.Cliente.NumeroPersonalizado3;
            cliente.NumeroPersonalizado4 = request.Cliente.NumeroPersonalizado4;
            cliente.NumeroPersonalizado5 = request.Cliente.NumeroPersonalizado5;
            cliente.NumeroTelefono1 = request.Cliente.NumeroTelefono1;
            cliente.NumeroTelefono2 = request.Cliente.NumeroTelefono2;
            cliente.Pais = request.Cliente.Pais;
            cliente.TextoPersonalizado1 = request.Cliente.TextoPersonalizado1;
            cliente.TextoPersonalizado2 = request.Cliente.TextoPersonalizado2;
            cliente.TextoPersonalizado3 = request.Cliente.TextoPersonalizado3;
            cliente.TextoPersonalizado4 = request.Cliente.TextoPersonalizado4;
            cliente.TextoPersonalizado5 = request.Cliente.TextoPersonalizado5;
            cliente.TipoCuentaID = request.Cliente.TipoCuentaID;

            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("ActualizarCliente");
            _genericRepository.UnitOfWork.Commit(transactionInfo);

            return request.Cliente;
        }

        public List<TipoCuentaDTO> ObtenerTipoCuentas(TipoCuentaRequest request)
        {
            IEnumerable<TipoCuenta> tiposCuentas = _genericRepository.GetAll<TipoCuenta>();

            return tiposCuentas.Select(r =>
                new TipoCuentaDTO
                {
                    TipoCuentaId = r.TipoCuentaId,
                    EsCredito = r.EsCredito,
                    Descripcion = r.Descripcion
                }).ToList();
        }

        public ClienteDTO ObtenerClienteGenerico()
        {
            Cliente cliente = _genericRepository.GetSingle<Cliente>(r => r.NumeroCuenta == "000");

            if (cliente.IsNotNull())
            {
                return MapEntidadToDTO(cliente);
            }

            DateTime fechaHoy = DateTime.Now;

            var requestGenerico = new ClienteRequest
            {
                Cliente = new ClienteDTO
                {
                    NumeroCuenta = "000",
                    Nombre = "CONSUMIDOR",
                    Apellido = "FINAL",
                    Compania = "GENERAL",
                    Pais = "Honduras",
                    Ciudad = "SPS",
                    Direccion = "CIUDAD",
                    Empleado = false,
                    NivelPrecio = 1,
                    DescuentoActual = 0,
                    LimiteCredito = 0,
                    TipoCuentaID = 1,
                    FechaPersonalizada1 = fechaHoy,
                    FechaPersonalizada2 = fechaHoy,
                    FechaPersonalizada3 = fechaHoy,
                    FechaPersonalizada4 = fechaHoy,
                    FechaPersonalizada5 = fechaHoy,
                },
                RequestUserInfo = new RequestUserInfo
                {
                    // Datos mínimos para que CrearTransactionInfo no falle
                    UsuarioId = "SYSTEM",
                }
            };

            // 3. Reutilizamos tu método existente
            return CrearCliente(requestGenerico);
        }

        public ClienteDTO MapEntidadToDTO(Cliente entidad)
        {
            return new ClienteDTO
            {
                AhorrosTotales = entidad.AhorrosTotales,
                Apellido = entidad.Apellido,
                AperturaCuenta = entidad.AperturaCuenta,
                Ciudad = entidad.Ciudad,
                Compania = entidad.Compania,
                CorreoElectronico = entidad.CorreoElectronico,
                Departamento = entidad.Departamento,
                DescuentoActual = entidad.DescuentoActual,
                Direccion = entidad.Direccion,
                Direccion2 = entidad.Direccion2,
                Empleado = entidad.Empleado,
                FotoRuta = entidad.FotoRuta,
                Id = entidad.Id,
                LimiteCredito = entidad.LimiteCredito,
                NivelPrecio = entidad.NivelPrecio,
                Nombre = entidad.Nombre,
                Notas = entidad.Notas,
                NumeroCuenta = entidad.NumeroCuenta,
                NumeroFax = entidad.NumeroFax,
                NumeroPersonalizado1 = entidad.NumeroPersonalizado1,
                NumeroPersonalizado2 = entidad.NumeroPersonalizado2,
                NumeroPersonalizado3 = entidad.NumeroPersonalizado3,
                NumeroPersonalizado4 = entidad.NumeroPersonalizado4,
                NumeroPersonalizado5 = entidad.NumeroPersonalizado5,
                NumeroTelefono1 = entidad.NumeroTelefono1,
                NumeroTelefono2 = entidad.NumeroTelefono2,
                Pais = entidad.Pais,
                SaldoCuenta = entidad.SaldoCuenta,
                TextoPersonalizado1 = entidad.TextoPersonalizado1,
                TextoPersonalizado2 = entidad.TextoPersonalizado2,
                TextoPersonalizado3 = entidad.TextoPersonalizado3,
                TextoPersonalizado4 = entidad.TextoPersonalizado4,
                TextoPersonalizado5 = entidad.TextoPersonalizado5,
                TipoCuentaID = entidad.TipoCuentaID,
                TotalVisitas = entidad.TotalVisitas,
                UltimaVisita = entidad.UltimaVisita,
                VentasTotales = entidad.VentasTotales,
                TieneCredito = entidad.TieneCredito()
            };
        }
    }
}
