using Realms;

namespace Xamrealm.Models
{
    public class Vote : RealmObject
    {
        //[MapTo("identity")]
        [Required]
        public string Identity { get; set; }
    }
}
