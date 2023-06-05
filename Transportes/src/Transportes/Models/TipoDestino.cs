using System;
using System.Collections.Generic;

namespace Transportes.Models;

public partial class TipoDestino
{
    public int IdTipoDestino { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Destino> Destinos { get; set; } = new List<Destino>();
}
