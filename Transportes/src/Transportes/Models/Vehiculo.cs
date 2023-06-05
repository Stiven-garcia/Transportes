using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Transportes.Models;

public partial class Vehiculo
{
    [Required]
    public string? Placa { get; set; }
    [Required]
    [Display(Name = "Tipo de Vehiculo")]
    public int? TipoVehiculo { get; set; }

    public virtual ICollection<Envio> Envios { get; set; } = new List<Envio>();

    public virtual TipoVehiculo TipoVehiculoNavigation { get; set; } = null!;
}
