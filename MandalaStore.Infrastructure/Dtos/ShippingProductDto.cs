using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Infrastructure.Dtos;

public class ShippingProductDto
{
    public string Id { get; set; } = "";

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public decimal Length { get; set; }

    public decimal Weight { get; set; }

    public decimal InsuranceValue { get; set; }

    public int Quantity { get; set; }
}