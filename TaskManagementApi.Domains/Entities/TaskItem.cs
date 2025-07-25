﻿using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Domains.Enums;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Domains.Entities
{
    /// <summary>
    /// Model for Task
    /// </summary>
    public class TaskItem
    {
        [Key]
        public Guid Id { get; set; }

        /*[DisplayName("Title")]*/
        [StringLength(120/*, ErrorMessage = "{0} Cannot exceed with 120 characters"*/)]
        public string Title { get; set; } = string.Empty;
        public Priority Priority { get; set; }
        public Status? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }



        // Foreign key and navigation to User
        public Guid UserId { get; set; }
        public TaskUsers User { get; set; }

        // Foreign key and navigation to Category
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    
       
    }

}
