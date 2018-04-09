using System;

namespace Xamrealm.Models
{
    public class Invitation
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Token { get; }
        public Invitation(string token)
        {
            Token = token;
        }
    }
}
