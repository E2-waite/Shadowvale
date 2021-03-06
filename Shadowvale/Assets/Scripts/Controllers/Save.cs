using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Save : MonoBehaviour
{
    public static string file = "SaveData";
    public void SaveGame()
    {
        GameData gameData = new GameData(Grid.size, Grid.noise, Resources.trees.Count + Resources.stones.Count);

        SaveTiles(gameData);
        SaveResources(gameData);
        SaveBuildings(gameData);
        SaveFollowers(gameData);
        SaveEnemies(gameData);
        SaveCreatures(gameData);
        SaveCorpses(gameData);
        SaveSquads(gameData);
        SaveProjectiles(gameData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + file + ".json", JsonUtility.ToJson(gameData));
    }

    private void SaveTiles(GameData gameData)
    {
        // Serialize tiles
        int ind = 0;
        for (int y = 0; y < Grid.size; y++)
        {
            for (int x = 0; x < Grid.size; x++)
            {
                if (Grid.tiles[x, y] != null)
                {
                    gameData.tiles[ind++] = new TileData(Grid.GetTile(new Vector2Int(x, y)));
                    //tilesString += JsonUtility.ToJson(data);
                }
            }
        }
    }

    private void SaveResources(GameData gameData)
    {
        // Serialize resources
        for (int i = 0; i < Resources.allResources.Count; i++)
        {
            Resource resource = Resources.allResources[i] as Resource;
            if (resource != null)
            {
                gameData.resources[i] = new ResourceData(resource);
            }
        }
    }

    private void SaveBuildings(GameData gameData)
    {
        for (int i = 0; i < Buildings.buildings.Count; i++)
        {
            Building building = Buildings.buildings[i];
            gameData.buildings[i] = new BuildingData(building);
        }
    }

    private void SaveFollowers(GameData gameData)
    {
        for (int i = 0; i < Followers.followers.Count; i++)
        {
            Follower follower = Followers.followers[i];
            if (follower != null)
            {
                gameData.followers[i] = new AIData(follower);
            }
            else
            {
                Followers.followers.RemoveAt(i);
            }
        }
    }

    private void SaveEnemies(GameData gameData)
    {
        for (int i = 0; i < Enemies.enemies.Count; i++)
        {
            Enemy enemy = Enemies.enemies[i];
            if (enemy != null)
            {
                gameData.enemies[i] = new AIData(enemy);
            }
            else
            {
                Enemies.enemies.RemoveAt(i);
            }
        }
    }

    private void SaveCreatures(GameData gameData)
    {
        for (int i = 0; i < Creatures.creatures.Count; i++)
        {
            Creature creature = Creatures.creatures[i] as Creature;
            if (creature != null)
            {
                gameData.creatures[i] = new CreatureData((int)creature.type, creature.health, creature.transform.position, creature.startPos);
            }
            else
            {
                Creatures.creatures.RemoveAt(i);
            }
        }
    }

    private void SaveCorpses(GameData gameData)
    {
        for (int i = 0; i < Followers.corpses.Count; i++)
        {
            Corpse corpse = Followers.corpses[i];
            if (corpse != null)
            {
                gameData.corpses[i] = new CorpseData((int)corpse.type, corpse.transform.position, corpse.transform.localScale.x <= 0);
            }
            else
            {
                Followers.corpses.RemoveAt(i);
            }
        }
    }

    private void SaveSquads(GameData gameData)
    {
        List<Squad> allSquads = new List<Squad>();
        allSquads.AddRange(Followers.squads);
        allSquads.AddRange(Enemies.squads);
        for (int i = 0; i < allSquads.Count; i++)
        {
            Squad squad = allSquads[i];
            int[] members = SquadMembersIndexes(squad);
            gameData.squads[i] = new SquadData(members, (squad is FollowerSquad));
        }
    }

    int[] SquadMembersIndexes(Squad squad)
    {
        int[] memberIndexes = new int[squad.members.Count];
        for (int i = 0; i < squad.members.Count; i++)
        {
            memberIndexes[i] = squad.members[i].Index();
        }
        return memberIndexes;
    }

    private void SaveProjectiles(GameData gameData)
    {
        for (int i = 0; i < Grid.projectiles.Count; i++)
        {
            Projectile projectile = Grid.projectiles[i];
            if (projectile != null)
            {
                gameData.projectiles[i] = new ProjectileData(projectile);
            }
            else
            {
                Grid.projectiles.RemoveAt(i);
            }
        }
    }
}

