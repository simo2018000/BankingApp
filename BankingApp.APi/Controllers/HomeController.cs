using Microsoft.AspNetCore.Mvc;
using BankingApp.Shared.DTOs;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace BankingApp.Controllers
{
    [Authorize] // Assure que seuls les utilisateurs authentifiés peuvent accéder à ce controller
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        // Endpoint factice pour tester la sécurité sans toucher à la vraie BDD
        [HttpPost("login-simulation")]
        public IActionResult LoginSimulation([FromBody] LoginRequest request)
        {
            // On récupère l'IP pour le tracage SIEM
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // Simulation : Seul l'utilisateur "admin" avec mdp "secure123" passe
            if (request.Username == "admin" && request.Password == "secure123")
            {
                // Log de succès (Faible criticité)
                Log.Information("SECURITY_EVENT: Login Successful | User: {Username} | IP: {SrcIp}",
                    request.Username, ipAddress);

                return Ok(new { Token = "demo-token-123", Message = "Login réussi" });
            }
            else
            {
                // LOG CRITIQUE : C'est ce signal que QRadar va intercepter !
                Log.Warning("SECURITY_EVENT: Login Failed | User: {Username} | IP: {SrcIp} | Reason: Bad Credentials",
                    request.Username, ipAddress);

                return Unauthorized(new { Message = "Échec de l'authentification" });
            }
        }
    }
}