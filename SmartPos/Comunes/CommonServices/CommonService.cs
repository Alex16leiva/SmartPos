using Aplicacion.DTOs;
using Aplicacion.DTOs.Seguridad;
using Notifications.Wpf.Core;
using SmartPos.Comunes.Notificaciones;
using System.Windows.Controls.Primitives;

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

        public void ShowWarning(string message)
        {
            ShowCustomNotification("Warning Message", message, "Warning");
        }

        public void ShowError(string message)
        {
            ShowCustomNotification("Error Message", message, "Error");
        }

        public void ShowSuccess(string message)
        {
            ShowCustomNotification("Success Message", message, "Success");
        }

        private void ShowCustomNotification(string title, string message, string type = "Success")
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                // 1. Creamos el contenido
                var toast = new ToastNotification();
                // Accedemos a los elementos del XAML (puedes usar Bindings si prefieres)
                toast.TitleText.Text = title; 
                toast.MessageText.Text = message;
                toast.IconText.Text = type switch
                {
                    "Success" => "✔", // Icono de éxito
                    "Error" => "✖",   // Icono de error
                    "Warning" => "⚠", // Icono de advertencia
                    _ => "ℹ"         // Icono de información
                };

                // 2. Creamos el contenedor Popup
                Popup popup = new Popup
                {
                    Child = toast,
                    AllowsTransparency = true,
                    Placement = PlacementMode.Right, // O PlacementMode.CenterScreen
                    StaysOpen = false,
                    PopupAnimation = PopupAnimation.Fade
                };

                popup.IsOpen = true;

                // 3. Auto-cierre con un Timer de Backend
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(3)
                };
                timer.Tick += (s, e) =>
                {
                    popup.IsOpen = false;
                    timer.Stop();
                };
                timer.Start();
            });
        }
    }
}
