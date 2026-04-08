using Microsoft.AspNetCore.Mvc;
using BankingApp.Shared.DTOs; // Uses your existing LoginRequest model
using Serilog;

namespace BankingApp.APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        // This controller is a "Simulation Sensor" for your SOC Lab.
        // It does not touch the real database to avoid locking real accounts.
        // Its sole purpose is to generate structured logs for QRadar/Logstash.

        [HttpPost("simulate-login")]
        public IActionResult SimulateLogin([FromBody] LoginRequest request)
        {
            // 1. Capture the IP (Crucial for SIEM correlation)
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // 2. Simulation Logic (Hardcoded for the Lab)
            // Scenario: Only "admin" with "Password123!" is allowed.
            if (request.Username == "admin" && request.Password == "Password123!")
            {
                // LOG SUCCESS (EventID: 1001)
                // Information level logs might be ignored by QRadar depending on config
                Log.Information("SECURITY_EVENT: Login Successful | User: {Username} | IP: {SrcIp}",
                    request.Username, ipAddress);

                return Ok(new { Message = "Simulation: Login Success" });
            }
            else
            {
                // LOG FAILURE (EventID: 4625 - The one we want!)
                // Warning level makes it stand out.
                // The text "SECURITY_EVENT" is the tag our Logstash pipeline looks for.
                Log.Warning("SECURITY_EVENT: Login Failed | User: {Username} | IP: {SrcIp} | Reason: Bad Credentials",
                    request.Username, ipAddress);

                return Unauthorized(new { Message = "Simulation: Login Failed" });
            }
        }

        [HttpGet("simulate-attack")]
        public IActionResult SimulateAttack()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // Simulation of a more severe attack (SQL Injection probe)
            Log.Error("SECURITY_EVENT: SQL Injection Attempt Detected | IP: {SrcIp} | Payload: ' OR 1=1 --", ipAddress);

            return BadRequest("Malicious payload detected");
        }
    }
}