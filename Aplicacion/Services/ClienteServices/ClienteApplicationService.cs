using Aplicacion.DTOs;
using Aplicacion.DTOs.Clientes;
using Aplicacion.Helpers;
using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Finanzas;
using Dominio.Core;
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
            dynamicFilter.Includes = new List<string>();
            dynamicFilter.Includes.Add("TipoCuenta");
            try
            {
                PagedCollection cliente = _genericRepository.GetPagedAndFiltered<Cliente>(dynamicFilter);

                return new SearchResult<ClienteDTO>
                {
                    PageCount = cliente.PageCount,
                    ItemCount = cliente.ItemCount,
                    PageIndex = cliente.PageIndex,
                    TotalItems = cliente.TotalItems,
                    Items = (from qry in cliente.Items as IEnumerable<Cliente>
                             select new ClienteDTO
                             {
                                 AhorrosTotales = qry.AhorrosTotales,
                                 Apellido = qry.Apellido,
                                 AperturaCuenta = qry.AperturaCuenta,
                                 Ciudad = qry.Ciudad,
                                 Compania = qry.Compania,
                                 CorreoElectronico = qry.CorreoElectronico,
                                 Departamento = qry.Departamento,
                                 DescuentoActual = qry.DescuentoActual,
                                 Direccion = qry.Direccion,
                                 Direccion2 = qry.Direccion2,
                                 Empleado = qry.Empleado,
                                 FotoRuta = qry.FotoRuta,
                                 Id = qry.Id,
                                 LimiteCredito = qry.LimiteCredito,
                                 NivelPrecio = qry.NivelPrecio,
                                 Nombre = qry.Nombre,
                                 Notas = qry.Notas,
                                 NumeroCuenta = qry.NumeroCuenta,
                                 NumeroFax = qry.NumeroFax,
                                 NumeroPersonalizado1 = qry.NumeroPersonalizado1,
                                 NumeroPersonalizado2 = qry.NumeroPersonalizado2,
                                 NumeroPersonalizado3 = qry.NumeroPersonalizado3,
                                 NumeroPersonalizado4 = qry.NumeroPersonalizado4,
                                 NumeroPersonalizado5 = qry.NumeroPersonalizado5,
                                 NumeroTelefono1 = qry.NumeroTelefono1,
                                 NumeroTelefono2 = qry.NumeroTelefono2,
                                 Pais = qry.Pais,
                                 SaldoCuenta = qry.SaldoCuenta,
                                 TextoPersonalizado1 = qry.TextoPersonalizado1,
                                 TextoPersonalizado2 = qry.TextoPersonalizado2,
                                 TextoPersonalizado3 = qry.TextoPersonalizado3,
                                 TextoPersonalizado4 = qry.TextoPersonalizado4,
                                 TextoPersonalizado5 = qry.TextoPersonalizado5,
                                 TipoCuentaID = qry.TipoCuentaID,
                                 TotalVisitas = qry.TotalVisitas,
                                 UltimaVisita = qry.UltimaVisita,
                                 VentasTotales = qry.VentasTotales,
                                 TieneCredito = qry.TieneCredito()
                             }).ToList()
                };
            }
            catch (Exception e)
            {

                throw;
            }
            
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
                NumeroTelefono1 = request.Cliente.NumeroTelefono1,
                NumeroTelefono2 = string.IsNullOrWhiteSpace(request.Cliente.NumeroTelefono2) ? string.Empty : request.Cliente.NumeroTelefono2,
                NumeroFax = string.IsNullOrWhiteSpace(request.Cliente.NumeroFax) ? string.Empty : request.Cliente.NumeroFax,
                CorreoElectronico = string.IsNullOrWhiteSpace(request.Cliente.CorreoElectronico) ? string.Empty : request.Cliente.CorreoElectronico,
                Compania = string.IsNullOrWhiteSpace(request.Cliente.Compania) ? string.Empty : request.Cliente.Compania,
                Empleado = request.Cliente.Empleado,
                DescuentoActual = request.Cliente.DescuentoActual,
                NivelPrecio = request.Cliente.NivelPrecio,
                TipoCuentaID = request.Cliente.TipoCuentaID,
                LimiteCredito = request.Cliente.LimiteCredito,
                Pais = request.Cliente.Pais,
                Departamento = request.Cliente.Departamento,
                Ciudad = request.Cliente.Ciudad,
                Direccion = request.Cliente.Direccion,
                Direccion2 = string.IsNullOrWhiteSpace(request.Cliente.Direccion2) ? string.Empty : request.Cliente.Direccion2,
                Notas = string.IsNullOrWhiteSpace(request.Cliente.Notas) ? string.Empty : request.Cliente.Notas,
                AperturaCuenta = fecha,
                NumeroPersonalizado1 = request.Cliente.NumeroPersonalizado1,
                NumeroPersonalizado2 = request.Cliente.NumeroPersonalizado2,
                NumeroPersonalizado3 = request.Cliente.NumeroPersonalizado3,
                NumeroPersonalizado4 = request.Cliente.NumeroPersonalizado4,
                NumeroPersonalizado5 = request.Cliente.NumeroPersonalizado5,
                TextoPersonalizado1 = string.IsNullOrWhiteSpace(request.Cliente.TextoPersonalizado1) ? string.Empty : request.Cliente.TextoPersonalizado1,
                TextoPersonalizado2 = string.IsNullOrWhiteSpace(request.Cliente.TextoPersonalizado2) ? string.Empty : request.Cliente.TextoPersonalizado2,
                TextoPersonalizado3 = string.IsNullOrWhiteSpace(request.Cliente.TextoPersonalizado3) ? string.Empty : request.Cliente.TextoPersonalizado3,
                TextoPersonalizado4 = string.IsNullOrWhiteSpace(request.Cliente.TextoPersonalizado4) ? string.Empty : request.Cliente.TextoPersonalizado4,
                TextoPersonalizado5 = string.IsNullOrWhiteSpace(request.Cliente.TextoPersonalizado5) ? string.Empty : request.Cliente.TextoPersonalizado5,
                UltimaVisita = fecha,
                FotoRuta = string.Empty
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
    }
}
