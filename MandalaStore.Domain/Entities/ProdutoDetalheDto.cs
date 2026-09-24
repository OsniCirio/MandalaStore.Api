using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Domain.Entities
{
    public class ProdutoDetalheDto
    {
        public string Id { get; set; } = null!;

        public string Nome { get; set; } = null!;

        public string Descricao { get; set; } = null!;

        public decimal Preco { get; set; }

        public string CategoriaId { get; set; } = null!;

        public string? Categoria { get; set; }

        public string TemaId { get; set; } = null!;

        public string? Tema { get; set; }
        public decimal Altura { get; set; }
        public decimal Largura { get; set; }

        public string ImagemUrl { get; set; } = string.Empty;
    }
}
