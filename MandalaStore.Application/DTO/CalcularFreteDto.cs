using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace MandalaStore.Application.DTO
{
    public class CalcularFreteDto
    {
        public string CepDestino { get; set; } = string.Empty;

        public List<ProdutoFreteDto> Produtos { get; set; } = new();
    }

    public class ProdutoFreteDto
    {
        public string Id { get; set; } = string.Empty;

        public decimal Peso { get; set; }

        public int Altura { get; set; }

        public int Largura { get; set; }

        public int Comprimento { get; set; }

        public decimal Valor { get; set; }

        public int Quantidade { get; set; }
    }
}
