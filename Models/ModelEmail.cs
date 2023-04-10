namespace ms_notificaciones.Models
{
    public class ModelEmail
    {
        public string? correoDestino { get; set; }
        public string? nombreDestino { get; set; }
        
        public string?  contenidoCorreo { get; set; }
        public string? asuntoCorreo { get; set; }
        
    }
}