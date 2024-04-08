using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.Memory;
using Rihal_Cinema.Dtos.Photo;
using Rihal_Cinema.Helpers;
using Rihal_Cinema.Infrastructure.ServiceContext;

namespace Rihal_Cinema.Services.Interfaces
{
    public interface IMemoryService : IRequestHeader
    {
        Task<ApiResponse<string>> AddMemoryPhoto(PhotoUploadInputDto input);
        Task<ApiResponse<MemoriesOutputDto>> CreateMemory(MemoryInputDto input);
        Task<ApiResponse<MemoryOutputDto>> DeleteMemory(int memoryId);
        Task<ApiResponse<string>> DeleteMemoryPhoto(int photoId);
        Task<ApiResponse<List<string>>> ExtractLinksFromMemoryStory(int memoryId);
        Task<byte[]> GetImage(int imageId);
        Task<ApiResponse<PaginatedList<MemoriesOutputDto>>> GetMemories(PaginationInputDto input);
        Task<ApiResponse<MemoryOutputDto>> GetMemory(int memoryId);
        Task<ApiResponse<List<MemoryTopFiveUsedWordsDto>>> GetTopFiveUsedWords();
        Task<ApiResponse<MemoryOutputDto>> UpdateMemory(MemoryUpdateInputDto input);
    }
}
