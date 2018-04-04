using System;
using System.Collections.Generic;
using Realms;

namespace Xamrealm.Models
{
    public class TaskList : RealmObject, ICompletable
    {
        [PrimaryKey]
        [Required]
        [MapTo("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MapTo("title")]
        [Required]
        public string Title { get; set; } = string.Empty;

        [MapTo("completed")]
        public bool IsCompleted { get; set; }

        [MapTo("color")]
        public string Color { get; set; }

        [MapTo("tasks")]
        public IList<Task> Tasks { get; }
    }
}
