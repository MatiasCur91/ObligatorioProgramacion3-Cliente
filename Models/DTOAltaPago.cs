namespace ClienteHTTPObligatorio.Models;

public class DTOAltaPago
{
    public double Monto { get; set; }

    public int IdTipoGasto { get; set; }
    public string MetodoDePago { get; set; }
    public string TipoSeleccionado { get; set; }
    public string Descripcion { get; set; }

    public DateTime? FechaPago { get; set; }
    public int? NumeroRecibo { get; set; }

    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    // public List<DTOTipoGasto> TiposGastos { get; set; } = new();
}