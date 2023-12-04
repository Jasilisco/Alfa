using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnerBuilder
{
    private RoomSpawner spawner;

    public RoomSpawnerBuilder(int[] gridPosition, Vector2 cellSize, Transform parent, Vector2 gridSize)
    {
        GameObject gO = new GameObject();
        spawner = gO.AddComponent<RoomSpawner>();
        spawner.setGridPosition(gridPosition);
        spawner.setGridSize(gridSize);
        spawner.setMask();
        gO.name = new string("Spawner " + int.Parse(gridPosition[0].ToString() + gridPosition[1].ToString()));
        Transform transform = gO.transform;
        transform.SetParent(parent, true);
        transform.localPosition = new Vector3(cellSize.x, cellSize.y) * .5f;
    }

    public RoomSpawner getSpawner()
    {
        return spawner;
    }
}
