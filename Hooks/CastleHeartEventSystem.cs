using System;
using CrimsonLog.Systems;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Systems;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace CrimsonLog.Hooks;

[HarmonyPatch(typeof(CastleHeartEventSystem), nameof(CastleHeartEventSystem.OnUpdate))]
internal class CastleHeartEventSystemPatch
{
    public static void Prefix(CastleHeartEventSystem __instance)
    {
        NativeArray<Entity> entities = default;
        try
        {
            entities = __instance._CastleHeartInteractEventQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (!entity.Exists())
                    continue;

                if (!entity.Has<CastleHeartInteractEvent>() || !entity.Has<FromCharacter>())
                    continue;

                var heartEvent = entity.Read<CastleHeartInteractEvent>();
                var fromCharacter = entity.Read<FromCharacter>();

                if (!fromCharacter.User.Exists() || !fromCharacter.User.Has<User>())
                    continue;

                var user = fromCharacter.User.Read<User>();
                User castleOwner = user;

                if (CastleHeartService.TryGetCastleHeartByID(heartEvent.CastleHeart, out Entity castle))
                {
                    if (castle.Exists() && castle.Has<UserOwner>())
                    {
                        var userOwner = castle.Read<UserOwner>();
                        if (userOwner.Owner._Entity.Exists())
                        {
                            castleOwner = userOwner.Owner._Entity.Read<User>();
                        }
                    }
                }

                string appended = castleOwner == user ? "their own castle." : $"{user.CharacterName}'s castle.";
                switch (heartEvent.EventType)
                {
                    case CastleHeartInteractEventType.Upgrade:
                        Logger.Record("CastleHeart", "heart_events", $"{user.CharacterName} has upgraded {appended}\n");
                        break;
                    case CastleHeartInteractEventType.Abandon:
                        Logger.Record("CastleHeart", "heart_events", $"{user.CharacterName} has abandoned {appended}\n");
                        break;
                    case CastleHeartInteractEventType.Expose:
                        Logger.Record("CastleHeart", "heart_events", $"{user.CharacterName} has exposed {appended}\n");
                        break;
                    case CastleHeartInteractEventType.Claim:
                        Logger.Record("CastleHeart", "heart_events", $"{user.CharacterName} has claimed {appended}\n");
                        break;
                    case CastleHeartInteractEventType.Raid:
                        Logger.Record("CastleHeart", "heart_events", $"{user.CharacterName} has raided {appended}\n");
                        break;
                    case CastleHeartInteractEventType.Destroy:
                        Logger.Record("CastleHeart", "heart_events", $"{user.CharacterName} has destroyed {appended}\n");
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Core.LogException(e);
        }
        finally
        {
            entities.Dispose();
        }
    }
}