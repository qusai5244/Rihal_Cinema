using Rihal_Cinema.Data;
using Rihal_Cinema.Dtos.Movie;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Helpers;
using Rihal_Cinema.Services.Interfaces;
using Rihal_Cinema.Dtos.Memory;
using Rihal_Cinema.Models;
using Rihal_Cinema.Enums;
using Microsoft.EntityFrameworkCore;
using StopWord;
using Minio;
using Minio.DataModel.Args;
using System.Net.Mime;
using System.Security.AccessControl;
using Minio.DataModel;
using Rihal_Cinema.Dtos.Photo;
using System.Xml.Linq;
using Minio.Exceptions;
using Rihal_Cinema.Infrastructure.ServiceContext;
using System;
using System.Text.RegularExpressions;

namespace Rihal_Cinema.Services
{
    public class MemoryService : IMemoryService
    {
        private readonly DataContext _dataContext;
        public RequestHeaderContent Header {  get; set; }
        public MemoryService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiResponse<MemoriesOutputDto>> CreateMemory(MemoryInputDto input)
        {
            try
            {

                var isValidMovie = await _dataContext.Movies.AnyAsync(m => m.Id == input.MovieId);

                if (!isValidMovie)
                {
                    return new ApiResponse<MemoriesOutputDto>(true, (int)ResponseCodeEnum.BadRequest, "Movie Not Found", null);
                }

                var memory = new Memory
                {
                    UserId = Header.UserId,
                    MovieId = input.MovieId,
                    Title = input.Title,
                    TakenOn = input.TakenOn.ToUniversalTime().AddHours(12),
                    Story = input.Story,
                };

                _dataContext.Memories.Add(memory);
                await _dataContext.SaveChangesAsync();

                if (input.Image != null)
                {
                    var uplodedImg = UploadImgToCloud(input.Image);

                    if (uplodedImg == null)
                    {
                        return new ApiResponse<MemoriesOutputDto>(true, (int)ResponseCodeEnum.BadRequest, "An Error Occurred While Uploading the Image", null);
                    }

                    var photo = new Photo
                    {
                        MemoryId = memory.Id,
                        Name = uplodedImg.Result.Name,
                        Extension = uplodedImg.Result.Extension,
                        Size = uplodedImg.Result.Size,
                        StoredName = uplodedImg.Result.StoredName,
                    };

                    _dataContext.Photos.Add(photo);
                    await _dataContext.SaveChangesAsync();
                }

                var memoryOutput = new MemoriesOutputDto
                {
                    Id = memory.Id,
                    MovieId = memory.MovieId,
                    Title = memory.Title,
                    TakenOn = memory.TakenOn,
                    Story = memory.Story,
                };

                return new ApiResponse<MemoriesOutputDto>(true, (int)ResponseCodeEnum.Success, "Memory Added Successfully", memoryOutput);

            }
            catch (Exception)
            {
                return new ApiResponse<MemoriesOutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while creating Memory", null);
            }
        }
        public async Task<ApiResponse<PaginatedList<MemoriesOutputDto>>> GetMemories(PaginationInputDto input)
        {
            try
            {
                var page = input.Page > 0 ? input.Page : 1;
                var pageSize = input.PageSize > 0 ? input.PageSize : 50;

                var memoriesQuery = _dataContext
                                     .Memories
                                     .Include(m => m.Movie)
                                     .AsNoTracking()
                                     .Where(m => m.UserId == Header.UserId);

                var memoriesCount = await memoriesQuery.CountAsync();

                if (memoriesCount == 0)
                {
                    return new ApiResponse<PaginatedList<MemoriesOutputDto>>(false, (int)ResponseCodeEnum.NotFound, "No Memory Found", null);
                }

                var memories = await memoriesQuery
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .Select(m => new MemoriesOutputDto
                                     {
                                         Id = m.Id,
                                         MovieId = m.MovieId,
                                         MovieName = m.Movie.Name,
                                         Title = m.Title,
                                         TakenOn = m.TakenOn,
                                         Story = m.Story,
                                     })
                                     .ToListAsync();



                var pagintaedList = new PaginatedList<MemoriesOutputDto>(memories, memoriesCount, page, pageSize);

                return new ApiResponse<PaginatedList<MemoriesOutputDto>>(true, (int)ResponseCodeEnum.Success, "Memories Retrieved successfully", pagintaedList);

            }
            catch (Exception)
            {
                return new ApiResponse<PaginatedList<MemoriesOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while getting Memories", null);
            }
        }
        public async Task<ApiResponse<MemoryOutputDto>> GetMemory(int memoryId)
        {
            try
            {

                var memory = await _dataContext
                                  .Memories
                                  .Include(m => m.Movie)
                                  .AsNoTracking()
                                  .Where(m => m.Id == memoryId && m.UserId == Header.UserId)
                                  .FirstOrDefaultAsync();

                if (memory == null)
                {
                    return new ApiResponse<MemoryOutputDto>(false, (int)ResponseCodeEnum.NotFound, "Memory Not Found", null);
                }

                var PhotoDto = await _dataContext
                                     .Photos
                                     .AsNoTracking()
                                     .Where(p => p.MemoryId == memoryId)
                                     .Select(p => new PhotoListOutputDto
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Extension = p.Extension,
                                         Size = (p.Size / 1000).ToString("0.00") + " KB",
                                         CreatedAt = p.CreatedAt
                                     })
                                     .ToListAsync();

                var memoryDto = new MemoryOutputDto
                {
                    Id = memoryId,
                    MovieId = memory.MovieId,
                    MovieName = memory.Movie.Name,
                    Story = memory.Story,
                    Title = memory.Title,
                    Photos = PhotoDto
                };
                return new ApiResponse<MemoryOutputDto>(true, (int)ResponseCodeEnum.Success, "Memory Retrieved successfully", memoryDto);

            }
            catch (Exception)
            {
                return new ApiResponse<MemoryOutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while getting Memories", null);
            }
        }
        public async Task<ApiResponse<MemoryOutputDto>> UpdateMemory(MemoryUpdateInputDto input)
        {
            try
            {
                var memory = await _dataContext
                                 .Memories
                                 .Where(m => m.Id == input.MemoryId && m.UserId == Header.UserId)
                                 .FirstOrDefaultAsync();

                if (memory == null)
                {
                    return new ApiResponse<MemoryOutputDto>(false, (int)ResponseCodeEnum.NotFound, "Memory Not Found", null);
                }

                if (!string.IsNullOrEmpty(input.Title))
                {
                    memory.Title = input.Title;
                }

                if (!string.IsNullOrEmpty(input.Story))
                {
                    memory.Story = input.Story;
                }

                await _dataContext.SaveChangesAsync();

                return new ApiResponse<MemoryOutputDto>(true, (int)ResponseCodeEnum.Success, "Memory Has Been Updated", null);

            }
            catch (Exception)
            {
                return new ApiResponse<MemoryOutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while Updating Memory", null);
            }

        }
        public async Task<ApiResponse<MemoryOutputDto>> DeleteMemory(int memoryId)
        {
            try
            {
                var memory = await _dataContext
                                 .Memories
                                 .Where(m => m.Id == memoryId && m.UserId == Header.UserId)
                                 .FirstOrDefaultAsync();

                if (memory == null)
                {
                    return new ApiResponse<MemoryOutputDto>(false, (int)ResponseCodeEnum.NotFound, "Memory Not Found", null);
                }

                var photos = await _dataContext
                                   .Photos
                                   .Where(p => p.MemoryId == memory.Id)
                                   .ToListAsync();

                if(photos.Any())
                {
                    var photosStoredName = photos.Select(p => p.StoredName).ToList();
                    await DeletePhoto(photosStoredName);
                    _dataContext.Photos.RemoveRange(photos);
                }

                _dataContext.Memories.Remove(memory);
                await _dataContext.SaveChangesAsync();

                return new ApiResponse<MemoryOutputDto>(true, (int)ResponseCodeEnum.Success, "Memory Has Been Deleted", null);

            }
            catch (Exception)
            {
                return new ApiResponse<MemoryOutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while Deleting Memory", null);
            }

        }
        public async Task<ApiResponse<List<MemoryTopFiveUsedWordsDto>>> GetTopFiveUsedWords()
        {
            try
            {
                var memoriesStories = await _dataContext
                                        .Memories
                                        .AsNoTracking()
                                        .Select(m => m.Story)
                                        .ToListAsync();

                if (!memoriesStories.Any())
                {
                    return new ApiResponse<List<MemoryTopFiveUsedWordsDto>>(false, (int)ResponseCodeEnum.NotFound, "Memories Not Found", null);
                }

                var combinedStories = string.Join(" ", memoriesStories);

                var stopWords = StopWords.GetStopWords("en");

                // Split the combined stories into words, ignoring punctuation
                var words = combinedStories.Split(new[] { ' ', ',', '.', ';', ':', '!', '?','-','_' }, StringSplitOptions.RemoveEmptyEntries);

                // Count the occurrences of each word, ignoring stop words
                var wordCounts = words
                    .Where(word => !stopWords.Contains(word.ToLower()))
                    .GroupBy(word => word.ToLower())
                    .Select(group => new { Word = group.Key, Count = group.Count() })
                    .OrderByDescending(item => item.Count)
                    .Take(5)
                    .ToList();

                var topFiveWordsDto = wordCounts.Select(item => new MemoryTopFiveUsedWordsDto { Word = item.Word, Count = item.Count }).ToList();

                return new ApiResponse<List<MemoryTopFiveUsedWordsDto>>(true, (int)ResponseCodeEnum.Success, "Top Five Used Words Retrieved Successfully", topFiveWordsDto);
            }
            catch (Exception)
            {
                return new ApiResponse<List<MemoryTopFiveUsedWordsDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred While Getting Top Five Used Words in Memories", null);
            }
        }
        public async Task<byte[]> GetImage(int imageId)
        {
            try
            {
                var photoStoredName = await _dataContext
                                        .Photos
                                        .Include(p=> p.Memory)
                                        .AsNoTracking()
                                        .Where(p => p.Id == imageId && p.Memory.UserId == Header.UserId)
                                        .Select(p => p.StoredName)
                                        .FirstOrDefaultAsync();

                if (photoStoredName == null)
                {
                    return null;
                }

                // Get the image bytes from the URL
                var imageUrl = await GetImgUrl(photoStoredName);
                if (imageUrl == null)
                {
                    return null;
                }
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                return imageBytes;
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        public async Task<ApiResponse<string>> DeleteMemoryPhoto(int photoId)
        {
            try
            {
                var photo = await _dataContext
                                   .Photos
                                   .Where(p => p.Id == photoId && p.Memory.UserId == Header.UserId)
                                   .FirstOrDefaultAsync();

                if (photo == null)
                {
                    return new ApiResponse<string>(false, (int)ResponseCodeEnum.NotFound, "Photo Not Found", null);
                }

                var photoList = new List<string>
                {
                    photo.StoredName
                };

                await DeletePhoto(photoList);
                _dataContext.Photos.Remove(photo);
                await _dataContext.SaveChangesAsync();

                return new ApiResponse<string>(true, (int)ResponseCodeEnum.Success, "Photo Deleted Successfully", null);

            }
            catch (Exception)
            {
                return new ApiResponse<string>(false, (int)ResponseCodeEnum.InternalServerError, "An Error occurred While Deleting The Photo", null);
            }
        }
        public async Task<ApiResponse<string>> AddMemoryPhoto(PhotoUploadInputDto input)
        {
            try
            {
                var memory = await _dataContext
                                   .Memories
                                   .AsNoTracking()
                                   .Where(m => m.Id == input.MemoryId && m.UserId == Header.UserId)
                                   .AnyAsync();

                if (!memory)
                {
                    return new ApiResponse<string>(false, (int)ResponseCodeEnum.NotFound, "Memory Not Found", null);
                }

                var uplodedImg = UploadImgToCloud(input.Image);

                if (uplodedImg == null)
                {
                    return new ApiResponse<string>(false, (int)ResponseCodeEnum.BadRequest, "An Error Occurred While Uploading the Image", null);
                }

                var photo = new Photo
                {
                    MemoryId = input.MemoryId,
                    Name = uplodedImg.Result.Name,
                    Extension = uplodedImg.Result.Extension,
                    Size = uplodedImg.Result.Size,
                    StoredName = uplodedImg.Result.StoredName,
                };

                _dataContext.Photos.Add(photo);
                await _dataContext.SaveChangesAsync();

                return new ApiResponse<string>(true, (int)ResponseCodeEnum.Success, "Image Uploaded Successfully", null);

            }
            catch (Exception)
            {
                return new ApiResponse<string>(false, (int)ResponseCodeEnum.InternalServerError, "An Error occurred While Uploading The Photo", null);
            }
        }
        public async Task<ApiResponse<List<string>>> ExtractLinksFromMemoryStory(int memoryId)
        {
            try
            {
                var memory = await _dataContext
                                    .Memories
                                    .AsNoTracking()
                                    .Where(m => m.Id == memoryId)
                                    .Select(m => m.Story)
                                    .FirstOrDefaultAsync();

                if (memory == null)
                {
                    return new ApiResponse<List<string>>(false, (int)ResponseCodeEnum.NotFound, "Memory Not Found", null);
                }

                // Extract links from the memory story using regular expressions
                var links = ExtractLinks(memory);

                return new ApiResponse<List<string>>(true, (int)ResponseCodeEnum.Success, "Links extracted successfully", links);
            }
            catch (Exception)
            {
                return new ApiResponse<List<string>>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred While Extracting Links From Memory Story", null);
            }
        }

        // Private Functions
        private static async Task<PhotoUploadOutputDto> UploadImgToCloud(IFormFile image)
        {
            try
            {
                var minioFactory = new PrivateMinioClient();
                using var minioClient = minioFactory.CreateMinioClient();

                string bucketName = await EnsureBucketExistsAsync(PhotoActionEnum.Add);

                using var stream = image.OpenReadStream();

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var fileExtension = GetFileExtensionFromContentType(image.ContentType);

                // Upload the image
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithContentType(fileExtension)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length); // Pass the length of stream data


                var response = await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                if (response == null)
                {
                    return null;
                }

                return new PhotoUploadOutputDto
                {
                    Name = image.FileName,
                    Extension = fileExtension,
                    StoredName = fileName,
                    Size = stream.Length
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static async Task<string> GetImgUrl(string storedName)
        {
            var minioFactory = new PrivateMinioClient();
            using var minioClient = minioFactory.CreateMinioClient();

            string bucketName = await EnsureBucketExistsAsync(PhotoActionEnum.Get);

            if (string.IsNullOrEmpty(bucketName))
            {
                return null;
            }

            // Generate URL for the image
            var args = new PresignedGetObjectArgs().WithBucket(bucketName).WithObject(storedName).WithExpiry(60);
            var url = await minioClient.PresignedGetObjectAsync(args);
            return url;
        }
        private static async Task<string> EnsureBucketExistsAsync(PhotoActionEnum photoActionEnum)
        {
            var minioFactory = new PrivateMinioClient();
            using var minioClient = minioFactory.CreateMinioClient();

            var bucketName = "rihal";
            // Check if the bucket exists
            var beArgs = new BucketExistsArgs().WithBucket(bucketName);
            bool found = await minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);

            if (!found)
            {
                if(photoActionEnum != PhotoActionEnum.Add)
                {
                    return null;
                };

                // If the bucket does not exist, create it
                var newBucket = new MakeBucketArgs().WithBucket(bucketName);
                await minioClient.MakeBucketAsync(newBucket).ConfigureAwait(false);
            }

            return bucketName;
        }
        private static async Task<bool> DeletePhoto(List<string> objects)
        {
            try
            {
                var minioFactory = new PrivateMinioClient();
                using var minioClient = minioFactory.CreateMinioClient();

                string bucketName = await EnsureBucketExistsAsync(PhotoActionEnum.Delete);

                if (string.IsNullOrEmpty(bucketName))
                {
                    return false;
                }

                foreach (var obj in objects)
                {
                    // Specify arguments for object removal
                    var removeArgs = new RemoveObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(obj);

                    // Delete the object
                    await minioClient.RemoveObjectAsync(removeArgs).ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        private static string GetFileExtensionFromContentType(string contentType)
        {
            // Extract the file extension from content type
            switch (contentType)
            {
                case "image/png":
                    return "png";
                case "image/jpeg":
                    return "jpeg";
                // Add more cases as needed for other content types
                default:
                    return "jpg"; // default to jpg if content type is not recognized
            }
        }
        private static List<string> ExtractLinks(string text)
        {
            var links = new List<string>();

            // Regular expression to find URLs in the text
            var regex = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Find matches
            var matches = regex.Matches(text);

            // Add matches to the list
            foreach (Match match in matches)
            {
                links.Add(match.Value);
            }

            return links;
        }
    }
}

