using System;
using System.Collections.Generic;
using Realms;

namespace Xamrealm.Models
{
    public class TaskList : RealmObject, ICompletable
    {
        [PrimaryKey]
        [MapTo("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MapTo("title")]
        [Required]
        public string Title { get; set; } = string.Empty;

        [MapTo("description")]
        public string Description { get; set; }

        [MapTo("dueDate")]
        public DateTimeOffset DueDate { get; set; }

        [MapTo("completed")]
        public bool IsCompleted { get; set; }

        [MapTo("tasks")]
        public IList<Task> Tasks { get; }
    }
}
