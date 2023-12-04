using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dungeon
{
    private GridAux grid;
    private Queue queue;
    private bool endPlaced;
    private RoomSpawner initSpawner;
    private List<RoomSpawner> dungeonList;
    private CameraSetUp cameraSetUp;
    private float cameraHeight;
    private int cooldownBig;


    public Dungeon(string name, RoomBuilder[] rooms, int Height, int Width, Vector2 CellSide, int tilesize, int endMinimumDistance)
    {
        int seed;
        cameraHeight = (CellSide.y * (tilesize / 16)) / 2;
        grid = new GridAux(name, Width, Height, CellSide, tilesize);
        initSpawner = grid.getCell(new Vector2((float)Math.Round(Width * .5f, MidpointRounding.AwayFromZero), (float)Math.Round(Height * .5f, MidpointRounding.AwayFromZero))).getSpawner();
        dungeonList = null;
        while(dungeonList == null || !endPlaced)
        {
            seed = UnityEngine.Random.Range(1, 1000000000); // 1;
            Debug.Log(seed);
            cooldownBig = 0;
            endPlaced = false;
            queue = new Queue(initSpawner);
            grid = new GridAux(grid);
            dungeonList = fillGrid(rooms, endMinimumDistance, seed);
        }
        grid.cleanGrid(dungeonList);
    }
    private CameraSetUp searchCamera()
    {
        var camera = GameObject.FindGameObjectWithTag("VirtualCamera");
        var cameraSetUp = camera.GetComponent<CameraSetUp>();
        if(cameraSetUp != null)
        {
            return cameraSetUp;
        }
        else
        {
            throw new Exception("Camara no encontrada");
        }
    }

    public void activateDungeon(Player player, AudioConf levelTheme)
    {
        cameraSetUp = searchCamera();
        grid.activateGrid();
        setPlayerInit(player);
        player.StartCoroutine(playAudio(player, levelTheme));
        player.StartCoroutine(spawnEntities(player));
        cameraSetUp.updateCameraSize(cameraHeight);
    }

    public void destroyDungeon(Player player)
    {
        grid.clearGrid();
        player.StopAllCoroutines();
    }

    private void setPlayerInit(Player player)
    {
        var initPos = initSpawner.getGridPosition();
        var playerPosition = grid.gridToLocal(initPos.x + .5f, initPos.y + .5f);
        player.setPosition(playerPosition);
        player.setLevelEnded(false);
    }

    private Vector2 getPlayerGridPos(Player player)
    {
        var playerPosition = player.getPosition();
        return grid.localToGrid(playerPosition.x, playerPosition.y);
    }

    public IEnumerator playAudio(Player player, AudioConf levelTheme)
    {
        Vector2 gridPlayerPos = new Vector2();
        AudioController audioController = GameObject.FindWithTag("Director").GetComponent<AudioController>();
        RoomBuilder roomBuilder;
        while (!player.reachedEnd())
        {
            if (getPlayerGridPos(player) == gridPlayerPos)
            {
                yield return null;
            }
            else
            {
                gridPlayerPos = getPlayerGridPos(player);
                var cell = grid.getCell(gridPlayerPos);
                if (cell != null)
                {
                    roomBuilder = cell.getSpawner().getRoom();
                    if(roomBuilder.getTheme().name != null)
                    {
                        if(audioController.getCurrentMusic() != roomBuilder.getTheme().name)
                            audioController.PlayAudio(roomBuilder.getTheme());
                    } 
                    else
                    {
                        if (audioController.getCurrentMusic() == levelTheme.name)
                        {
                            yield return null;
                        }
                        else
                        {
                            if (levelTheme != null)
                            {
                                audioController.PlayAudio(levelTheme);
                            }
                        }
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }
    }

    public IEnumerator spawnEntities(Player player)
    {
        Vector2 gridPlayerPos = new Vector2();
        RoomBuilder roomBuilder;
        while (!player.reachedEnd())
        {
            if (getPlayerGridPos(player) == gridPlayerPos)
            {
                yield return null;
            }
            else
            {
                var newCellSpawnStatus = grid.getCell(getPlayerGridPos(player)).getSpawner().getRoom().isSpawned();
                if (!newCellSpawnStatus)
                {
                    if (grid.getCell(gridPlayerPos) != null)
                        player.StartCoroutine(grid.getCell(gridPlayerPos).getSpawner().getRoom().manageEntities("despawn"));

                    gridPlayerPos = getPlayerGridPos(player);
                    var cell = grid.getCell(gridPlayerPos);
                    if (cell != null)
                    {
                        roomBuilder = cell.getSpawner().getRoom();
                        player.StartCoroutine(roomBuilder.manageEntities("spawn"));
                    }
                    else
                        yield return null;
                }
                else
                {
                    gridPlayerPos = getPlayerGridPos(player);
                    yield return null;
                }
            }
        }
    }

    private void processNeighbor(Mask actualMask, List<RoomSpawner> dungeon, int gridPositionX, int gridPositionY, int adjacentBorder, int actualBorderIndex)
    {
        var spawn = grid.getCell(new Vector2(gridPositionX, gridPositionY)).getSpawner();
        if (!dungeon.Contains(spawn) && spawn.getRoom() == null)
        {
            spawn.getMask().setBorder(adjacentBorder, actualMask.borders[actualBorderIndex]);
            if (actualMask.borders[actualBorderIndex] == 1 && !queue.contains(spawn))
                queue.enqueue(spawn);
        }
    }

    private bool vectorEquals(Vector2 vectorA, Vector2 vectorB)
    {
        if (vectorA[0] == vectorB[0] && vectorA[1] == vectorB[1])
            return true;
        else
            return false;
    }

    private bool isEndPlaceable(Vector2 position, int endMinimumDistance)
    {
        if (endPlaced)
            return false;
        else
        {
            var initPosition = initSpawner.getGridPosition();
            if (position[0] < initPosition[0] - endMinimumDistance || position[0] > initPosition[0] + endMinimumDistance)
                return true;
            else if (position[1] < initPosition[1] - endMinimumDistance || position[1] > initPosition[1] + endMinimumDistance)
                return true;
            else
                return false;
        }            
    }

    private void processNeighbors(Mask mask, List<RoomSpawner> dungeon, Vector2 gridSize, Vector2 gridPosition)
    {
        for (int i = 0; i < mask.borders.Length; i++)
        {
            switch (i)
            {
                case 0: //Top
                    if (gridPosition[1] < gridSize.y - 1)
                        if(mask.borders[i] != 2)
                            processNeighbor(mask, dungeon, (int)gridPosition[0], (int)gridPosition[1] + 1, 1, i);
                    continue;
                case 1: // Bottom
                    if (gridPosition[1] > 0)
                        if (mask.borders[i] != 2)

                            processNeighbor(mask, dungeon, (int)gridPosition[0], (int)gridPosition[1] - 1, 0, i);
                    continue;
                case 2: // Left
                    if (gridPosition[0] > 0)
                        if (mask.borders[i] != 2)

                            processNeighbor(mask, dungeon, (int)gridPosition[0] - 1, (int)gridPosition[1], 3, i);
                    continue;
                case 3: // Right
                    if (gridPosition[0] < gridSize.x - 1)
                        if (mask.borders[i] != 2)

                            processNeighbor(mask, dungeon, (int)gridPosition[0] + 1, (int)gridPosition[1], 2, i);
                    continue;
            }
        }
    }

    private List<RoomSpawner> fillGrid(RoomBuilder[] rooms, int endMinimumDistance, int seed)
    {
        List<RoomSpawner> dungeon = new List<RoomSpawner>();
        var gridSize = grid.getGridSize();
        while (!queue.isEmpty())
        {
            var first = queue.dequeue();
            var init = (cooldownBig == 0) ? initSpawner.getGridPosition() : new Vector2(-1, -1);
            var mask =  vectorEquals(first.getGridPosition(), initSpawner.getGridPosition()) ? first.ExpandByTag(rooms, grid, "Start", init, queue, seed) : 
                isEndPlaceable(first.getGridPosition(), endMinimumDistance) ? first.ExpandByTag(rooms, grid, "End", init, queue, seed) : 
                first.Expand(rooms, grid, init, queue, seed);
            if (mask.Item2 == null)
                return null;
            if (first.getRoom().getRoom().tag == "End")
                endPlaced = true;
            if (mask.Item2.Length == 1)
            {
                if (cooldownBig > 0)
                    cooldownBig--;
                dungeon.Add(first);
                var gridPosition = first.getGridPosition();
                processNeighbors(mask.Item2[0], dungeon, gridSize, gridPosition);
            }
            else
            {
                var gridUnits = (int) Math.Sqrt(mask.Item2.Length);
                var zone = grid.getZone(gridUnits, mask.Item1);
                if (zone.Length == mask.Item2.Length)
                {
                    for (int j = 0; j < mask.Item2.Length; j++)
                    {
                        dungeon.Add(grid.getCell(zone[j].getSpawner().getGridPosition()).getSpawner());
                        processNeighbors(mask.Item2[j], dungeon, gridSize, zone[j].getSpawner().getGridPosition());
                    }
                }
                cooldownBig = 20;
            }
        }
        return dungeon;
    }
}
