using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Application.DTO.Response
{
    public class FreteResponse
    {
        public string Servico { get; set; }

        public decimal Valor { get; set; }

        public int Prazo { get; set; }
    }
}
