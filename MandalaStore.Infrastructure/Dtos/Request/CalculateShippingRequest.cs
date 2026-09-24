using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MandalaStore.Infrastructure.DTO;
using MandalaStore.Infrastructure.Dtos;

namespace MandalaStore.Infrastructure.DTO.Request;
public class CalculateShippingRequest
{
    public string CepDestino { get; set; } = "";

    public List<ShippingProductDto> Products { get; set; } = new();
}

