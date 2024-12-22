using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace fourtitudeTest.Models;
public class SuccessApiResponse
{
    public int result { get; set; } // The allowed partner's key

    [JsonPropertyName("totalamount")]
    [Range(1, long.MaxValue, ErrorMessage = "TotalAmount must be a positive value.")]
    public long TotalAmount { get; set; } // Partner's reference number for this transaction

    [JsonPropertyName("totaldiscount")]
    [Range(1, long.MaxValue, ErrorMessage = "TotalDiscount must be a positive value.")]
    public long TotalDiscount { get; set; } // Base64 encoded format

    [JsonPropertyName("finalamount")]
    [Range(1, long.MaxValue, ErrorMessage = "FinalAmount must be a positive value.")]
    public long? FinalAmount { get; set; } // Total amount of payment in cents, must be positive

}

public class FailedApiResponse
{
    public int result { get; set; } // The allowed partner's key

    [JsonPropertyName("resultmessage")]
    public string ResultMessage { get; set; } // ISO 8601 format string, UTC time

}   