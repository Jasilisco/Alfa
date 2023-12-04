using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    private Pool pool;
    private Dungeon dungeon;
    private AudioConf levelTheme;

    public Level(string name, PoolConf pool, PrefabConf prefabConf, int Height, int Width, string levelTheme)
    {
        this.pool = new Pool(pool, prefabConf);
        this.levelTheme = createLevelTheme(levelTheme);
        dungeon = new Dungeon(name, this.pool.pool, Height, Width, pool.cellsize, pool.tilesize, pool.end.minimumDistance);
        clearPool();
    }

    public bool isReady()
    {
        if (dungeon != null)
            return true;
        else return false;
    }

    private AudioConf createLevelTheme(string levelTheme)
    {
        AudioClip audioClip = Resources.Load<AudioClip>("SoundSource/Themes/" + levelTheme);
        return new AudioConf(levelTheme, AudioResourceType.Music, audioClip);
    }

    public void activateLevel(Player player)
    {
        dungeon.activateDungeon(player, levelTheme);
    }

    public void destroyLevel(Player player)
    {
        dungeon.destroyDungeon(player);
    }

    private void clearPool()
    {
        pool.destroyPool();
    }

    public Pool getPool()
    {
        return pool;
    }
}
