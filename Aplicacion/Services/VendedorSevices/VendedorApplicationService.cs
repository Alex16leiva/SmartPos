using Aplicacion.DTOs;
using Aplicacion.DTOs.Vendedores;
using Aplicacion.Helpers;
using Dominio.Context.Entidades;
using Dominio.Core;
using Infraestructura.Context;

namespace Aplicacion.Services.VendedorSevices
{
    public class VendedorApplicationService : IVendedorApplicationService
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;
        public VendedorApplicationService(IGenericRepository<IDataContext> genericRepository) 
        {
            _genericRepository = genericRepository;
        }

        public SearchResult<VendedorDTO> ObtenerVendedoresPaginado(ObtenerVendedor request)
        {
            DynamicFilter dynamicFilter = DynamicFilterFactory.CreateDynamicFilter(request.QueryInfo);
            var vendedores = _genericRepository.GetPagedAndFiltered<Vendedor>(dynamicFilter);

            return new SearchResult<VendedorDTO>
            {
                ItemCount = vendedores.ItemCount,
                PageCount = vendedores.PageCount,
                PageIndex = vendedores.PageIndex,
                TotalItems = vendedores.TotalItems,
                Items = (from qry in vendedores.Items as IEnumerable<Vendedor>
                         select new VendedorDTO
                         {
                             VendedorId = qry.VendedorId,
                             Codigo = qry.Codigo,
                             Apellido = qry.Apellido,
                             Nombre = qry.Nombre,
                             Telefono = qry.Telefono
                         }).ToList()
            };
        }

        public List<VendedorDTO> ObtenerVendedores(ObtenerVendedor request)
        {
            IEnumerable<Vendedor> vendedores = _genericRepository.GetAll<Vendedor>();

            return vendedores.Select(r =>
                new VendedorDTO
                {
                    VendedorId = r.VendedorId,
                    Codigo = r.Codigo,
                    Apellido = r.Apellido,
                    Nombre = r.Nombre,
                    Telefono = r.Telefono
                }).ToList();
        }

        public VendedorDTO CrearVendedor(CrearVenderor request)
        {
            Vendedor vendedor = _genericRepository.GetSingle<Vendedor>(r => r.Codigo == request.Vendedor.Codigo);

            if (vendedor != null)
            {
                return new VendedorDTO
                {
                    Message = "El código de vendedor ya existe."
                };
            }

            Vendedor nuevoVendedor = new Vendedor
            {
                VendedorId = request.Vendedor.VendedorId,
                Codigo = request.Vendedor.Codigo,
                Nombre = request.Vendedor.Nombre,
                Apellido = request.Vendedor.Apellido,
                Telefono = request.Vendedor.Telefono
            };

            _genericRepository.Add(nuevoVendedor);
            Commit(request.RequestUserInfo, "CrearVendedor");

            return request.Vendedor;
        }

        public VendedorDTO ActualizarVendedor(ActualizarVenderor request)
        {
            Vendedor existeCodigoVendedor = _genericRepository.GetSingle<Vendedor>(r => r.VendedorId != request.Vendedor.VendedorId &&
                                                                    r.Codigo == request.Vendedor.Codigo);
            if (existeCodigoVendedor != null)
            {
                return new VendedorDTO
                {
                    Message = $"El código para vendedor {existeCodigoVendedor.Codigo} ya existe."
                };
            }

            Vendedor existeVendedor = _genericRepository.GetSingle<Vendedor>(r => r.VendedorId == request.Vendedor.VendedorId);

            existeVendedor.Codigo = request.Vendedor.Codigo;
            existeVendedor.Nombre = request.Vendedor.Nombre;
            existeVendedor.Apellido = request.Vendedor.Apellido;
            existeVendedor.Telefono = request.Vendedor.Telefono;

            Commit(request.RequestUserInfo, "ActualizarVendedor");

            return request.Vendedor;
        }

        public VendedorDTO EliminarVendedor(ObtenerVendedor request)
        {
            Vendedor existeMensajeArticulo = _genericRepository.GetSingle<Vendedor>(r => r.VendedorId == request.Vendedor.VendedorId);

            if (existeMensajeArticulo == null)
            {
                return new VendedorDTO
                {
                    Message = $"El mensaje {request.Vendedor.Nombre} no existe."
                };
            }

            _genericRepository.Remove(existeMensajeArticulo);
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("EliminarVendedor");
            _genericRepository.UnitOfWork.Commit(transactionInfo);

            return request.Vendedor;
        }


        private void Commit(RequestUserInfo requestUserInfo, string tipoTransaccion)
        {
            TransactionInfo transactionInfo = requestUserInfo.CrearTransactionInfo(tipoTransaccion);
            _genericRepository.UnitOfWork.Commit(transactionInfo);
        }
    }
}
