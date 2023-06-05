namespace Transportes.Models
{
    public class EnviosSearchRequest
    {
        public string? NumeroGuia { get; set; }

        public int? TipoEnvio { get; set; }

        public DateTime? FechaRegistroStart { get; set; }
        public DateTime?FechaRegistroEnd { get; set; }
        public DateTime? FechaEntregaStart { get; set; }
        public DateTime? FechaEntregaEnd { get; set; }

        public double? PrecioStart { get; set; }
        public double? PrecioEnd { get; set; }

        public long? Cedula { get; set; }
        public string? NombreApellido { get; set; }

        public string? TipoProducto { get; set; }

        public string? Placa { get; set; }
        public int? TipoVehiculo { get; set; }

        public string? Destino { get; set; }
        public int? TipoDestino{ get; set; }
    }
}
