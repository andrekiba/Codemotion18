using System;
using System.Collections.Generic;
using Realms;

namespace Xamrealm.Models
{
    public class Task : RealmObject, ICompletable
    {
        [PrimaryKey]
        //[MapTo("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        //[MapTo("title")]
        [Required]
        public string Title { get; set; } = string.Empty;

        //[MapTo("description")]
        public string Description { get; set; }

        //[MapTo("score")]
        public float Score { get; set; }

        //[MapTo("tags")]
        public string Tags { get; set; }

        //[MapTo("dueDate")]
        public DateTimeOffset DueDate { get; set; }

        //[MapTo("completed")]
        public bool IsCompleted { get; set; }

        //[MapTo("votes")]
        public IList<Vote> Votes { get; }
    }
}
