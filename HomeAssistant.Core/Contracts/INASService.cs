using HomeAssistant.Core.Models.NAS;

namespace HomeAssistant.Core.Contracts
{
	public interface INASHostService
	{
		Task<IEnumerable<DirViewModel>?> GetData(string path, int skip = 0, int take = 100);

		Task<HttpResponseMessage?> GetFileString(string path);

		Task<HttpResponseMessage?> GetPhoto(string path, bool isFull = false);

		Task<PhotoPrevNextPaths?> GetPrevAndNextPhotoLocation(string currentPhotoPath);

		Task<DirViewModel?> GetPhotoInfo(string path);

		Task<bool> CheckConnection();

		Task<bool> ScanForAvailibleHost();

		Task<HttpResponseMessage?> GetVideoRange(string path, string rangeHeader);
	}
}
