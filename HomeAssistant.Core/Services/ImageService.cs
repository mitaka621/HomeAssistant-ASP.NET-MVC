using HomeAssistant.Core.Contracts;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace HomeAssistant.Core.Services
{
	public class ImageService : IimageService
    {
        private readonly IMongoClient _client;
        private IGridFSBucket _gridFS;

		public ImageService(IMongoClient client)
        {
            _client = client;
        }

        public async Task SavePFP(string userId, byte[] imageData)
        {
			_gridFS = new GridFSBucket(_client.GetDatabase("HomeAssistant"), new GridFSBucketOptions
			{
				BucketName = "ProfilePictures"
			});

			var existingImageFilter = Builders<GridFSFileInfo>.Filter.Eq("filename", userId);
            var existingImage = (await _gridFS.FindAsync(existingImageFilter)).FirstOrDefault();

            if (existingImage != null)
            {
                await _gridFS.DeleteAsync(existingImage.Id);
            }

            using (var stream = new MemoryStream(imageData))
            {
                var options = new GridFSUploadOptions
                {
                    Metadata = new BsonDocument("userId", userId)
                };

                await _gridFS.UploadFromStreamAsync(userId, stream, options);
            }


        }

        public async Task<byte[]> GetPFP(string userId)
        {
			_gridFS = new GridFSBucket(_client.GetDatabase("HomeAssistant"), new GridFSBucketOptions
			{
				BucketName = "ProfilePictures"
			});

			var filter = Builders<GridFSFileInfo>.Filter.Eq("filename", userId);
            var fileInfo = (await _gridFS.FindAsync(filter)).FirstOrDefault();

            if (fileInfo != null)
            {
                using (var stream = new MemoryStream())
                {
                    await _gridFS.DownloadToStreamAsync(fileInfo.Id, stream);
                    return stream.ToArray();
                }
            }
            else
            {
                filter = Builders<GridFSFileInfo>.Filter.Eq("filename", "defaultpfp");
                fileInfo = (await _gridFS.FindAsync(filter)).FirstOrDefault();
				using (var stream = new MemoryStream())
                {
                    await _gridFS.DownloadToStreamAsync(fileInfo.Id, stream);
                    return stream.ToArray();
                }
            }        
        }

		public async Task SaveRecipeImage(int recipeId, byte[] imageData)
		{
			_gridFS = new GridFSBucket(_client.GetDatabase("HomeAssistant"), new GridFSBucketOptions
			{
				BucketName = "RecipePictures"
			});

			var existingImageFilter = Builders<GridFSFileInfo>.Filter.Eq("filename", recipeId);
			var existingImage = (await _gridFS.FindAsync(existingImageFilter)).FirstOrDefault();

			if (existingImage != null)
			{
				await _gridFS.DeleteAsync(existingImage.Id);
			}

			using (var stream = new MemoryStream(imageData))
			{
				var options = new GridFSUploadOptions
				{
					Metadata = new BsonDocument("recipeId", recipeId)
				};

				await _gridFS.UploadFromStreamAsync(recipeId.ToString(), stream, options);
			}

		}

		public async Task<byte[]> GetRecipeImage(int recipeId, CancellationToken cancellationToken=new())
		{
			cancellationToken.ThrowIfCancellationRequested();
			_gridFS = new GridFSBucket(_client.GetDatabase("HomeAssistant"), new GridFSBucketOptions
			{
				BucketName = "RecipePictures"
			});

			var filter = Builders<GridFSFileInfo>.Filter.Eq("filename", recipeId.ToString());
			var fileInfo = (await _gridFS.FindAsync(filter)).FirstOrDefault();

			if (fileInfo != null)
			{
				cancellationToken.ThrowIfCancellationRequested();
				using (var stream = new MemoryStream())
				{
					await _gridFS.DownloadToStreamAsync(fileInfo.Id, stream);
					return stream.ToArray();
				}
			}

			return new byte[0];
		}
	}
}
