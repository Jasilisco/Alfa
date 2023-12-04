using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct Item
{
    public string code;
    public int solid;
    public Vector2 size;
    public Tile tile;

    public Item(string code, int solid, Vector2 size, Tile tile)
    {
        this.code = code;
        this.solid = solid;
        this.size = size;
        this.tile = tile;
    }
}

public class Pool
{
    private PoolConf biome;
    private SpriteConf tileset = new SpriteConf();
    private List<Item> objects = new List<Item>();
    private List<Item> tiles = new List<Item>();
    public RoomBuilder[] pool;

    public Pool(PoolConf biome, PrefabConf prefabConf)
    {
        this.biome = biome;
        tileset = tileset.SpriteDeserializer(this.biome.tileinfo);
        Sprite[] sprites = Resources.LoadAll<Sprite>("SpriteSheet/" + biome.tileset);
        tiles = getItems(sprites, true);
        objects = getItems(sprites, false);
        pool = buildPool(prefabConf);
    }

    private Item buildItem(Sprite[] spriteSource, string code, string name, int solid, Vector2 size = default(Vector2))
    {
        Tile aux = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        Item item = new Item();
        if (code != "0")
        {
            if (size == default(Vector2))
            {
                size = new Vector2(biome.tilesize, biome.tilesize);
            }
            aux.sprite = Array.Find(spriteSource, element => element.name == name);
            aux.name = name;
            item = new Item(code, solid, size, aux);
        }
        return item;
    }

    private List<Item> getItems(Sprite[] spriteSource, bool tile)
    {
        List<Item> result = new List<Item>();
        if (tile)
        {
            for (int i = 0; i < tileset.tiles.Count; i++)
                result.Add(buildItem(spriteSource, tileset.tiles[i].code, tileset.tiles[i].name, tileset.tiles[i].solid));
        }
        else
        {
            for (int i = 0; i < tileset.objects.Count; i++)
                result.Add(buildItem(spriteSource, tileset.objects[i].code, tileset.objects[i].name, tileset.objects[i].solid, tileset.objects[i].size));
        }
        return result;
    }

    public void destroyPool()
    {
        foreach(RoomBuilder room in pool)
        {
            room.destroyRoom();
        }
    }

    private RoomBuilder[] buildPool(PrefabConf prefabConf)
    {
        RoomBuilder[] pool = (biome.start != null && biome.start != "") ? new RoomBuilder[biome.rooms.Count + 2] : new RoomBuilder[biome.rooms.Count + 1];
        RoomConf roomConf = new RoomConf();
        for(int i = 0; i < pool.Length; i++)
        {
            roomConf = i == biome.rooms.Count + 1 ? roomConf.RoomDeserializer(biome.start) :
                i == biome.rooms.Count ? roomConf.RoomDeserializer(biome.end.prefab) :
                roomConf.RoomDeserializer(biome.rooms[i]);
            
            var room = i == biome.rooms.Count + 1 ? buildRoom(roomConf, "Start") :
                i == biome.rooms.Count ? buildRoom(roomConf, "End") :
                buildRoom(roomConf, "Room");  
            
            if (roomConf.entities.Count != 0)
            {
                pool[i] = new RoomBuilder(room, roomConf.gridUnits, buildSpawners(room, roomConf.entities, prefabConf), roomConf.theme);
            }
            else
            {
                pool[i] = new RoomBuilder(room, roomConf.gridUnits, null, roomConf.theme);
            }
        }
        return pool;
    }

    private GameObject calculateDoors(int gridUnits, List<Vector4> doorsList)
    {
        GameObject units = new GameObject("Units");
        if (doorsList.Count == 1)
        {
            GameObject unit = calculateUnit(doorsList[0], "Unit00");
            if (unit != null)
                unit.transform.SetParent(units.transform);
        }
        else
        {
            if (gridUnits == doorsList.Count) {
                for (int i = 0; i < gridUnits; i++)
                {
                    GameObject unit = calculateUnit(doorsList[i], "Unit" + i);
                    if (unit != null)
                        unit.transform.SetParent(units.transform);
                }
            }
        }
        if (units.transform.childCount == 0)
        {
            UnityEngine.Object.Destroy(units);
            return null;
        }
        else return units;
    }


    private void createDoor(GameObject parent, string tag)
    {
        GameObject door = new GameObject("Door");
        door.transform.SetParent(parent.transform);
        door.tag = tag;
    }

