using Realms;
using Xamarin.Forms;

namespace Xamrealm.Models
{
    public class Task : RealmObject, ICompletable
    {
        [MapTo("title")]
        [Required]
        public string Title { get; set; } = string.Empty;

        [MapTo("completed")]
        public bool IsCompleted { get; set; }
    }
}
