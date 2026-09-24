using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MandalaStore.Application.DTO.Response;
public class MelhorEnvioShippingResponse
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Price { get; set; }

    [JsonPropertyName("custom_price")]
    public string? CustomPrice { get; set; }

    public string? Currency { get; set; }

    [JsonPropertyName("delivery_time")]
    public int? DeliveryTime { get; set; }

    [JsonPropertyName("delivery_range")]
    public MelhorEnvioDeliveryRange? DeliveryRange { get; set; }

    public MelhorEnvioCompany? Company { get; set; }

    public string? Error { get; set; }
}

public class MelhorEnvioCompany
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Picture { get; set; }
}

public class MelhorEnvioDeliveryRange
{
    public int Min { get; set; }

    public int Max { get; set; }
}