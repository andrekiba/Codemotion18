using System.Collections.Generic;
using Realms;

namespace Xamrealm.Models
{
    public class Task : RealmObject, ICompletable
    {
        [MapTo("title")]
        [Required]
        public string Title { get; set; } = string.Empty;

        [MapTo("completed")]
        public bool IsCompleted { get; set; }

        [MapTo("votes")]
        public IList<Vote> Votes { get; }
    }
}
