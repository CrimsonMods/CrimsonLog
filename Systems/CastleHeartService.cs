using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace CrimsonLog.Systems;

internal class CastleHeartService
{
    public static EntityQuery CaslteHeartQuery;
    public static EntityQuery ClanQuery;

    public CastleHeartService()
    {
        CaslteHeartQuery = Core.EntityManager.CreateEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] {
                ComponentType.ReadOnly<CastleHeart>(),
                ComponentType.ReadOnly<Team>(),
            },
        });

        ClanQuery = Core.EntityManager.CreateEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] {
                ComponentType.ReadOnly<ClanTeam>()
            },
        });
    }


    private static int CountTeamHearts(Entity entity)
    {
        var team = entity.Read<Team>();
        int i = 0;

        var heartEntities = CaslteHeartQuery.ToEntityArray(Allocator.Temp);
        foreach (var heartEntity in heartEntities)
        {
            var heartTeam = heartEntity.Read<Team>();
            if (team.Value.Equals(heartTeam.Value))
            {
                i++;
            }
        }

        heartEntities.Dispose();
        return i;
    }

    public static bool TryGetClanByID(NetworkId clandId, out Entity clan)
    {
        var clanEntities = CaslteHeartQuery.ToEntityArray(Allocator.Temp);
        foreach (var clanEntity in clanEntities)
        {
            var networkId = clanEntity.Read<NetworkId>();
            if (networkId.Equals(clandId))
            {
                clan = clanEntity;
                clanEntities.Dispose();
                return true;
            }
        }
        clanEntities.Dispose();
        clan = Entity.Null;
        return false;
    }

    public static bool TryGetCastleHeartByID(NetworkId castleId, out Entity castle)
    {
        var castleEntities = CaslteHeartQuery.ToEntityArray(Allocator.Temp);
        foreach (var castleEntity in castleEntities)
        {
            var networkId = castleEntity.Read<NetworkId>();
            if (networkId.Equals(castleId))
            {
                castle = castleEntity;
                castleEntities.Dispose();
                return true;
            }
        }
        castleEntities.Dispose();
        castle = Entity.Null;
        return false;   
    }
}
