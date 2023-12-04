using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class hola : MonoBehaviour
{

    // Sample terrain to be generated
    List<List<int>> gameWorld = new List<List<int>>
    {
        new List<int> { 0, 0, 1, 0, 0},
        new List<int> { 0, 0, 0, 0, 0},
        new List<int> { 0, 0, 0, 0, 1},
        new List<int> { 0, 2, 0, 0, 0},
        new List<int> { 0, 0, 0, 0, 0},
    };

    private GameObject CreateLevel()
    {
        Tile tempTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        Sprite[] list = Resources.LoadAll<Sprite>("SpriteSheet/16x16(1)");
        tempTile.sprite = Array.Find(list, element => element.name == "16x16(1)_294");
        tempTile.name = tempTile.sprite.name;
        Debug.Log(tempTile.name);
        // Crea un nuevo objeto Grid en la escena
        GameObject grid = new GameObject("Grid");
        grid.AddComponent<Grid>();
        // Crea un nuevo objeto Tilemap en la escena y enlázalo al objeto Grid
        Tilemap tilemap = new GameObject("Tilemap").AddComponent<Tilemap>();
        tilemap.gameObject.AddComponent<TilemapRenderer>();
        tilemap.transform.SetParent(grid.transform);

        for (int x = 0; x < gameWorld.Count; x++)
        {
            for (int y = 0; y < gameWorld[x].Count; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), (gameWorld[gameWorld[x].Count - 1 - y][x] == 1 ? tempTile : null));
                if (gameWorld[gameWorld[x].Count - 1 - y][x] == 2)
                {
                    GameObject gO = new GameObject("EntitySpawner");
                    gO.transform.SetParent(grid.transform, true);
                    gO.transform.localPosition = new Vector2(x + 0.5f, y + 0.5f);
                }
            }
        }
        return grid;
    }
    void Start()
    {
        GameObject grid = CreateLevel();
        
    }
}