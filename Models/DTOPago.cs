namespace ClienteHTTPObligatorio.Models
{
    public class DTOPago
    {
        public int Id { get; set; }

        public string Descripcion { get; set; }

        public string MetodoDePago { get; set; }

        public string TipoGasto { get; set; }

        public string TipoPago { get; set; }

        public double MontoTotal { get; set; }

        public string NombreUsuario { get; set; }

        public DateTime FechaPago { get; set; }
        public double SaldoPendiente { get; set; }

        public string UsuarioEmail { get; set; }
    }
}
