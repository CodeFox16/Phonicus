using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PhonicusBackend.Controllers
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

            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"Scripts/whisper_transcribe.py \"{filePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(psi);
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return Ok(new { transcript = output });
        }
    }
}