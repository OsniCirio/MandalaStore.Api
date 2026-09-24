using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Application.DTO.Response
{
    public class FreteResponseDto
    {
        public int Id { get; set; } 
        public string Servico { get; set; } = string.Empty;

        public decimal Valor { get; set; }

        public int Prazo { get; set; }
        public string Transportadora { get; set; } = string.Empty;
        public int PrazoDias { get; set; }
    }
}
