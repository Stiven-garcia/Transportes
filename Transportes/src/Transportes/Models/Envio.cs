using System;
using System.Collections.Generic;

namespace Transportes.Models;

public partial class Envio
{
    public string NumeroGuia { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public DateTime FechaEntrega { get; set; }

    public double Precio { get; set; }

    public int CantidadProducto { get; set; }

    public long CedulaCliente { get; set; }

    public int TipoProducto { get; set; }

    public string Vehiculo { get; set; } = null!;

    public int Destino { get; set; }

    public virtual Cliente CedulaClienteNavigation { get; set; } = null!;

    public virtual Destino DestinoNavigation { get; set; } = null!;

    public virtual TipoProducto TipoProductoNavigation { get; set; } = null!;

    public virtual Vehiculo VehiculoNavigation { get; set; } = null!;
}
