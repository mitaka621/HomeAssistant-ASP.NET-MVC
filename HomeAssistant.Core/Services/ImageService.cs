using HomeAssistant.Core.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace HomeAssistant.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly IMongoClient _client;
        private IGridFSBucket _gridFS = null!;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        public ImageService(IMongoClient client, ILogger<ImageService> logger, IWebHostEnvironment env)
        {
            _client = client;
            _logger = logger;
            _env = env;
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
            try
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
                    return await GetDefaultPfp();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Atlas Mongodb error");
                return [];
            }
        }

        public async Task<Dictionary<string, byte[]>> GetPfpRange(params string[] userIds)
        {
            try
            {
                _gridFS = new GridFSBucket(_client.GetDatabase("HomeAssistant"), new GridFSBucketOptions
                {
                    BucketName = "ProfilePictures"
                });

                byte[] defaultPfp = await GetDefaultPfp();

                var filter = Builders<GridFSFileInfo>.Filter.In("filename", userIds);
                var cursor = await _gridFS.FindAsync(filter);

                var picturesDict = new Dictionary<string, byte[]>();

                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var fileInf in batch)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await _gridFS.DownloadToStreamAsync(fileInf.Id, stream);
                            picturesDict[fileInf.Filename] = stream.ToArray();
                        }
                    }
                }

                foreach (var userId in userIds)
                {
                    if (!picturesDict.ContainsKey(userId) && defaultPfp != null)
                    {
                        picturesDict[userId] = defaultPfp;
                    }
                }

                return picturesDict;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Atlas Mongodb error");
                return new Dictionary<string, byte[]>();
            }

        }

        public async Task SaveRecipeImage(int recipeId, byte[] imageData)
        {
            _gridFS = new GridFSBucket(_client.GetDatabase("HomeAssistant"), new GridFSBucketOptions
            {
                BucketName = "RecipePictures"
            });

            var existingImageFilter = Builders<GridFSFileInfo>.Filter.Eq("filename", recipeId.ToString());
            var existingImage = await _gridFS.FindAsync(existingImageFilter);

            if (existingImage != null)
            {
                await existingImage.ForEachAsync(x => _gridFS.DeleteAsync(x.Id));
            }

            using (var stream = new MemoryStream(imageData))
            {
                var options = new GridFSUploadOptions
                {
                    Metadata = new BsonDocument("recipeId", recipeId.ToString())
                };

                var id = await _gridFS.UploadFromStreamAsync(recipeId.ToString(), stream, options);
            }
        }

        public async Task DeleteIfExistsRecipeImg(int recipeId)
        {
            _gridFS = new GridFSBucket(_client.GetDatabase("HomeAssistant"), new GridFSBucketOptions
            {
                BucketName = "RecipePictures"
            });

            var existingImageFilter = Builders<GridFSFileInfo>.Filter.Eq("filename", recipeId.ToString());
            var existingImage = await _gridFS.FindAsync(existingImageFilter);

            if (existingImage != null)
            {
                await existingImage.ForEachAsync(x => _gridFS.DeleteAsync(x.Id));
            }
        }

        public async Task<byte[]> GetRecipeImage(int recipeId, CancellationToken cancellationToken = new())
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

            return await GetDefaultRecipeImage();
        }

        public async Task<Dictionary<int, byte[]>> GetRecipeImageRange(int[] recipeIds)
        {
            try
            {
                _gridFS = new GridFSBucket(_client.GetDatabase("HomeAssistant"), new GridFSBucketOptions
                {
                    BucketName = "RecipePictures"
                });

                var filter = Builders<GridFSFileInfo>.Filter.In("filename", recipeIds.Select(x => x.ToString()).ToList());
                var cursor = await _gridFS.FindAsync(filter);

                var picturesDict = new Dictionary<int, byte[]>();

                var tasks = new List<Task>();
                foreach (var fileInf in await cursor.ToListAsync())
                {
                    var id = int.Parse(fileInf.Filename);
                    var downloadTask = DownloadImageAsync(fileInf.Id, id, picturesDict);
                    tasks.Add(downloadTask);
                }

                await Task.WhenAll(tasks);

                foreach (var item in recipeIds)
                {
                    if (!picturesDict.ContainsKey(item))
                    {
                        picturesDict[item] = new byte[0];
                    }
                }

                return picturesDict;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Dictionary<int, byte[]>();
            }
        }

        private async Task DownloadImageAsync(ObjectId fileId, int id, Dictionary<int, byte[]> picturesDict)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await _gridFS.DownloadToStreamAsync(fileId, stream);
                    picturesDict[id] = stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image with id {id}: {ex.Message}");
                picturesDict[id] = new byte[0];
            }
        }

        private async Task<byte[]> GetDefaultPfp()
        {
            var imagePath = Path.Combine(_env.WebRootPath, "images", "default-profile.png");
            return File.Exists(imagePath) ? await File.ReadAllBytesAsync(imagePath) : [];
        }

        private async Task<byte[]> GetDefaultRecipeImage()
        {
            var imagePath = Path.Combine(_env.WebRootPath, "images", "default-recipe.jpg");
            return File.Exists(imagePath) ? await File.ReadAllBytesAsync(imagePath) : [];
        }
    }
}
