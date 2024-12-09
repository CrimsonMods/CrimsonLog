using BepInEx;
using BepInEx.Configuration;
using System.IO;

namespace CrimsonLog.Structs;

public readonly struct Settings
{
    // base configs
    public static ConfigEntry<bool> ToggleLog { get; private set; }
    public static ConfigEntry<int> DaysToKeep { get; private set; }

    // loggers
    public static ConfigEntry<bool> ChatSingleFile { get; private set; }
    public static ConfigEntry<bool> GlobalChat { get; private set; }
    public static ConfigEntry<bool> WhisperChat { get; private set; }
    public static ConfigEntry<bool> RegionChat { get; private set; }
    public static ConfigEntry<bool> LocalChat { get; private set; }
    public static ConfigEntry<bool> TeamChat { get; private set; }

    public static ConfigEntry<bool> CastleHeart { get; private set; }

    public static ConfigEntry<bool> ClanMembership { get; private set; }

    public static void InitConfig()
    {
        ToggleLog = InitConfigEntry("_Config", "ToggleLog", true,
            "Enable or disable the mod overall");
        DaysToKeep = InitConfigEntry("_Config", "DaysToKeep", 5,
            "How many days (server time) to keep logs before automated delete. -1 to set to manually delete only.");

        // loggers
        ChatSingleFile = InitConfigEntry("Chat", "SingleFile", true,
            "If this is true, all chats will go to a single file; otherwise they will have their own files");
        GlobalChat = InitConfigEntry("Chat", "Global", true,
            "Log the global chat");
        WhisperChat = InitConfigEntry("Chat", "Whisper", false,
            "Log player whispers");
        RegionChat = InitConfigEntry("Chat", "Region", false,
            "Log region chat");
        LocalChat = InitConfigEntry("Chat", "Local", false,
            "Log local chat");
        TeamChat = InitConfigEntry("Chat", "Team", false,
            "Log team chat");

        CastleHeart = InitConfigEntry("CastleHeart", "CastleHeart", true,
            "Log castle heart activity");

        ClanMembership = InitConfigEntry("Clans", "Membership", true,
            "Log clan membership changes");
    }

    static ConfigEntry<T> InitConfigEntry<T>(string section, string key, T defaultValue, string description)
    {
        // Bind the configuration entry and get its value
        var entry = Plugin.Instance.Config.Bind(section, key, defaultValue, description);

        // Check if the key exists in the configuration file and retrieve its current value
        var newFile = Path.Combine(Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}.cfg");

        if (File.Exists(newFile))
        {
            var config = new ConfigFile(newFile, true);
            if (config.TryGetEntry(section, key, out ConfigEntry<T> existingEntry))
            {
                // If the entry exists, update the value to the existing value
                entry.Value = existingEntry.Value;
            }
        }
        return entry;
    }
}
