using System;
using System.Threading;
using System.Threading.Tasks;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MinimalAPITemplate.Data;
using MinimalAPITemplate.Entities;
using MinimalAPITemplate.Features.Todos.Common;

namespace MinimalAPITemplate.Features.Todos
{
    public record CreateTodoCommand
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }
    
    public record CreateTodoRequest(string Title, string Description) : IRequest<TodoResponse>;
    
    public class CreateTodoCommandHandler : IRequestHandler<CreateTodoRequest, TodoResponse>
    {
        private readonly ApplicationDbContext _dbContext;
        
        public CreateTodoCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<TodoResponse> Handle(CreateTodoRequest request, CancellationToken cancellationToken)
        {
            
            var todo = new TodoItem
            {
                Title = request.Title,
                Description = request.Description,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            
            _dbContext.TodoItems.Add(todo);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            var totalCount = await _dbContext.TodoItems.CountAsync(cancellationToken);
            
            return new TodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt,
                CompletedAt = todo.CompletedAt,
                TotalTodoCount = totalCount
            };
        }
    }
    
    public class CreateTodoModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/todos", async (CreateTodoCommand command, IMediator mediator) =>
            {
                var request = new CreateTodoRequest(command.Title, command.Description);
                
                var result = await mediator.Send(request);
                
                return Results.Created($"/api/todos/{result.Id}", result);
            })
            .WithName("CreateTodo")
            .WithTags("Todos");
        }
    }
} 