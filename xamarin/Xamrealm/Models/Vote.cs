using Realms;

namespace Xamrealm.Models
{
    public class Vote : RealmObject
    {
        [Required]
        public string Identity { get; set; }
    }
}
