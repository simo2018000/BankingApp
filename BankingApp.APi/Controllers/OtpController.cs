using BankingApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BankingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        // POST: api/otp/generate
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateOtp([FromQuery] Guid userId)
        {
            var otp = await _otpService.GenerateOtpAsync(userId);

            // Here you would normally send it via SMS/email
            Log.Information("OTP GENERATED for user {UserId} ", userId);

            return Ok(new { Message = "OTP generated successfully", Otp = otp });
        }

        // POST: api/otp/validate
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateOtp([FromQuery] Guid userId, [FromQuery] string code)
        {
            var isValid = await _otpService.ValidateOtpAsync(userId, code);

            if (!isValid)
            {
                Log.Warning("OTP Validation FAILED for user {UserId}", userId);
                return Unauthorized(new { Message = "Invalid or expired OTP" });
            }

            Log.Information("OTP Validation SUCCESS for user {UserId}", userId);
            return Ok(new { Message = "OTP validated successfully" });
        }

        // POST: api/otp/invalidate
        [HttpPost("invalidate")]
        public async Task<IActionResult> InvalidateOtp([FromQuery] Guid userId)
        {
            await _otpService.InvalidateOtpAsync(userId);

            return Ok(new { Message = "OTP invalidated" });
        }
    }
}
