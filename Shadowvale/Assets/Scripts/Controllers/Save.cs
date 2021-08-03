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
                    Tile tile = Grid.GetTile(new Vector2Int(x, y));
                    gameData.tiles[ind++] = new TileData((int)tile.type, tile.corruptionVal, x, y);
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
                gameData.resources[i] = new ResourceData((int)resource.type, resource.val, (int)resource.transform.position.x, (int)resource.transform.position.y);
            }
        }
    }

    private void SaveBuildings(GameData gameData)
    {
        for (int i = 0; i < Buildings.buildings.Count; i++)
        {
            Building building = Buildings.buildings[i];
            int[] cost = new int[Resources.NUM], remaining = new int[Resources.NUM];
            if (building.construct != null)
            {
                cost = building.construct.cost;
                remaining = building.construct.remaining;
            }
            gameData.buildings[i] = new BuildingData(building.type, building.repair,
                (building is ResourceStorage) ? (building as ResourceStorage).currentStorage : 0, cost, remaining, building.tiles);
        }
    }

    private void SaveFollowers(GameData gameData)
    {
        for (int i = 0; i < Followers.followers.Count; i++)
        {
            Follower follower = Followers.followers[i];
            if (follower != null)
            {
                gameData.followers[i] = new AIData((int)follower.type, follower.health, follower.transform.position, follower.currentPos, follower.marker.transform.position, follower.statusEffects, follower.actions, 
                    (follower is Worker) ? (follower as Worker).inventory : null);
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
                gameData.enemies[i] = new AIData((int)enemy.type, enemy.health, enemy.transform.position, enemy.currentPos, Vector2.zero, enemy.statusEffects, enemy.actions);
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
            gameData.squads[i] = new SquadData(members);
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
}

[System.Serializable]
public class GameData
{
    public int mapSize, noise;
    public float camX, camY;
    public TileData[] tiles;
    public ResourceData[] resources;
    public BuildingData[] buildings;
    public AIData[] followers, enemies;
    public CreatureData[] creatures;
    public CorpseData[] corpses;
    public SquadData[] squads;
    public GameData(int size, int noiseVal, int numResources)
    {
        mapSize = size;
        noise = noiseVal;
        camX = GameController.Instance.gameCam.transform.position.x;
        camY = GameController.Instance.gameCam.transform.position.y;
        tiles = new TileData[size * size];
        resources = new ResourceData[numResources];
        buildings = new BuildingData[Buildings.buildings.Count];
        followers = new AIData[Followers.followers.Count];
        enemies = new AIData[Enemies.enemies.Count];
        creatures = new CreatureData[Creatures.creatures.Count];
        corpses = new CorpseData[Followers.corpses.Count];
        squads = new SquadData[Followers.squads.Count + Enemies.squads.Count];
    }
}

[System.Serializable]
public class TileData
{
    public int type;
    public float corruption;
    public int x, y;
    public TileData(int _type, float _corruption, int _x, int _y)
    {
        type = _type;
        corruption = _corruption;
        x = _x;
        y = _y;
    }
}

[System.Serializable]
public class ResourceData
{
    public int type, val, x, y;

    public ResourceData(int _type, int _val, int _x, int _y)
    {
        type = _type;
        val = _val;
        x = _x;
        y = _y;
    }
}

[System.Serializable]
public class BuildingData
{
    public int type, health, storage;
    public int[] resourceCost, resourceRemaining;
    public Vector2Int[] tiles;
    public BuildingData(int _type, int _health, int _storage, int[] _resourceCost, int[] _resourceRemaining, Vector2Int[] _tiles)
    {
        type = _type;
        health = _health;
        storage = _storage;
        resourceCost = _resourceCost;
        resourceRemaining = _resourceRemaining;

        tiles = _tiles;
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
    public AIData(int _type, int _health, 
        Vector2 _pos, Vector2Int _gridPos, Vector2 _markerPos, List<StatusEffect> _statusEffects, List<Action> _actions, Inventory _inventory = null, Cooldown[] _cooldowns = null)
    {
        type = _type;
        health = _health;
        pos = _pos;
        gridPos = _gridPos;
        inventory = _inventory;
        statusEffects = new StatusEffectData(_statusEffects);
        cooldowns = _cooldowns;
        markerPos = _markerPos;

        numActions = _actions.Count;
        targets = new int[numActions];
        states = new int[numActions];
        for (int i = 0; i < numActions; i++)
        {
            _actions[i].Save();
            targets[i] = _actions[i].targetInd;
            states[i] = _actions[i].state;
        }
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
    public SquadData(int[] _members)
    {
        members = _members;
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