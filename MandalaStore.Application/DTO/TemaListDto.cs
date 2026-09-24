using MandalaStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Domain.Dtos
{
    public class TemaListDto
    {
        public string Id { get; set; }
        public string Tema_Nome { get; set; } = string.Empty;
        public int QuantidadeProdutos { get; set; } 
    }
    public class TemaComProdutos : Categoria
    {
        public List<Produto> Produtos { get; set; } = [];
    }
}
