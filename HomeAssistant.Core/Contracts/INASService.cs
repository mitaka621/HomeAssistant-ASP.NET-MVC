using HomeAssistant.Core.Models.NAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistant.Core.Contracts
{
	public interface INASService
	{
		Task<IEnumerable<DirViewModel>?> GetData(string path,int skip= 0, int take = 100);

		Task<HttpResponseMessage?> GetFileString(string path);

		Task<HttpResponseMessage?> GetPhoto(string path,bool isFull=false);

		Task<PhotoPrevNextPaths?> GetPrevAndNextPhotoLocation(string currentPhotoPath);

    }
}
