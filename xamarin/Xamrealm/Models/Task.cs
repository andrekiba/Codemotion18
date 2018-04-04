using Realms;

namespace Xamrealm.Models
{
    public class Task : RealmObject, ICompletable
    {
        [MapTo("title")]
        [Required]
        public string Title { get; set; } = string.Empty;

        [MapTo("color")]
        public string Color { get; set; }

        [MapTo("completed")]
        public bool IsCompleted { get; set; }
    }
}
