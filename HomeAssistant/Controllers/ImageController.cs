using System.Security.Claims;
using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{

    public class ImageController : Controller
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [Authorize(Roles = "StandardUser")]
        public async Task<IActionResult> Profile(string id)
        {
            byte[] image = await _imageService.GetPFP(id);

            if (image == null || image.Length == 0)
            {
                return NotFound();
            }

            return File(image, "image/jpeg");
        }

        public async Task<IActionResult> ProfileLoggedUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            byte[] image = await _imageService.GetPFP(userId);

            if (image == null || image.Length == 0)
            {
                return NotFound();
            }

            return File(image, "image/jpeg");
        }

        [Authorize(Roles = "StandardUser")]
        public async Task<IActionResult> Recipe(int id)
        {
            byte[] image = await _imageService.GetRecipeImage(id);

            if (image == null || image.Length == 0)
            {
                return NotFound();
            }

            return File(image, "image/jpeg");
        }
    }
}
