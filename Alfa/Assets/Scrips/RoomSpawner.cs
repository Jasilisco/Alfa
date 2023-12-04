using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    private Mask mask;
    private int[] gridPosition;
    private int maxX;
    private int maxY;
    private RoomBuilder roomBuilder;

    public RoomBuilder getRoom()
    {
        return roomBuilder;
    }

    public void setRoom(RoomBuilder roomBuilder)
    {
        this.roomBuilder = roomBuilder;
    }

    public void setMask()
    {
        mask = new Mask();
        maskBuilder();
    }

    public void setMask(Mask mask)
    {
        this.mask = mask;
    }

    public void setGridSize(Vector2 gridSize)
    {
        maxX = (int)gridSize.x - 1;
        maxY = (int)gridSize.y - 1;
    }

    public void setGridPosition(int[] gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    private void maskBuilder()
    {
        if (gridPosition[0] == 0)
        {
            mask.setBorder(2, -1); 
        }
        else if (gridPosition[0] == maxX)
        {
            mask.setBorder(3, -1);
        }
        if (gridPosition[1] == 0)
        {
            mask.setBorder(1, -1);
        }
        else if (gridPosition[1] == maxY)
        {
            mask.setBorder(0, -1);
        }
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public Mask getMask()
    {
        return mask;
    }

    public Vector2 getGridPosition()
    {
        return new Vector2(gridPosition[0], gridPosition[1]);
    }



    private IEnumerator activateLayers()
    {
        var roomBuilder = this.roomBuilder;
        var tilemapsWrapper = roomBuilder.getRoom().transform.Find("Tilemaps");
        for (int i = 0; i < tilemapsWrapper.childCount; i++)
        {
            tilemapsWrapper.Find(i.ToString()).gameObject.SetActive(true);
            yield return null;
        }
    }

    public void activateRoom()
    {
        StartCoroutine(activateLayers());
    }

    private (Vector2, Mask[]) placeRoom(Vector2 start, GridAux grid, RoomBuilder roomBuilder)
    {
        if (start.x == -1 || start.y == -1)
        {
            mask = roomBuilder.mask;
            var room = Instantiate(roomBuilder.getRoom(), gameObject.transform.position, roomBuilder.getRoom().transform.rotation);
            room.transform.SetParent(transform.parent);
            this.roomBuilder = new RoomBuilder(room, roomBuilder.getGridUnits(), roomBuilder.getSpawners(), roomBuilder.getTheme().name);
            Mask[] result = { mask };
            return (new Vector2(-1, -1), result);
        }
        else
        {
            var zone = grid.getZone((int)Math.Sqrt(roomBuilder.getGridUnits()), start);
            var room = Instantiate(roomBuilder.getRoom(), grid.getCell(start).getSpawner().getPosition(), Quaternion.identity);
            room.transform.SetParent(transform.parent);
            this.roomBuilder = new RoomBuilder(room, roomBuilder.getGridUnits(), roomBuilder.getSpawners(), roomBuilder.getTheme().name);
            if (zone.Length == this.roomBuilder.getMasks().Length)
            {
                var masks = this.roomBuilder.getMasks();
                for (int i = 0; i < zone.Length; i++)
                {
                    if (zone[i].getSpawner().getGridPosition().x == gridPosition[0] && zone[i].getSpawner().getGridPosition().y == gridPosition[1])
                        mask = masks[i];
                    else
                    {
                        grid.getCell(zone[i].getSpawner().getGridPosition()).getSpawner().setRoom(this.roomBuilder);
                        grid.getCell(zone[i].getSpawner().getGridPosition()).getSpawner().setMask(masks[i]);
                    }
                }
                return (start, masks);
            }
            else return (new Vector2(-1, -1), null);
        }
    }

    public (Vector2, Mask[]) Expand(RoomBuilder[] rooms, GridAux grid, Vector2 init, Queue spawners, int seed)
    {
        var candidates = getCandidates(rooms, grid, init, spawners);
        if(candidates.Length == 0)
            return (new Vector2(-1, -1), null);
        var roomIndex = seed % UnityEngine.Random.Range(1, candidates.Length); // candidates.Length;
        return placeRoom(candidates[roomIndex].Item1, grid, candidates[roomIndex].Item2);
    }

    public (Vector2, Mask[]) ExpandByTag(RoomBuilder[] rooms, GridAux grid, string tag, Vector2 init, Queue spawners, int seed)
    {
        var startRoomBuilder = getRoomByTag(rooms, grid, tag, init, spawners);
        if (startRoomBuilder.Item2 == null)
            return Expand(rooms, grid, init, spawners, seed);
        else
            return placeRoom(startRoomBuilder.Item1, grid, startRoomBuilder.Item2);
    }

    private (Vector2, RoomBuilder) getRoomByTag(RoomBuilder[] rooms, GridAux grid, string tag, Vector2 init, Queue spawners)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].getRoom().tag == tag)
            {
                var candidate = checkCandidate(rooms[i], grid, init, spawners);
                if (candidate.Item2 != null)
                    return candidate;
            }
        }
        return (new Vector2(-1, -1), null);
    }

    private Mask[] cellToMask(CellGrid[] cells)
    {
        List<Mask> masks = new List<Mask>();
        foreach(CellGrid cell in cells)
        {
            var mask = cell.getSpawner().getMask();
            masks.Add(mask);
        }
        return masks.ToArray();
    }

    private (Vector2, RoomBuilder)[] getCandidates(RoomBuilder[] rooms, GridAux grid, Vector2 init, Queue spawners)
    {
        var result = new List<(Vector2, RoomBuilder)>();
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].getRoom().tag == "Room")
            {
                var candidate = checkCandidate(rooms[i], grid, init, spawners);
                if (candidate.Item2 != null)
                    result.Add(candidate);
            }
        }
        return result.ToArray();
    }

    private bool isAvaliable(CellGrid[] zone)
    {
        var temp = true;
        for(int i = 0; i < zone.Length; i++)
        {
            if (zone[i].getSpawner().getRoom() != null)
                temp = false;
        }
        return temp;
    }

    private bool contained(CellGrid[] zone, Queue spawners, GridAux grid)
    {
        var temp = false;
        for (int i = 0; i < zone.Length; i++)
        {
            if (spawners.contains(grid.getCell(zone[i].getSpawner().getGridPosition()).getSpawner()))
                temp = true;
        }
        return temp;
    }

    private (Vector2, RoomBuilder) checkCandidate(RoomBuilder room, GridAux grid, Vector2 init, Queue spawners)
    {
        var gridUnits = (int) Math.Sqrt(room.getGridUnits());
        if (gridUnits == 1)
        {
            if (Mask.Fits(mask, room.mask))
                return (new Vector2(-1, -1), room);
            else return (new Vector2(-1, -1), null);
        }
        else
        {
            if ((init.x != -1 && init.y != -1) &&
                (gridPosition[0] + gridUnits < init.x || gridPosition[0] - gridUnits > init.x || 
                gridPosition[1] + gridUnits < init.y || gridPosition[1] - gridUnits > init.y))
            {
                for (int j = gridPosition[1] + (gridUnits - 1); j >= gridPosition[1]; j--)
                    for (int m = gridPosition[0] - (gridUnits - 1); m <= gridPosition[0]; m++)
                    {
                        var zone = grid.getZone(gridUnits, new Vector2(m, j));
                        if (zone.Length == Math.Pow(gridUnits, 2) && isAvaliable(zone))
                            if (Mask.Fits(cellToMask(zone), room.getMasks()) && !contained(zone, spawners, grid))
                                return (new Vector2(m, j), room);
                    }
                return (new Vector2(-1, -1), null);
            }
            else return (new Vector2(-1, -1), null);
        }
    }
}
