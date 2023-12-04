using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAux
{
    private int height, width;
    private Vector2 cellSize;
    private GameObject grid;
    private string name;
    private CellGrid[,] GridArr;

    public GridAux(string name, int width, int height, Vector2 cellSize, int tileSize){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize * (tileSize / 16);
        this.name = name;

        grid = new GameObject(this.name);
        grid.tag = "grid";
        GridArr = new CellGrid[this.width, this.height];

        for (int i = 0; i < GridArr.GetLength(0); i++){
            for (int j = 0; j < GridArr.GetLength(1); j++){
                GridArr[i, j] = new CellGrid(gridToLocal(i, j), gridToLocal(i, j + 1), gridToLocal(i + 1, j), cellSize, grid, i, j, width, height);
            }
        }
    }

    public GridAux(GridAux grid)
    {
        UnityEngine.Object.Destroy(grid.grid);

        width = grid.width;
        height = grid.height;
        cellSize = grid.cellSize;
        name = grid.name;

        this.grid = new GameObject(name);
        this.grid.tag = "grid";
        GridArr = new CellGrid[width, height];

        for (int i = 0; i < GridArr.GetLength(0); i++)
        {
            for (int j = 0; j < GridArr.GetLength(1); j++)
            {
                GridArr[i, j] = new CellGrid(gridToLocal(i, j), gridToLocal(i, j + 1), gridToLocal(i + 1, j), cellSize, this.grid, i, j, width, height);
            }
        }
    }

    public void clearGrid()
    {
        for(int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (GridArr[x, y] != null)
                {
                    GridArr[x, y].destroyCell();
                    GridArr[x, y] = null;
                }
            }
        }
        UnityEngine.Object.Destroy(grid);
    }

    public void activateGrid()
    {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (GridArr[x, y] != null)
                {
                    GridArr[x, y].activateCell();
                }
            }
        }
    }

    public void cleanGrid(List<RoomSpawner> dungeon)
    {
        for (int x = 0; x < width; x++ ) {
            for (int y = 0; y < height; y++) { 
                if (GridArr[x, y] != null)
                {
                    if (!dungeon.Contains(GridArr[x, y].getSpawner()))
                    {
                        GridArr[x, y].destroyCell();
                        GridArr[x, y] = null;
                    }
                }
            }
        }
    }

    public Vector2 getGridSize()
    {
        return new Vector2(width, height);
    }

    public GameObject getGridObject() 
    {
        return grid;
    }

    public CellGrid[] getZone(int gridUnits, Vector2 start)
    {
        List<CellGrid> list = new List<CellGrid>();
        if (gridUnits == 1)
            list.Add(getCell(start));
        else
        {
            for (int i = (int)start.y; i > ((int)start.y - gridUnits); i--)
                for (int j = (int)start.x; j < ((int)start.x + gridUnits); j++)
                {
                    if ((i < height && i >= 0) && (j < width && j >= 0))
                    {
                        var position = new Vector2(j, i);
                        list.Add(getCell(position));
                    }
                }
        }
        return list.ToArray();
    }

    public CellGrid getCell(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        if ((x < width && x >= 0) && (y < height && y >= 0))
            return GridArr[(int)position.x, (int)position.y];
        else
            throw new Exception("Celda fuera de los límites del Grid " + position);
    }

    public Vector3 gridToLocal(float x, float y)
    {
        return new Vector3(x * cellSize.x, y * cellSize.y);
    }

    public Vector3 localToGrid(float x, float y)
    {
        return new Vector3((float)Math.Floor(x / cellSize.x), (float)Math.Floor(y / cellSize.y));
    }

}
