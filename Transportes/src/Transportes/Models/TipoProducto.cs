using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transportes.Models;

public partial class TipoProducto
{
    public int? IdTipoProducto { get; set; }

    [Required]
    [Display(Name = "Tipo De Producto")]
    public string? Descripcion { get; set; }

    public virtual ICollection<Envio> Envios { get; set; } = new List<Envio>();
}
