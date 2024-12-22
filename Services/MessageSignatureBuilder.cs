using System.Security.Cryptography;

namespace fourtitudeTest.Services;
public class MessageSignatureBuilder
{
    public string BuildSignatureString(string PartnerKey, string PartnerRefNo, long TotalAmount, string PartnerPassword)
    {
        //Encode the PartnerPasswword
        var encodedPassword = HashToBase64(PartnerPassword);
        // Format the timestamp as yyyyMMddHHmmss
        string formattedTimestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        var siganture = $"{formattedTimestamp}{PartnerKey}{PartnerRefNo}{TotalAmount}{encodedPassword}";

        return siganture;
        
    }

    public string HashToBase64(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null or empty", nameof(input));
        }

        using (SHA256 sha256 = SHA256.Create())
        {
            // Convert the input string to a byte array
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

            // Compute the SHA-256 hash
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // Encode the hash in Base64 and return it
            return Convert.ToBase64String(hashBytes);
        }
    }
}