    private GameObject calculateUnit(Vector4 doors, string name)
    {
        GameObject parent = new GameObject(name);
        for(int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    if (doors[i] == 1)
                        createDoor(parent, "T");
                    else if (doors[i] == 2)
                        createDoor(parent, "InnerT");
                    continue;
                case 1:

                    if (doors[i] == 1)
                        createDoor(parent, "B");
                    else if (doors[i] == 2)
                        createDoor(parent, "InnerB");
                    continue;

                case 2:
                   if (doors[i] == 1)
                        createDoor(parent, "L");
                    else if (doors[i] == 2)
                        createDoor(parent, "InnerL");
                    continue;
                case 3:
                    if (doors[i] == 1)
                        createDoor(parent, "R");
                    else if (doors[i] == 2)
                        createDoor(parent, "InnerR");
                    continue;
            }
        }
        if (parent.transform.childCount == 0)
        {
            UnityEngine.Object.Destroy(parent);
            return null;
        }
        else return parent;
    }

    private List<List<string>> splitGameWorld(List<string> gameWorld)
    {
        List<List<string>> result = new List<List<string>>();
        foreach (string row in gameWorld)
        {
            List<string> caracteres = new List<string>();
            foreach (char code in row)
            {
                caracteres.Add(code.ToString());
            }
            result.Add(caracteres);
        }
        return result;
    }

    private Item getItemByCode(string code)
    {
        Item item = new Item();
        if ((item = tiles.Find(x => x.code == code)).code != null)
        {
            return item;
        }
        else if ((item = objects.Find(x => x.code == code)).code != null)
        {
            return item;
        }
        else throw new Exception("Unable to get item: Code " + code + " doesn't exist");
    }

    private GameObject buildRoom(RoomConf roomConf, string tag)
    {
        // Crear un nuevo objeto Grid en la escena
        GameObject grid = new GameObject("Room");
        grid.AddComponent<Grid>();
        grid.tag = tag;

        //Crear Objeto Rooms y situarlo(Contiene las referencias a las puertas)
        GameObject rooms = calculateDoors(roomConf.gridUnits, roomConf.doors);
        if (rooms == null)
            throw new Exception("Habitación sin puertas (código: " + roomConf.name + ")");

        rooms.transform.SetParent(grid.transform, false);
        rooms.transform.localPosition = new Vector3(roomConf.size.x / 2 * -1, roomConf.size.y / 2 * -1, 0f);

        //Crear el envoltorio de las Layers
        GameObject tilemaps = new GameObject("Tilemaps");
        tilemaps.transform.SetParent(grid.transform, false);

        //Recorrer los diferentes layers
        for (int j = 0; j < roomConf.gameWorld.Count; j++)
        {
            // Crear Tilemap
            Tilemap tilemap = new GameObject(j.ToString()).AddComponent<Tilemap>();
            tilemap.gameObject.AddComponent<TilemapRenderer>();
            tilemap.transform.SetParent(tilemaps.transform, false);
            tilemap.tag = "Tilemap";

            GameObject colliders = new GameObject("Colliders");
            colliders.transform.SetParent(tilemap.transform, false);

            //Transformar el gameWorld en lo que se necesita
            List<List<string>> gameWorld = splitGameWorld(roomConf.gameWorld[j]);
            for (int x = gameWorld.Count - 1; x >= 0; x--)
            {
                for (int y = 0; y < gameWorld[x].Count; y++)
                {
                    var code = gameWorld[gameWorld.Count - 1 - x][y];
                    if (code != "0")
                    {
                        Item item = getItemByCode(code);
                        
                        //Si el item es sólido hay que añadirle Rigidbody y collider
                        if (item.solid == 1)
                        {
                            GameObject collision = new GameObject(item.tile.name);

                            //Si es el final de nivel hay que etiquetarlo como End
                            if (tag == "End")
                            {
                                if (roomConf.levelEnd.position != null)
                                {
                                    if (j == roomConf.levelEnd.layer && x == roomConf.levelEnd.position.y && y == roomConf.levelEnd.position.x)
                                    {
                                        collision.tag = "End";
                                    }
                                    else
                                    {
                                        collision.tag = "Collider";
                                    }
                                }
                                else
                                {
                                    throw new Exception("La habitación asignada como final no tiene Objeto de Final de Nivel");
                                }
                            }
                            BoxCollider2D collider = collision.AddComponent<BoxCollider2D>();
                            Rigidbody2D rigidbody = collision.AddComponent<Rigidbody2D>();
                            rigidbody.bodyType = RigidbodyType2D.Static;
                            collider.offset = new Vector2(y + 0.5f * (biome.tilesize / 16), x + 0.5f * (biome.tilesize / 16));
                            collider.size = new Vector2(item.size.x / 16, item.size.y / 16);
                            collision.transform.SetParent(colliders.transform);

                            //Situar el objeto en su layer
                            int LayerForeground = LayerMask.NameToLayer("Foreground");
                            collision.layer = LayerForeground;
                        }
                        // Instanciar la tile
                        tilemap.SetTile(new Vector3Int(y * (biome.tilesize / 16), x * (biome.tilesize / 16), 0), item.tile);
                    }
                }
            }
            //Poner la capa oculta para ser activada cuando sea necesario
            tilemap.gameObject.SetActive(false);
        }
        //Situar los tilemaps
        if (roomConf.gridUnits == 1)
            tilemaps.transform.localPosition = new Vector3(roomConf.size.x / 2 * -1, roomConf.size.y / 2 * -1, 0f);
        else 
        {
            var gridUnits = (int)Math.Sqrt(roomConf.gridUnits);
            tilemaps.transform.localPosition = new Vector3(roomConf.size.x / gridUnits / 2 * -1, (-(roomConf.size.y / gridUnits) * (gridUnits - 1)) - ((roomConf.size.y / gridUnits) / 2), 0f); 
        }

        //Devolver el objeto
        return grid;
    }

    private List<PrefabSpawner> buildSpawners(GameObject room, List<EntitySpawner> spawnersConf, PrefabConf prefabConf)
    {
        var spawnersList = new List<PrefabSpawner>();
        var tilemapsWrapper = room.transform.Find("Tilemaps");
        var layerParent = tilemapsWrapper.GetChild(tilemapsWrapper.childCount - 1);

        var objectParent = new GameObject("Entities");
        objectParent.transform.SetParent(layerParent.transform, false);
        foreach (EntitySpawner spawner in spawnersConf)
        {
            var rand = UnityEngine.Random.Range(0, spawner.keys.Count);
            var key = spawner.keys[rand];
            Prefab entityConf = prefabConf.prefabs.Find(x => x.key == key);
            PrefabSpawner finalSpawner = new PrefabSpawner(spawner.position, entityConf, biome.tilesize);
            spawnersList.Add(finalSpawner);
        }
        return spawnersList;
    }
}
