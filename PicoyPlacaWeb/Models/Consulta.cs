namespace PicoYPlacaApp.Models
{
    public class Consulta
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Resultado { get; set; }
    }
}
