using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Domain.Entities
{
    public class ItemPedido
    {
        public string Id { get; set; } = string.Empty;

        public string ProdutoId { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }

        public decimal Preco { get; set; }
        public string ImagemUrl { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
}
