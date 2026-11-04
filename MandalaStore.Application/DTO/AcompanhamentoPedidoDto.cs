namespace MandalaStore.Application.DTOs;

public class AcompanhamentoPedidoDto
{
    public string NomeCliente { get; set; } = "";
    public DateTime DataPedido { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "";
    public string? FreteServico { get; set; }
}
