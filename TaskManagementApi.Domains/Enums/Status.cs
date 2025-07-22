using System.Runtime.Serialization;

namespace TaskManagementApi.Domains.Enums
{
    public enum Status
    {
        /// <summary>
        /// The task has been created but no work has started yet. It's in the backlog or queue.
        /// </summary>
        Open = 0, // Often used instead of ToDo, or as an initial state.
    
        /// <summary>
        /// Work on the task has actively begun.
        /// </summary>
        InProgress = 1,
        
        /// <summary>
        /// The Task is Done
        /// </summary>
        Done = 2,

        /// <summary>
        /// The task Being Cancelled or Delete
        /// </summary>
        Cancelled
    }
}
