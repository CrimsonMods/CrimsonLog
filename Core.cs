using BepInEx.Unity.IL2CPP.Utils.Collections;
using CrimsonLog.Systems;
using ProjectM;
using ProjectM.Physics;
using ProjectM.Scripting;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Entities;
using UnityEngine;

namespace CrimsonLog;

internal static class Core
{

    public static World Server { get; } = GetWorld("Server") ?? throw new System.Exception("There is no Server world (yet). Did you install a server mod on the client?");

    public static EntityManager EntityManager { get; } = Server.EntityManager;
    public static GameDataSystem GameDataSystem { get; } = Server.GetExistingSystemManaged<GameDataSystem>();
    public static PrefabCollectionSystem PrefabCollectionSystem { get; internal set; }
    public static RelicDestroySystem RelicDestroySystem { get; internal set; }
    public static ServerScriptMapper ServerScriptMapper { get; internal set; }
    public static ServerGameManager ServerGameManager => ServerScriptMapper.GetServerGameManager();
    public static CastleHeartService CastleHearts { get; internal set; }

    public static ServerGameSettingsSystem ServerGameSettingsSystem { get; internal set; }

    static MonoBehaviour monoBehaviour;

    public const int MAX_REPLY_LENGTH = 509;

    public static void LogException(System.Exception e, [CallerMemberName] string caller = null)
    {
        Plugin.LogInstance.LogError($"Failure in {caller}\nMessage: {e.Message} Inner:{e.InnerException?.Message}\n\nStack: {e.StackTrace}\nInner Stack: {e.InnerException?.StackTrace}");
    }

    internal static void InitializeAfterLoaded()
    {
        if (_hasInitialized) return;

        PrefabCollectionSystem = Server.GetExistingSystemManaged<PrefabCollectionSystem>();
        RelicDestroySystem = Server.GetExistingSystemManaged<RelicDestroySystem>();
        ServerGameSettingsSystem = Server.GetExistingSystemManaged<ServerGameSettingsSystem>();
        ServerScriptMapper = Server.GetExistingSystemManaged<ServerScriptMapper>();
        CastleHearts = new CastleHeartService();

        _hasInitialized = true;
        Plugin.LogInstance.LogInfo($"{nameof(InitializeAfterLoaded)} completed");
    }

    public static bool _hasInitialized = false;

    private static World GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name == name)
            {
                return world;
            }
        }

        return null;
    }

    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (monoBehaviour == null)
        {
            var go = new GameObject("CrimsonShards");
            monoBehaviour = go.AddComponent<IgnorePhysicsDebugSystem>();
            Object.DontDestroyOnLoad(go);
        }

        return monoBehaviour.StartCoroutine(routine.WrapToIl2Cpp());
    }

    public static void StopCoroutine(Coroutine coroutine)
    {
        if (monoBehaviour == null)
        {
            return;
        }

        monoBehaviour.StopCoroutine(coroutine);
    }
}
