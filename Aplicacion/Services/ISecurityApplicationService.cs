using Aplicacion.DTOs;
using Aplicacion.DTOs.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Services
{
    public interface ISecurityApplicationService
    {
        UsuarioDTO EditarUsuario(EdicionUsuarioRequest request);

        List<PantallaDTO> ObtenerPantallas();

        RolDTO EdicionPermisos(EdicionPermisosRequest request);

        UsuarioDTO CrearUsuario(EdicionUsuarioRequest request);

        UsuarioDTO IniciarSesion(UserRequest request);

        SearchResult<UsuarioDTO> ObtenerUsuario(GetUserRequest request);

        RolDTO CrearRol(EdicionRolRequest request);

        RolDTO EditarRol(EdicionRolRequest request);

        Task<List<RolDTO>> ObtenerRoles();
    }
}
