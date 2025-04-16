using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MinimalAPITemplate.Data;
using MinimalAPITemplate.Features.Todos.Common;

namespace MinimalAPITemplate.Features.Todos
{
    public record GetTodosQuery : IRequest<List<TodoResponse>>;
    
    public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, List<TodoResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        
        public GetTodosQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<List<TodoResponse>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
        {
            var totalCount = await _dbContext.TodoItems.CountAsync(cancellationToken);
            
            var todos = await _dbContext.TodoItems
                .Select(todo => new TodoResponse
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    IsCompleted = todo.IsCompleted,
                    CreatedAt = todo.CreatedAt,
                    CompletedAt = todo.CompletedAt,
                    TotalTodoCount = totalCount
                })
                .ToListAsync(cancellationToken);
            
            return todos;
        }
    }
    
    public class GetTodosModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/todos", async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetTodosQuery());
                    return Results.Ok(result);
                })
                .WithName("GetTodos")
                .WithTags("Todos");
        }
    }
} 