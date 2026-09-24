using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Application.DTO
{
    public class ProdutoCriacaoDto
    {
        public string Nome { get; set; } = null!;

        public string Descricao { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public string CategoriaId { get; set; }

        public string TemaId { get; set; }

        public string Preco { get; set; }

        public int Estoque { get; set; }

        public string Peso { get; set; }
        public string Largura { get; set; }
        public string Altura { get; set; }

        public bool Ativo { get; set; }

        public IFormFile? Imagem { get; set; }
    }
}
