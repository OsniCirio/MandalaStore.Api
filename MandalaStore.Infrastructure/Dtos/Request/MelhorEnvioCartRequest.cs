using System.Text.Json.Serialization;

public class MelhorEnvioCartRequest
{
    public int Service { get; set; }

    public MelhorEnvioPessoa From { get; set; } = new();
    public MelhorEnvioPessoa To { get; set; } = new();

    public List<MelhorEnvioProduto> Products { get; set; } = [];
    public List<MelhorEnvioVolume> Volumes { get; set; } = [];

    public MelhorEnvioOptions Options { get; set; } = new();
}

public class MelhorEnvioPessoa
{
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Document { get; set; } = "";

    public string Address { get; set; } = "";
    public string Number { get; set; } = "";
    public string? Complement { get; set; }
    public string District { get; set; } = "";
    public string City { get; set; } = "";

    [JsonPropertyName("state_abbr")]
    public string StateAbbr { get; set; } = "";

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; } = "";

    [JsonPropertyName("country_id")]
    public string CountryId { get; set; } = "BR";
}

public class MelhorEnvioProduto
{
    public string Name { get; set; } = "";
    public int Quantity { get; set; }

    [JsonPropertyName("unitary_value")]
    public decimal UnitaryValue { get; set; }
}

public class MelhorEnvioVolume
{
    public decimal Height { get; set; }
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    public decimal Weight { get; set; }
}

public class MelhorEnvioOptions
{
    [JsonPropertyName("insurance_value")]
    public decimal InsuranceValue { get; set; }

    public bool Receipt { get; set; } = false;

    [JsonPropertyName("own_hand")]
    public bool OwnHand { get; set; } = false;

    public bool Reverse { get; set; } = false;

    [JsonPropertyName("non_commercial")]
    public bool NonCommercial { get; set; } = true;
}
