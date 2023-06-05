using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transportes.Models;

public partial class Destino
{
    public int? IdDestino { get; set; }
    [Required]
    public string? Nombre { get; set; }
    [Required]
    public string? Ubicacion { get; set; }
    [Required]
    [Display(Name = "Tipo de Destino")]
    public int TipoDestino { get; set; }

    public virtual ICollection<Envio> Envios { get; set; } = new List<Envio>();

    public virtual TipoDestino? TipoDestinoNavigation { get; set; }
}
