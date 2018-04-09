using System;
using System.Threading.Tasks;
using Realms;
using Realms.Sync;
using Xamrealm.Models;

namespace Xamrealm.Helpers
{
    public static class InvitationHelper
    {
        public static async Task<Invitation> CreateInviteAsync(string realmUrl)
        {
            var token = await User.Current.OfferPermissionsAsync(realmUrl, AccessLevel.Write, DateTimeOffset.UtcNow.AddDays(7));

            return new Invitation(token);
        }

        public static async Task<Realm> AcceptInvitationAsync(Invitation invitation)
        {
            var sharedRealmPath = await User.Current.AcceptPermissionOfferAsync(invitation.Token);

            var config = new SyncConfiguration(User.Current, new Uri(sharedRealmPath));

            return Realm.GetInstance(config);
        }
    }
}
