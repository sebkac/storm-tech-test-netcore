using System.Linq;
using Todo.Data.Entities;
using Todo.EntityModelMappers.TodoItems;
using Todo.Models.TodoLists;

namespace Todo.EntityModelMappers.TodoLists
{
    public static class TodoListDetailViewmodelFactory
    {
        public static TodoListDetailViewmodel Create(TodoList todoList, bool rankDescending)
        {
            var items = todoList.Items.Select(TodoItemSummaryViewmodelFactory.Create).Where(i => i.IsDone == false);
            if (rankDescending)
            {
                items = items.OrderByDescending(i => i.Rank);
            }
            else
            {
                items = items.OrderBy(i => i.Rank);
             }
            var sortedItems = items.OrderBy(i => i.Importance).ToList();
            return new TodoListDetailViewmodel(todoList.TodoListId, todoList.Title, sortedItems);
        }
    }
}