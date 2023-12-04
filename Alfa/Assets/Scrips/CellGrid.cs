using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    private GameObject Cell;
    private Vector2 cellSize;
    private RoomSpawner Spawner;
    public Vector3 Coords;

    public CellGrid(Vector3 position, Vector3 vertical, Vector3 horizontal, Vector2 cellSize, GameObject parent, int x, int y, int width, int height)
    {
        Coords = position;
        this.cellSize = cellSize;
        Cell = new GameObject("cell" + int.Parse(x.ToString() + y.ToString()));
        Cell.tag = "cell";
        Transform transform = Cell.transform;
        transform.SetParent(parent.transform, true);
        transform.localPosition = Coords;
        drawCube(position, vertical, horizontal);
        Spawner = placeSpawner(Cell.transform, this.cellSize, x, y, width, height);
    }

    public void destroyCell()
    {
        Object.Destroy(Cell.gameObject);
    }

    public RoomSpawner getSpawner()
    {
        return Spawner;
    }

    public void activateCell()
    {
        Spawner.activateRoom();
    } 

    private void drawCube(Vector3 position, Vector3 vertical, Vector3 horizontal)
    {
        Debug.DrawLine(position, vertical, Color.black, 100f);
        Debug.DrawLine(position, horizontal, Color.black, 100f);
    }

    private RoomSpawner placeSpawner(Transform parent, Vector2 cellSize, int x, int y, int width, int height)
    {
        return new RoomSpawnerBuilder(new int[2] { x, y }, cellSize, parent, new Vector2(width, height)).getSpawner();
    }
}
