
namespace MandalaStore.Infrastructure.DTO;
public class ShippingOptionDto
{
    public int  Id { get; set; } 
    public string Carrier { get; set; } = string.Empty; 

    public string Service { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int DeliveryDays { get; set; }
}
