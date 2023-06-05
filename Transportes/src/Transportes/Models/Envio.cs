using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transportes.Models;

public partial class Envio
{
    [Required]
    [Display(Name ="Numero de Guia")]
    public string? NumeroGuia { get; set; }

    [Required]
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; }

    [Required]
    [Display(Name = "Fecha de Entrega")]
    public DateTime FechaEntrega { get; set; }

    [Required]
    public double Precio { get; set; }


    public int GetDescuento()
    {
        if (this.CantidadProducto > 10)
        {
            if (this.VehiculoNavigation.TipoVehiculo == 1)
            {
                return 3;
            }
            else
            {
                return 5;
            }
        }
        return 0;
    }

    public double GetPrecioDescuento()
    {
        double descuentoPor = Double.Parse(this.GetDescuento().ToString())/ 100;
        var descuento = this.Precio * descuentoPor; // Calcular el descuento
        return this.Precio - descuento; // Calcular el precio con descuento
    }

    [Required]
    [Display(Name = "Cantidad del Producto")]
    public int CantidadProducto { get; set; }

    [Required]
    [Display(Name = "Cedula del Cliente")]
    public long CedulaCliente { get; set; }

    [Required]
    [Display(Name = "Tipo de Producto")]
    public int TipoProducto { get; set; }

    [Required]
    public string? Vehiculo { get; set; }

    [Required]
    public int Destino { get; set; }

    public virtual Cliente? CedulaClienteNavigation { get; set; }

    public virtual Destino? DestinoNavigation { get; set; }

    public virtual TipoProducto? TipoProductoNavigation { get; set; }

    public virtual Vehiculo? VehiculoNavigation { get; set; }
}
