using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using fourtitudeTest.Models;
using FourtitudeTest.Services;

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
        // Validate timestamp
        if (!DateTime.TryParse(timeStamp, out DateTime parsedDateTime))
        {
            return false;
        }

        return parsedDateTime.Kind == DateTimeKind.Utc && timeStamp.EndsWith("Z");
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

    public bool DecodeSignature(string signature,ApiRequest request)
    {
        try
        {
            // Decode the Base64 string into a byte array sdas
            byte[] decodedBytes = Convert.FromBase64String(signature);

            // Convert the byte array into a string (assuming UTF-8 encoding)
            string decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);

            // Step 3: Use a regular expression to split the string into its components
            // Assuming the string format is 'timestamp + partnerkey + partnerrefno + totalamount + partnerpassword(encoded)'
            string pattern = @"^(\d{14})([a-zA-Z0-9]{50})([a-zA-Z0-9]{50})(\d+)([a-zA-Z0-9+/=]+)$";
            Match match = Regex.Match(decodedString, pattern);

            if (match.Success)
            {
                // Extract the components
                string timestamp = match.Groups[1].Value;
                string partnerKey = match.Groups[2].Value;
                string partnerRefNo = match.Groups[3].Value;
                string totalAmount = match.Groups[4].Value;
                string partnerPasswordEncoded = match.Groups[5].Value;

                // Step 4: Validate timestamp format (yyyyMMddHHmmss)
                if (!DateTime.TryParseExact(timestamp, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    
                    return false;
                }

                if (partnerKey != request.partnerkey || partnerRefNo != request.partnerrefno || totalAmount != request.totalamount.ToString())
                {
                    
                    return false;
                }

                string decodedPassword = DecodeBase64(request.partnerpassword); 
                if (decodedPassword != partnerPasswordEncoded)
                {
                    
                    return false;
                }
            
                return true;
            }else
            {
                return false;
            }
        }catch(Exception ex)
        {
            return false;
        }   
        
      
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