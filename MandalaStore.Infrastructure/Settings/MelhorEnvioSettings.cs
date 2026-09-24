using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Infrastructure.Settings;
public class MelhorEnvioSettings
{
    public string BaseUrl { get; set; } = "";
    public string Token { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string CepOrigem { get; set; } = "";
    public string UserAgent { get; set; } = "";
    public string Application { get; set; } = "";
    public MelhorEnvioRemetenteSettings Remetente { get; set; } = new MelhorEnvioRemetenteSettings();
}
public class MelhorEnvioRemetenteSettings
{
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
}