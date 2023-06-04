using System;
using System.Collections.Generic;

namespace Transportes.Models;

public partial class TipoVehiculo
{
    public int IdTipoVehiculo { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
