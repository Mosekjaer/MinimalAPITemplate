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
using MinimalAPITemplate.Features.Todos.Common;

namespace MinimalAPITemplate.Features.Todos
{
    public record CompleteTodoCommand(Guid TodoId) : IRequest<TodoResponse>;
    public class CompleteTodoCommandHandler : IRequestHandler<CompleteTodoCommand, TodoResponse>
    {
        private readonly ApplicationDbContext _dbContext;
        public CompleteTodoCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TodoResponse> Handle(CompleteTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await _dbContext.TodoItems.FindAsync(new object[] { request.TodoId }, cancellationToken) 
                ?? throw new Exception($"Todo with ID {request.TodoId} not found");

            todo.IsCompleted = true;
            todo.CompletedAt = DateTime.UtcNow;
            
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

    public class CompleteTodoModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/todos/{todoId}/complete", async (Guid todoId, IMediator mediator) =>
            {
                try
                {
                    var result = await mediator.Send(new CompleteTodoCommand(todoId));
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.NotFound(new { error = ex.Message });
                }
            })
            .WithName("CompleteTodo")
            .WithTags("Todos");
        }
    }
} 