using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MandalaStore.Domain.Entities
{
    public class Pedido
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Cliente
        public string NomeCliente { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty; // CPF ou CNPJ

        // Endereço de entrega
        public string Cep { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;

        // Produtos
        public List<ItemPedido> Itens { get; set; } = new();

        // Valores
        public decimal Subtotal { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal Total { get; set; }

        // Frete escolhido
        public int? FreteServicoId { get; set; }
        public string? FreteServico { get; set; }
        public string? FreteTransportadora { get; set; }
        public int? FretePrazo { get; set; }

        // Melhor Envio / Etiqueta
        public string? MelhorEnvioId { get; set; }
        public string? EtiquetaUrl { get; set; }
        public DateTime? EtiquetaGeradaEm { get; set; }

        // Pedido
        public DateTime? FreteCompradoEm { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pendente";

        // Mercado Pago
        public long? PaymentId { get; set; }
    }
}
