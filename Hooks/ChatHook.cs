using CrimsonLog.Systems;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace CrimsonLog.Hooks;

[HarmonyPatch]
public static class ChatMessageSystem_Patch
{
    [HarmonyPatch(typeof(ChatMessageSystem), nameof(ChatMessageSystem.OnUpdate))]
    [HarmonyPrefix]
    public static bool OnUpdate(ChatMessageSystem __instance)
    {
        if (__instance.__query_661171423_0 != null)
        {
            NativeArray<Entity> entities = __instance.__query_661171423_0.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                var fromData = __instance.EntityManager.GetComponentData<FromCharacter>(entity);
                var userData = __instance.EntityManager.GetComponentData<User>(fromData.User);
                var chatEventData = __instance.EntityManager.GetComponentData<ChatMessageEvent>(entity);
                
                if (chatEventData.ReceiverEntity.IsValid)
                {
                    Chat.Whisper(chatEventData, userData);
                }
            }
        }

        return true;
    }
}