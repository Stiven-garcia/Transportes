using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transportes.Models;

public partial class Cliente
{
    [Required]
    public long Cedula { get; set; }
    [Required]
    public string? Nombres { get; set; }
    [Required]
    public string? Apellidos { get; set; }
    public long? Telefono { get; set; }

    public virtual ICollection<Envio> Envios { get; set; } = new List<Envio>();
}
