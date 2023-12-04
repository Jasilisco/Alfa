
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState : State
{
    private string name;
    private PoolConf pool;
    private LevelConf levelConf;
    private PrefabConf prefabConf;
    private Player player;
    private Level level;
    private string nextLevel;
    private bool initialized = false;

    public LevelState buildState(PoolConf pool, LevelConf levelConf, PrefabConf prefabConf, string nextLevel)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        this.pool = pool;
        this.prefabConf = prefabConf;
        this.levelConf = levelConf;
        name = this.levelConf.name;
        this.nextLevel = nextLevel;
        initialized = true;
        return this;
    }

    public string getNext()
    {
        if (initialized)
            return nextLevel;
        else
            throw new Exception("LevelState not initialized");

    }

    public bool isRunning()
    {
        if (initialized)
            return !player.reachedEnd();
        else
            throw new Exception("LevelState not initialized");

    }

    public bool isEnd()
    {
        if (initialized)
        {
            if (nextLevel == "")
                return true;
            else return false;
        }
        else
            throw new Exception("LevelState not initialized");

    }

    public void activate()
    {
        if (initialized)
        {
            level = new Level(name, pool, prefabConf, (int)levelConf.gridSize.y, (int)levelConf.gridSize.x, levelConf.theme);
            level.activateLevel(player);
        }
        else
            throw new Exception("LevelState not initialized");
    }

    public void deactivate()
    {
        if (initialized)
            level.destroyLevel(player);
        else
            throw new Exception("LevelState not initialized");

    }

    public string getName()
    {
        if (initialized)
            return name;
        else
            throw new Exception("LevelState not initialized");

    }

    public bool isReady()
    {
        if (level != null)
            return level.isReady();
        else return false;
    }
}
