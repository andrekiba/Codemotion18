using Realms;

namespace Xamrealm.Models
{
    public class LoginInfo : RealmObject
    {
        public string ServerUrl { get; set; }

        public string Username { get; set; }
    }
}
