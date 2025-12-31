using Aplicacion.DTOs;
using Aplicacion.DTOs.Seguridad;
using Notifications.Wpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.Comunes.CommonServices
{
    public class CommonService : ICommonService
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();
        public RequestUserInfo RequestUserInfo { get; set; }
        public List<PermisosDTO> Permisos { get; set; }

        public RequestUserInfo GetRequestInfo()
        {
            return RequestUserInfo;
        }

        public List<PermisosDTO> ObtenerPermisos()
        {
            throw new NotImplementedException();
        }

        public void SetearRequestInfo(UsuarioDTO usuarioDTO, int cajaId)
        {
            RequestUserInfo = new RequestUserInfo
            {
                UsuarioId = usuarioDTO.UsuarioId,
            };
        }

        public void ShowSuccess(string title, string message)
        {
            _notificationManager.ShowAsync(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = NotificationType.Success
            }, expirationTime: TimeSpan.FromSeconds(3));
        }

        public void ShowWarning(string title, string message)
        {
            _notificationManager.ShowAsync(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = NotificationType.Warning
            }, expirationTime: TimeSpan.FromSeconds(5));
        }

        public void ShowError(string title, string message)
        {
            _notificationManager.ShowAsync(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = NotificationType.Error
            }, expirationTime: TimeSpan.FromSeconds(5));
        }
    }
}
