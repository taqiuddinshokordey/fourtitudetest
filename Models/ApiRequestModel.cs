using System.ComponentModel.DataAnnotations;

namespace fourtitudeTest.Models;
public class ApiRequest
{
    [Required(ErrorMessage = "partnerrefno is required.")]
    [StringLength(50, ErrorMessage = "partnerrefno must not exceed  50 characters.")]
    public string partnerrefno { get; set; } // The allowed partner's key

    [Required(ErrorMessage = "partnerkey is required.")]
    [StringLength(50, ErrorMessage = "partnerkey must not exceed 50 characters.")]
    public string partnerkey { get; set; } // Partner's reference number for this transaction

    [Required(ErrorMessage = "partnerpassword is required.")]
    [StringLength(50, ErrorMessage = "partnerpassword must not exceed 50 characters.")]
    public string partnerpassword { get; set; } // Base64 encoded format

    [Required(ErrorMessage = "totalamount is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "TotalAmount must be a positive value.")]
    public long totalamount { get; set; } // Total amount of payment in cents, must be positive

    [Required(ErrorMessage = "signature is required.")]
    public string sig { get; set; } // Base64 encoded format

    [Required(ErrorMessage = "timestamp is required.")]
    public string timestamp { get; set; } // ISO 8601 format string, UTC time    

    // Optional array of ItemDetail
    public ItemDetails[]? items { get; set; } // Array of ItemDetail (optional)

}