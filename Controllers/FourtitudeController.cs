using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using fourtitudeTest.Models;
using fourtitudeTest.Services;
using FourtitudeTest.Services;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace fourtitudeTest.Controllers;

[ApiController]
public class FourtitudeController : Controller
{
    private static readonly ILog log = LogManager.GetLogger(typeof(FourtitudeController));
    CheckRequestValidity  _checkPartner = new CheckRequestValidity();
    //SuccessApiResponse _response = new SuccessApiResponse();
    DiscountCheck  _discountCheck = new DiscountCheck();
    
    [Route("api/submittrxmessage")]
    [HttpPost]
    public IActionResult Post([FromBody] ApiRequest request)
    {
        log.Info("Received request: " + JsonConvert.SerializeObject(request));
        
        #region Check Validity of Field
        //Check Partner Validity & Check Signature
        var isPasswordEncoded = _checkPartner.IsEncoded(request.partnerpassword);
        if(!isPasswordEncoded)
        {
            var failedResponse = new FailedApiResponse
            {
                result = 0,
                ResultMessage = "Invalid Password!"
            };

            log.Error("Password is not encoded.");
            return BadRequest(failedResponse);
        }   

        string decodedPassword = _checkPartner.DecodeBase64(request.partnerpassword);
        var isPartnerValid = _checkPartner.IsPartnerValid(request.partnerrefno,request.partnerkey,decodedPassword);
        var isSignatureValid = _checkPartner.DecodeSignature(request);    
        if(!isPartnerValid || !isSignatureValid)
        {
            var failedResponse = new FailedApiResponse
            {
                result = 0,
                ResultMessage = "Access Denied!"
            };

            log.Error("Partner or signature validation failed.");
            return BadRequest(failedResponse);
        }

        //Check if timestamp in UTC time of the operation in ISO 8601 format ie. 2024-08-15T02:11:22.0000000Z
        var isTimeStampValid = _checkPartner.isTimeStampValid(request.timestamp);   
        if(!isTimeStampValid){

            var failedResponse = new FailedApiResponse
            {
                result = 0,
                ResultMessage = "Invalid Timestamp!"
            };

            log.Error("Invalid timestamp.");
            return BadRequest(failedResponse);
        }

        //Check if provided timestamp exceed server time by 5 minutes
        var isTimeStampExceed = _checkPartner.isTimeStampExceed(request.timestamp);
        if(!isTimeStampExceed){
            var failedResponse = new FailedApiResponse
            {
                result = 0,
                ResultMessage = "Expired"
            };

            log.Error("Timestamp exceeded.");
            return BadRequest(failedResponse);
        }

         
        #endregion

        #region Check Item Details
        if(request.items != null)
        {
            foreach (var item in request.items)
            {
                var itemValidationResults = new List<ValidationResult>();
                bool isItemValid = Validator.TryValidateObject(item, new ValidationContext(item), itemValidationResults, true);
                
                if (!isItemValid)
                {
                    foreach (var validationResult in itemValidationResults)
                    {
                        var failedResponse = new FailedApiResponse
                        {
                            result = 0,
                            ResultMessage = validationResult.ErrorMessage
                        };

                        log.Error("Item validation failed: " + validationResult.ErrorMessage);
                        return BadRequest(failedResponse);
                    }
                }
            }

            //Check if total amount is equal to sum of all items
            var isTotalAmountValid = _checkPartner.IsTotalAmountValid(request.totalamount,request.items.ToList());
            if(!isTotalAmountValid)
            {
                var failedResponse = new FailedApiResponse
                {
                    result = 0,
                    ResultMessage = "Invalid Total Amount."
                };

                log.Error("Invalid total amount.");
                return BadRequest(failedResponse);
            }
        }


        #endregion  

        #region Calculate Discount
        // Calculate discount and final amount
        var (totalDiscount, finalAmount) = _discountCheck.CalculateDiscount(request.totalamount);

        #endregion
        
        
        // Create success response object
        var response = new SuccessApiResponse
        {
            result =1,
            TotalAmount = request.totalamount,
            TotalDiscount = totalDiscount,
            FinalAmount = finalAmount
            
        };

        log.Info("Success Response: " + JsonConvert.SerializeObject(response));
        // Return the consent data in the response
        //return Json(new { response });
        return Ok(new
        {
            result = response.result,
            totalamount = response.TotalAmount,
            totaldiscount = response.TotalDiscount,
            finalamount = response.FinalAmount
        });
        
    }

    
}