using MandalaStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Domain.Dtos
{
    public class CategoriaListDto
    {
        public string Id { get; set; }
        public string Categoria_Nome { get; set; } = string.Empty;
        public int QuantidadeProdutos { get; set; } 
    }
    public class CategoriaComProdutos : Categoria
    {
        public List<Produto> Produtos { get; set; } = [];
    }
}
