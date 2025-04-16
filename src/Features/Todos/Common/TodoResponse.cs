using System;

namespace MinimalAPITemplate.Features.Todos.Common
{
    public class TodoResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TotalTodoCount { get; set; }
    }
} 