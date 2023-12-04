using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder
{
    private GameObject room;
    private bool spawned;
    public bool isSpawned()
    {
        return spawned;
    }
    public GameObject getRoom()
    {
        return room;
    }

    public Mask mask;
    public Mask getMask()
    {
        return mask;
    }

    private Mask[] masks;
    public Mask[] getMasks()
    {
        return masks;
    }

    public int gridUnits;
    public int getGridUnits()
    {
        return gridUnits;
    }

    private List<PrefabSpawner> spawners;
    public List<PrefabSpawner> getSpawners()
    {
        return spawners;
    }

    private AudioConf roomTheme;
    public AudioConf getTheme()
    {
        return roomTheme;
    }

    public RoomBuilder(GameObject roomPrefab, int gridUnits, List<PrefabSpawner> entities, string roomTheme)
    {
        spawned = false;
        room = roomPrefab;
        this.gridUnits = gridUnits;
        this.roomTheme = createRoomTheme(roomTheme);
        mask = new Mask();
        if(entities != null)
            spawners = entities;
        maskBuilder(gridUnits);
    }

    private AudioConf createRoomTheme(string roomTheme)
    {
        AudioClip audioClip = Resources.Load<AudioClip>("SoundSource/Themes/" + roomTheme);
        return new AudioConf(roomTheme, AudioResourceType.Music, audioClip);
    }

    public void destroyRoom()
    {
        UnityEngine.Object.Destroy(room.gameObject);
    }

    public IEnumerator manageEntities(string option)
    {
       if (spawners != null)
        {
            if (spawners.Count != 0)
            {
                if (option == "spawn" && !spawned)
                {
                    spawned = true;
                    GameObject child = room.transform.Find("Tilemaps").gameObject;
                    child = child.transform.GetChild(child.transform.childCount - 1).gameObject.transform.Find("Entities").gameObject;
                    for (int x = 0; x < spawners.Count; x++)
                    {
                        spawners[x].spawn(child);
                        yield return null;
                    }
                }
                else if (option == "despawn" && spawned)
                {
                    spawned = false;
                    for (int x = 0; x < spawners.Count; x++)
                    {
                        spawners[x].despawn();
                        yield return null;
                    }
                }
                else
                {
                    throw new Exception("Unknown Option(Use spawn or despawn for entities)");
                }
            }
        }
    }

    private void maskBuilder(int gridUnits)
    {
        GameObject child = room.transform.Find("Units").gameObject;
        if (gridUnits == 1)
            mask = maskGenerator(child.transform.GetChild(0).gameObject);
        else
        {
            var masks = new Mask[gridUnits];
            for(int i = 0; i < masks.Length; i++)
            {
                masks[i] = maskGenerator(child.transform.GetChild(i).gameObject);
            }
            this.masks = masks;
        }
    }

    private Mask maskGenerator(GameObject unit)
    {
        Mask mask = new Mask();
        var temp = new int[4] { 0, 0, 0, 0 };
        for (int j = 0; j < unit.transform.childCount; j++)
        {
            switch (unit.transform.GetChild(j).gameObject.tag)
            {
                case "T":
                    temp[0] = 1;
                    continue;
                case "B":
                    temp[1] = 1;
                    continue;
                case "L":
                    temp[2] = 1;
                    continue;
                case "R":
                    temp[3] = 1;
                    continue;
                case "InnerT":
                    temp[0] = 2;
                    continue;
                case "InnerB":
                    temp[1] = 2;
                    continue;
                case "InnerL":
                    temp[2] = 2;
                    continue;
                case "InnerR":
                    temp[3] = 2;
                    continue;
            }
        }
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] == 0)
                mask.setBorder(i, -1);
            else
                mask.setBorder(i, temp[i]);
        }
        return mask;
    }
}
