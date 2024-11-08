namespace LojaAPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.Now;
        public string StatusPedido { get; set; } = "Em Andamento";
        public decimal ValorTotal { get; set; } = 0;
    }
}
