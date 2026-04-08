using BankingApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BankingApp.Controllers
{
    [Authorize] // Ensure only authenticated users can access this controller (optional for simulation)
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Récupère l'IP pour que QRadar sache qui attaquer/bloquer
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // Simulation très simple : seul admin/password fonctionne
            if (request.Username == "admin" && request.Password == "password")
            {
                // LOG SUCCÈS (Info)
                Log.Information("SECURITY_EVENT: Login Successful | User: {Username} | IP: {SrcIp}",
                    request.Username, ipAddress);

                return Ok(new { Token = "fake-jwt-token-pour-le-lab", Message = "Login successful" });
            }
            else
            {
                // LOG ECHEC (Warning) -> C'est ça que le SIEM cherche !
                Log.Warning("SECURITY_EVENT: Login Failed | User: {Username} | IP: {SrcIp} | Reason: Bad Credentials",
                    request.Username, ipAddress);

                return Unauthorized(new { Message = "Invalid credentials" });
            }
        }
    }
}