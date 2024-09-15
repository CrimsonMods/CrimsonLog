using Bloodstone.Hooks;
using CrimsonLog.Structs;
using ProjectM;
using ProjectM.Network;
using System;
using System.IO;
using Unity.Entities;

namespace CrimsonLog.Systems;

public static class Chat
{
    private static readonly bool SingleFile = Settings.ChatSingleFile.Value;
    private static readonly bool LogGlobal = Settings.GlobalChat.Value;
    private static readonly bool LogWhisper = Settings.WhisperChat.Value;
    private static readonly bool LogRegion = Settings.RegionChat.Value;
    private static readonly bool LogLocal = Settings.LocalChat.Value;
    private static readonly bool LogTeam = Settings.TeamChat.Value;

    private static async void LogMessage(string chatType, string prefix, string message, string characterName, string additionalInfo = "")
    {
        string path = await Logger.CreateDirectory("Chat");

        string start = SingleFile ? $"{Logger.Time()} | {chatType}" : Logger.Time();
        string logEntry = $"{start} | {characterName}{additionalInfo} | {message}\n";

        string fileName = $"{path}/{(SingleFile ? "chat_" : prefix)}{DateTime.Now:dd-MM-yyyy}";

        if (!File.Exists(fileName))
        {
            File.Create(fileName).Dispose();
            Logger.DeleteOldLogs(path, prefix);
        }

        File.AppendAllText(fileName, logEntry);
    }

    public static void Global(VChatEvent message)
    {
        if (LogGlobal)
        {
            LogMessage("Global", "global_", message.Message.ToString(), message.User.CharacterName.ToString());
        }
    }

    public static void Region(VChatEvent message)
    {
        if (LogRegion)
        {
            LogMessage("Region", "region_", message.Message.ToString(), message.User.CharacterName.ToString());
        }
    }
    public static void Local(VChatEvent message)
    {
        if (LogLocal)
        {
            LogMessage("Local", "local_", message.Message.ToString(), message.User.CharacterName.ToString());
        }
    }

    public static void Team(VChatEvent message)
    {
        if (LogTeam)
        {
            string clanName = message.User.ClanEntity._Entity.Read<ClanTeam>().Name.Value;
            LogMessage("Clan", "clan_", message.Message.ToString(), message.User.CharacterName.ToString(), $" | {clanName}");
        }
    }

    public static void Whisper(ChatMessageEvent message, User fromUser)
    {
        if (LogWhisper)
        {
            string toUser = GetEntityFromNetworkId(message.ReceiverEntity).Read<User>().CharacterName.ToString();
            LogMessage("Whisper", "whisper_", message.MessageText.ToString(), fromUser.CharacterName.ToString(), $" -> {toUser}");
        }
    }

    public static Entity GetEntityFromNetworkId(NetworkId networkid)
    {
        var networkIdLookupEntity = ECSExtensions.GetEntitiesByComponentTypes<NetworkIdSystem.Singleton>(EntityQueryOptions.IncludeSystems)[0];
        var singleton = networkIdLookupEntity.Read<NetworkIdSystem.Singleton>();
        singleton.GetNetworkIdLookupRW().TryGetValue(networkid, out Entity entity);
        return entity;
    }
}
