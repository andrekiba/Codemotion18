using System;
using System.Collections.Generic;
using Realms;

namespace Xamrealm.Models
{
    public class Task : RealmObject, ICompletable
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; }

        public float Score { get; set; }

        public string Tags { get; set; }

        public bool IsCompleted { get; set; }

        public IList<Vote> Votes { get; }
    }
}
