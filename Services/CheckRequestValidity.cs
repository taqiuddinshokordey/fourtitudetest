using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using fourtitudeTest.Models;
using FourtitudeTest.Services;
using Microsoft.Extensions.Primitives;

namespace fourtitudeTest.Services;
public class CheckRequestValidity
{

    public bool IsEncoded(string password)
    {
        // A quick check to ensure the input is not null or empty
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // Base64 strings should have a length that's a multiple of 4
        if (password.Length % 4 != 0)
            return false;

        // Attempt to decode it; return false if any exception occurs
        try
        {
            Convert.FromBase64String(password);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }


    public bool IsPartnerValid(string partnerNo, string partnerName, string partnerPassword)
    {
        PartnerService _partnerServices = new PartnerService();
        if (string.IsNullOrWhiteSpace(partnerNo) || 
            string.IsNullOrWhiteSpace(partnerName) || 
            string.IsNullOrWhiteSpace(partnerPassword))
        {
            // Return false if any input is invalid or null
            return false;
        }

        // Retrieve partner details from the service
        var partner = _partnerServices.GetPartners();

        foreach (var item in partner)
        {
            if (item.PartnerNo == partnerNo && item.AllowedPartner == partnerName && item.PartnerPassword == partnerPassword)
            {
                return true;
            }
        }

        return false;
    }

    public bool isTimeStampValid(string timeStamp)
    {
        string format = "yyyy-MM-ddTHH:mm:ss.fffffffZ";
        return DateTime.TryParseExact(
            timeStamp, 
            format, 
            CultureInfo.InvariantCulture, 
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, 
            out _);
    }

    public string DecodeBase64(string base64Encoded)
    {
        // Check if the input string is null or empty
        if (string.IsNullOrEmpty(base64Encoded))
        {
            throw new ArgumentException("Input string cannot be null or empty", nameof(base64Encoded));
        }

        // Decode the Base64 string into a byte array
        byte[] decodedBytes = Convert.FromBase64String(base64Encoded);

        // Convert the byte array into a string (assuming UTF-8 encoding)
        string decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);

        return decodedString;
    }

    public bool DecodeSignature(ApiRequest rq)
    {
        string signatureParameterOrder = GenerateSignatureParameterOrder(rq.timestamp, rq.partnerkey, rq.partnerrefno, rq.totalamount, rq.partnerpassword);
        string signatureGen = GenerateSHA256(signatureParameterOrder);
        string encodedSignature = EncodeToBase64(signatureGen);

        if (rq.sig == encodedSignature)    
        {
            return true;
        }

        return false;
      
    }

    private string GenerateSHA256(string input)
    {
        // Compute the SHA-256 hash
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Convert hash bytes to a hexadecimal string
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2")); // Format as two-digit hexadecimal
            }

            return sb.ToString();
        }
    }

    private string EncodeToBase64(string input)
    {
        // Convert the input string to a byte array
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        // Convert the byte array to a Base64 encoded string
        return Convert.ToBase64String(inputBytes);
    }

    private string GenerateSignatureParameterOrder(string timestamp, string partnerKey, string partnerRefNo, long totalAmount, string partnerPassword)
    {

        // Parse the ISO 8601 timestamp
        DateTime parsedTimestamp = DateTime.ParseExact(timestamp, "yyyy-MM-ddTHH:mm:ss.fffffffZ", null, System.Globalization.DateTimeStyles.AdjustToUniversal);

        // Convert to the desired format
        string formattedTimestamp = parsedTimestamp.ToString("yyyyMMddHHmmss");
        // Generate timestamp in yyyyMMddHHmmss format

        // Concatenate the values
        string data = formattedTimestamp + partnerKey + partnerRefNo + totalAmount + partnerPassword;

        // Compute the SHA256 hash
        return data;
    }

    public bool isTimeStampExceed(string timeStamp)
    {
        // Validate timestamp
        if (!DateTime.TryParse(timeStamp, out DateTime parsedDateTime))
        {
            return false;
        }

        // Check if the timestamp exceeds the server time by 5 minutes
        return parsedDateTime.AddMinutes(5) < DateTime.UtcNow;
    }

    public bool IsTotalAmountValid(long totalAmount, List<ItemDetails> itemDetails)
    {
        // Calculate the sum of totalValue from all items
        long calculatedTotalValue = itemDetails.Sum(item => item.qty * item.unitprice);

        // Compare it with the provided totalAmount
        return totalAmount == calculatedTotalValue;
    }
    
}