﻿using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using CrimsonLog.Structs;
using CrimsonLog.Systems;
using HarmonyLib;
using System.IO;
using System.Reflection;

namespace CrimsonLog;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.Bloodstone")]
public class Plugin : BasePlugin
{
    Harmony _harmony;

    internal static Plugin Instance { get; private set; }
    public static Settings Settings { get; private set; }
    public static Harmony Harmony => Instance._harmony;
    public static ManualLogSource LogInstance => Instance.Log;

    public static readonly string ConfigFFiles = Path.Combine(Paths.ConfigPath, MyPluginInfo.PLUGIN_NAME);

    public override void Load()
    {
        Instance = this;
        Settings = new Settings();
        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Settings.InitConfig();
        LogInstance.LogInfo($"{MyPluginInfo.PLUGIN_NAME} loaded");
    }

    public override bool Unload()
    {
        Config.Clear();
        _harmony?.UnpatchSelf();
        return true;
    }
}
