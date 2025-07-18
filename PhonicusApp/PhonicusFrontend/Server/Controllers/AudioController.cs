using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PhonicusFrontend.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioController : ControllerBase
    {
        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeAudio(IFormFile audioFile)
        {
            var filePath = Path.Combine("Temp", audioFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await audioFile.CopyToAsync(stream);
            }

            var scriptPath = @"C:\Personal\PhonicusApp\Scripts\whisper_transcribe.py";
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{filePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            var process = Process.Start(psi);
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return Ok(new { transcript = output });
        }
    }
}