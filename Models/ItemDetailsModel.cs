using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace fourtitudeTest.Models;
public class ItemDetails
{
    [JsonPropertyName("partneritemref")]
    [Required(ErrorMessage = "partneritemref is required.")]
    [StringLength(50, ErrorMessage = "partneritemref must be at most 50 characters.")]
    public string? PartnerItemRef { get; set; }

    [Required(ErrorMessage = "name is required.")]
    [StringLength(100, ErrorMessage = "name must be at most 100 characters.")]
    public string? name { get; set; }
    
    [Range(2, 5, ErrorMessage = "Quantity must be greater than 1 and no more than 5.")]
    public int qty { get; set; }

    public long unitprice { get; set; }
}