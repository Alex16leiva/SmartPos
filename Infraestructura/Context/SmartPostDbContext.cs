using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Context.Entidades.Seguridad;
using Dominio.Core;
using Infraestructura.Context.Mapping.Articulos;
using Infraestructura.Context.Mapping.Factura;
using Infraestructura.Context.Mapping.Seguridad;
using Infraestructura.Core;
using Microsoft.EntityFrameworkCore;
using SmartPos.Data.Mapping;

namespace Infraestructura.Context
{
    public class SmartPostDbContext : BCUnitOfWork, IDataContext
    {
        public SmartPostDbContext(DbContextOptions<SmartPostDbContext> context)
            : base(context)
        {
            Database.SetCommandTimeout((int)TimeSpan.FromSeconds(1).TotalSeconds);
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Rol> Rol {  get; set; }
        public virtual DbSet<Pantalla> Pantalla { get; set; }
        public virtual DbSet<Permisos> Permisos { get; set; }
        public virtual DbSet<Articulo> Artuculo { get; set; }
        public virtual DbSet<InventarioMovimiento> InventarioMovimiento { get; set; }
        public virtual DbSet<FacturaEncabezado> FacturaEncabezado { get; set; }
        public virtual DbSet<FacturaDetalle> FacturaDetalle { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new RolMap());
            modelBuilder.ApplyConfiguration(new PantallaMap());
            modelBuilder.ApplyConfiguration(new PermisosMap());
            modelBuilder.ApplyConfiguration(new ArticuloMap());
            modelBuilder.ApplyConfiguration(new InventarioMovimientoMap());
            modelBuilder.ApplyConfiguration(new FacturaEncabezadoMap());
            modelBuilder.ApplyConfiguration(new FacturaDetalleMap());
            base.OnModelCreating(modelBuilder);
        }


        public override void Commit(TransactionInfo transactionInfo)
        {
            base.Commit(transactionInfo);
        }
    }
}
