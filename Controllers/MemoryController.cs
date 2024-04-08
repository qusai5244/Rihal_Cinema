using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.Memory;
using Rihal_Cinema.Dtos.Photo;
using Rihal_Cinema.Enums;
using Rihal_Cinema.Helpers;
using Rihal_Cinema.Services;
using Rihal_Cinema.Services.Interfaces;

namespace Rihal_Cinema.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Consumes("multipart/form-data")]
    public class MemoryController : BaseController
    {
        private readonly IMemoryService _memoryService;

        public MemoryController(IMemoryService memoryService)
        {
            _memoryService = memoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMemories([FromQuery] PaginationInputDto input)
        {
            _memoryService.Header = BindRequestHeader();
            var response = await _memoryService.GetMemories(input);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemory([FromRoute] int id)
        {
            _memoryService.Header = BindRequestHeader();
            var response = await _memoryService.GetMemory(id);

            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateMemory([FromForm] MemoryInputDto input)
        {
            _memoryService.Header = BindRequestHeader();
            var response = await _memoryService.CreateMemory(input);

            return Ok(response);
        }

        [HttpGet("Photo/{id}")]
        public async Task<IActionResult> GetPhoto([FromRoute] int id)
        {
            _memoryService.Header = BindRequestHeader();
            var imageBytes = await _memoryService.GetImage(id);

            if (imageBytes == null || imageBytes.Length == 0)
            {
                return Ok("Image Not Found");
            }

            return File(imageBytes, "image/jpeg");
        }

        [HttpDelete("Photo/{id}")]
        public async Task<IActionResult> DeletePhoto([FromRoute] int id)
        {
            _memoryService.Header = BindRequestHeader();
            var response = await _memoryService.DeleteMemoryPhoto(id);

            return Ok(response);
        }

        [HttpPost("UploadPhoto")]
        public async Task<IActionResult> AddPhoto([FromForm]PhotoUploadInputDto input)
        {
            _memoryService.Header = BindRequestHeader();
            var response = await _memoryService.AddMemoryPhoto(input);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMemory([FromRoute] int id)
        {
            _memoryService.Header = BindRequestHeader();
            var response = await _memoryService.DeleteMemory(id);

            return Ok(response);
        }
        [Consumes("application/json")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateMemory([FromBody] MemoryUpdateInputDto input)
        {
            _memoryService.Header = BindRequestHeader();
            var response = await _memoryService.UpdateMemory(input);

            return Ok(response);
        }

        [HttpGet("TopFiveUsedWords")]
        public async Task<IActionResult> GetTopFiveUsedWords()
        {
            var response = await _memoryService.GetTopFiveUsedWords();

            return Ok(response);
        }
    }
}
