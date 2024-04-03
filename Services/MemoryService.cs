using Rihal_Cinema.Data;
using Rihal_Cinema.Dtos.Movie;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Helpers;
using Rihal_Cinema.Services.Interfaces;
using Rihal_Cinema.Dtos.Memory;
using Rihal_Cinema.Models;
using Rihal_Cinema.Enums;
using Microsoft.EntityFrameworkCore;

namespace Rihal_Cinema.Services
{
    public class MemoryService : IMemoryService
    {
        private readonly DataContext _dataContext;

        public MemoryService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiResponse<MemoryOutputDto>> CreateMemory(int userId, MemoryInputDto input)
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

                var memoryOutput = new MemoryOutputDto
                {
                    Id = memory.Id,
                    MovieId = memory.MovieId,
                    Title = memory.Title,
                    TakenOn = memory.TakenOn,
                    Story = memory.Story,
                };

                return new ApiResponse<MemoryOutputDto>(true, (int)ResponseCodeEnum.Success, "Memory Added Successfully", memoryOutput);

            }
            catch (Exception ex)
            {
                return new ApiResponse<MemoryOutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while creating Memory", null);
            }
        }

        public async Task<ApiResponse<PaginatedList<MemoryOutputDto>>> GetMemories(int userId, PaginationInputDto input)
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
                    return new ApiResponse<PaginatedList<MemoryOutputDto>>(false, (int)ResponseCodeEnum.NotFound, "No Memory Found", null);
                }

                var memories = await memoriesQuery
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .Select(m => new MemoryOutputDto
                                     {
                                         Id = m.Id,
                                         MovieId = m.MovieId,
                                         Title = m.Title,
                                         TakenOn = m.TakenOn,
                                         Story = m.Story,
                                     })
                                     .ToListAsync();

                var pagintaedList = new PaginatedList<MemoryOutputDto>(memories, memoriesCount, page, pageSize);

                return new ApiResponse<PaginatedList<MemoryOutputDto>>(true, (int)ResponseCodeEnum.Success, "Memories Retrieved successfully", pagintaedList);

            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedList<MemoryOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while getting Memories", null);
            }
        }
    }
}
