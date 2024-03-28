using HomeAssistant.Core.Models.Recipe;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
	public class RecipeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Add(RecipeFormViewModel r)
		{
			return View();
		}
	}
}