[System.Serializable]
public class GameData
{
    public int mapSize, noise, maxTrees, maxStones;
    public float camX, camY, camZoom;
    public TileData[] tiles;
    public ResourceData[] resources;
    public BuildingData[] buildings;
    public AIData[] followers, enemies;
    public CreatureData[] creatures;
    public CorpseData[] corpses;
    public SquadData[] squads;
    public ProjectileData[] projectiles;
    public GameData(int size, int noiseVal, int numResources)
    {
        mapSize = size;
        noise = noiseVal;
        camX = GameController.Instance.gameCam.transform.position.x;
        camY = GameController.Instance.gameCam.transform.position.y;
        camZoom = GameController.Instance.gameCam.orthographicSize;
        tiles = new TileData[size * size];
        resources = new ResourceData[numResources];
        buildings = new BuildingData[Buildings.buildings.Count];
        followers = new AIData[Followers.followers.Count];
        enemies = new AIData[Enemies.enemies.Count];
        creatures = new CreatureData[Creatures.creatures.Count];
        corpses = new CorpseData[Followers.corpses.Count];
        squads = new SquadData[Followers.squads.Count + Enemies.squads.Count];
        projectiles = new ProjectileData[Grid.projectiles.Count];
        maxTrees = Resources.maxTrees;
        maxStones = Resources.maxStones;
    }
}

[System.Serializable]
public class TileData
{
    public int type;
    public float corruption;
    public int x, y, multi;
    public int decor1 = -1, decor2 = -1, decor3 = -1, decor4 = -1;
    public TileData(Tile tile)
    {

        tile.Save(this);

    }
}

[System.Serializable]
public class ResourceData
{
    public int type, val, size;
    public Vector2Int pos;

    public ResourceData(Resource resource)
    {
        resource.Save(this);
    }
}

[System.Serializable]
public class BuildingData
{
    public int type, health, storage;
    public int[] resourceCost, resourceRemaining;
    public Vector2Int[] tiles;
    public int[] members;
    public Cooldown[] timers;
    public bool constructed = true;
    public BuildingData(Building building)
    {
        building.Save(this);
    }
}

[System.Serializable]
public class AIData
{
    public int type, health;
    public Vector2 pos, markerPos;
    public Vector2Int gridPos;
    public Inventory inventory;
    public StatusEffectData statusEffects = null;
    public Cooldown[] cooldowns;
    public int[] targets, states;
    public int numActions = 0;
    public AIData(Follower follower)
    {
        follower.Save(this);
    }
    public AIData(Enemy enemy)
    {
        enemy.Save(this);
    }
}

[System.Serializable]
public class CreatureData
{
    public int type, health;
    public Vector2 pos, startPos;
    public CreatureData(int _type, int _health, Vector2 _pos, Vector2  _startPos)
    {
        type = _type;
        health = _health;
        pos = _pos;
        startPos = _startPos;
    }
}

[System.Serializable]
public class SquadData
{
    public int[] members;
    public bool friendly;
    public SquadData(int[] _members, bool _friendly)
    {
        members = _members;
        friendly = _friendly;
    }
}

[System.Serializable]
public class StatusEffectData
{
    public int numEffects = 0;
    public ShadowEffect shadow = null;
    public HealEffect heal = null;

    public StatusEffectData(List<StatusEffect> effects)
    {
        numEffects = effects.Count;
        for (int i = 0; i < numEffects; i++)
        {
            effects[i].index = i;
            Assign(effects[i]);
        }
    }

    public List<StatusEffect> Read(Interaction target)
    {
        if (numEffects > 0)
        {
            StatusEffect[] effects = new StatusEffect[numEffects];
            if (shadow != null)
            {
                shadow.target = target;
                effects[shadow.index] = shadow;
            }
            if (heal != null)
            {
                heal.target = target;
                effects[heal.index] = heal;
            }
            return new List<StatusEffect>(effects);
        }
        return new List<StatusEffect>();
    }

    private void Assign(StatusEffect effect)
    {
        if (effect is ShadowEffect)
        {
            shadow = effect as ShadowEffect;
        }
        else if (effect is HealEffect)
        {
            heal = effect as HealEffect;
        }
    }
}

[System.Serializable]
public class CorpseData
{
    public int type;
    public Vector2 pos;
    public bool flipped;

    public CorpseData(int _type, Vector2 _pos, bool _flipped)
    {
        type = _type;
        pos = _pos;
        flipped = _flipped;
    }
}

[System.Serializable]
public class ProjectileData
{
    public int type, target, origin, damage;
    public float speed;
    public Vector2 pos;
    public ProjectileData(Projectile projectile)
    {
        type = (int)projectile.type;
        target = projectile.target.Index();
        if (projectile.origin != null)
        {
            origin = projectile.origin.Index();
        }
        else
        {
            origin = -1;
        }
        damage = projectile.damage;
        speed = projectile.speed;
        pos = projectile.transform.position;
    }
}