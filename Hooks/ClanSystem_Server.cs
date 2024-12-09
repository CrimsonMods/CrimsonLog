using CrimsonLog.Systems;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Clan;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace CrimsonLog.Hooks;

[HarmonyPatch(typeof(ClanSystem_Server), nameof(ClanSystem_Server.OnUpdate))]
internal class ClanSystemServerPatch
{
    public static void Postfix(ClanSystem_Server __instance)
    {
        var queries = new[]
        {
            (__instance._ClanInviteResponseQuery, "Response"),
            (__instance._KickRequestQuery, "Kick"),
            (__instance._CreateClanEventQuery, "Create"),
            (__instance._LeaveClanEventQuery, "Leave"),
        };

        foreach (var (query, type) in queries)
        {
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                Log(entity, type);
            }
            entities.Dispose();
        }
    }

    private static void Log(Entity entity, string type)
    {
        var fromCharacter = entity.Read<FromCharacter>();
        var user = fromCharacter.User.Read<User>();

        switch (type)
        {
            case "Create":
                Logger.Record("Clans", "clan", $"{user.CharacterName} created a clan.");
                break;
            case "Leave":
                var leaveClan = entity.Read<ClanEvents_Client.LeaveClan>();
                if (CastleHeartService.TryGetClanByID(leaveClan.ClanId, out Entity clan))
                {
                    var clanOwner = clan.Read<UserOwner>();
                    if (clanOwner.Owner._Entity.Exists())
                    {
                        var clanOwnerUser = clanOwner.Owner._Entity.Read<User>();
                        Logger.Record("Clans", "clan", $"{user.CharacterName} left a clan owned by {clanOwnerUser.CharacterName}.");
                    }
                }
                break;
            case "Response":
                var inviteResponse = entity.Read<ClanEvents_Client.ClanInviteResponse>();
                if (inviteResponse.Response.Equals(InviteRequestResponse.Accept))
                {
                    if (CastleHeartService.TryGetClanByID(inviteResponse.ClanId, out Entity joinedClan))
                    {
                        var clanOwner = joinedClan.Read<UserOwner>();
                        if (clanOwner.Owner._Entity.Exists())
                        {
                            var clanOwnerUser = clanOwner.Owner._Entity.Read<User>();
                            Logger.Record("Clans", "clan", $"{user.CharacterName} joined a clan owned by {clanOwnerUser.CharacterName}.");
                        }
                    }
                }
                break;
            case "Kick":
                break;
        }
    }
}
