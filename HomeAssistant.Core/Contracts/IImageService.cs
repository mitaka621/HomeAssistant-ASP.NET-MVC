using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface IimageService
	{
		Task SavePFP(string userId, byte[] imageData);

		Task<byte[]> GetPFP(string userId);

		Task SaveRecipeImage(int RecipeId, byte[] imageData);

		Task<byte[]> GetRecipeImage(int RecipeId);
	}
}
