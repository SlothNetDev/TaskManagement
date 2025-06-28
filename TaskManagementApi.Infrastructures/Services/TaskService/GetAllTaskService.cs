using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Infrastructures.Data;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.DTOs.TaskDto;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Infrastructures.Services.TaskService
{
    public class GetAllTaskService(IHttpContextAccessor _httpContextAccessor,
        TaskManagementDbContext _dbContext,
        ILogger<GetAllTaskService> _logger) : IGetAllTask
    {
        public async Task<ResponseType<List<TaskResponseDto>>> GetAllTaskAsync()
        {
            var response = new ResponseType<List<TaskResponseDto>>();

            if(await _dbContext.TaskDb.CountAsync() <= 0)
            {
                _logger.LogInformation($"Current Post {_dbContext.TaskDb.Count()}");
                response.Success = false;
                response.Message = "No Task Found Found, Create One First";
                return response;
            }

            try
            {
                var taskAsQuery =await  _dbContext.TaskDb
                    .OrderBy(t => t.CreatedAt)
                    .Select(t => new TaskResponseDto(t.Id, t.Title,
                    t.Priority.ToString(), t.Status.ToString() ?? string.Empty,
                    t.CreatedAt, t.DueDate, t.UpdatedAt))
                    .ToListAsync();

                response.Success = true;
                response.Data = taskAsQuery;
                response.Message = "Successfully Display all Blogs";
                return response;  
            }catch(Exception ex)
            {
                _logger.LogError(" Cannot Retrieve All Blogs, Reason: {error}",ex.Message);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
