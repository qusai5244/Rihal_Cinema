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

namespace Rihal_Cinema.Services
{
    public class MemoryService : IMemoryService
    {
        private readonly DataContext _dataContext;

        public MemoryService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiResponse<MemoriesOutputDto>> CreateMemory(int userId, MemoryInputDto input)
        {
            try
            {
                var memory = new Memory
                {
                    UserId = userId,
                    MovieId = input.MovieId,
                    Title = input.Title,
                    TakenOn = input.TakenOn,
                    Story = input.Story,
                };

                _dataContext.Memories.Add(memory);
                await _dataContext.SaveChangesAsync();

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
            catch (Exception ex)
            {
                return new ApiResponse<MemoriesOutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while creating Memory", null);
            }
        }

        public async Task<ApiResponse<PaginatedList<MemoriesOutputDto>>> GetMemories(int userId, PaginationInputDto input)
        {
            try
            {
                var page = input.Page > 0 ? input.Page : 1;
                var pageSize = input.PageSize > 0 ? input.PageSize : 50;

                var memoriesQuery = _dataContext
                                     .Memories
                                     .AsNoTracking()
                                     .Where(m => m.UserId == userId);

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
                                         Title = m.Title,
                                         TakenOn = m.TakenOn,
                                         Story = m.Story,
                                     })
                                     .ToListAsync();

                var pagintaedList = new PaginatedList<MemoriesOutputDto>(memories, memoriesCount, page, pageSize);

                return new ApiResponse<PaginatedList<MemoriesOutputDto>>(true, (int)ResponseCodeEnum.Success, "Memories Retrieved successfully", pagintaedList);

            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedList<MemoriesOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while getting Memories", null);
            }
        }
        public async Task<ApiResponse<MemoryOutputDto>> GetMemory(int userId, int memoryId)
        {
            try
            {

                var memory = await _dataContext
                                  .Memories
                                  .Include(m => m.Movie)
                                  .AsNoTracking()
                                  .Where(m => m.Id == memoryId && m.UserId == userId)
                                  .FirstOrDefaultAsync();

                if (memory == null)
                {
                    return new ApiResponse<MemoryOutputDto>(false, (int)ResponseCodeEnum.NotFound, "Memory Not Found", null);
                }

                var memoryDto = new MemoryOutputDto
                {
                    Id = memoryId,
                    MovieId = memory.MovieId,
                    MovieName = memory.Movie.Name,
                    Story = memory.Story,
                    Title = memory.Title,
                };

                return new ApiResponse<MemoryOutputDto>(true, (int)ResponseCodeEnum.Success, "Memory Retrieved successfully", memoryDto);

            }
            catch (Exception ex)
            {
                return new ApiResponse<MemoryOutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while getting Memories", null);
            }
        }

        public async Task<ApiResponse<String>> UpdateMemory(int userId, MemoryUpdateInputDto input)
        {

            try
            {
                var memory = await _dataContext
                                 .Memories
                                 .Where(m => m.Id == input.MemoryId && m.UserId == userId)
                                 .FirstOrDefaultAsync();

                if (memory == null)
                {
                    return new ApiResponse<string>(false, (int)ResponseCodeEnum.NotFound, "Memory Not Found", null);
                }

                if(!string.IsNullOrEmpty(input.Title))
                {
                    memory.Title = input.Title;
                }

                if(string.IsNullOrEmpty(input.Story))
                {
                    memory.Story = input.Story;
                }

                await _dataContext.SaveChangesAsync();

                return new ApiResponse<string>(true, (int)ResponseCodeEnum.Success, "Memory Has Been Updated", null);

            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while Updating Memory", null);
            }

        }

        public async Task<ApiResponse<String>> DeleteMemory(int userId, int memoryId)
        {

            try
            {
                var memory = await _dataContext
                                 .Memories
                                 .Where(m => m.Id == memoryId && m.UserId == userId)
                                 .FirstOrDefaultAsync();

                if (memory == null)
                {
                    return new ApiResponse<string>(false, (int)ResponseCodeEnum.NotFound, "Memory Not Found", null);
                }

                _dataContext.Memories.Remove(memory);
                await _dataContext.SaveChangesAsync();

                return new ApiResponse<string>(true, (int)ResponseCodeEnum.Success, "Memory Has Been Deleted", null);

            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while Deleting Memory", null);
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
                var words = combinedStories.Split(new[] { ' ', ',', '.', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

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
            catch (Exception ex)
            {
                return new ApiResponse<List<MemoryTopFiveUsedWordsDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred While Getting Top Five Used Words in Memories", null);
            }
        }


    }
}

