using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public struct End
{

    public string prefab; // Ruta del XML de la habitacion final
    public int minimumDistance; // Distancia m�nima de habitaciones que puede haber entre la salida y la entrada
}

public struct PoolConf
{
    public string name; // Nombre del bioma
    public string tileset; // Nombre del archivo tileset(sin extensi�n)
    public int tilesize;// Tama�o de los tiles(no objetos) Tama�os permitidos: M�ltiplos de 16
    public Vector2 cellsize;// Tama�o de las celdas del grid(tama�o obligatorio para las habitaciones que ocupen una celda, medido en unidades de tilesize x tilesize)
    public string tileinfo; // Nombre del xml con los c�digos del tileset(sin extensi�n)
    public string start; // Ruta del XML de la habitacion inicio
    public End end;
    public List<string> rooms; // Rutas de los XML de las habitaciones intermedias

    public PoolConf(string name, Vector2 cellsize, string tileset, int tilesize, string tileinfo, string start, End end, List<string> rooms)
    {
        this.name = name;
        this.tileset = tileset;
        this.tilesize = tilesize;
        this.cellsize = cellsize;
        this.tileinfo = tileinfo;
        this.start = start;
        this.end = end;
        this.rooms = rooms;
    }
}

public struct LevelConf
{
    public string name; //Nombre del nivel
    public int priority; // N�mero de nivel
    public Vector2 gridSize; // Tama�o del grid donde se colocar�n las habitaciones
    public string theme; // Ruta del recurso de audio del nivel
    public string pool; // Nombre exacto de la pool

    public LevelConf(string name, Vector2 gridSize, int priority, string theme, string pool)
    {
        this.name = name;
        this.priority = priority;
        this.gridSize = gridSize;
        this.theme = theme;
        this.pool = pool;
    }
}

public struct PlayerConf
{
    public string playerPrefab; // Ruta del prefab del player
    public string playerBehaviour; // Ruta del comportamiento del player 
    public Stats stats;
}

public struct UIElement
{
    public string id; //Nombre de la clase UI b�sica que implementa
    public string path; // Nombre del recurso en la carpeta Resources/XML/Prefabs
}

[XmlRoot]
public class GameConf : Deserializer
{
    [XmlElement]
    public string name; // Nombre del juego
    [XmlElement]
    public PlayerConf playerConf;
    [XmlElement]
    public string controlsConf; // Archivo que contiene los controles
    [XmlElement]
    public string prefabConf; // Archivo que contiene la informaci�n de los Prefabs
    [XmlArray]
    [XmlArrayItem]
    public List<UIElement> UI; // Elementos UI
    [XmlArray]
    [XmlArrayItem]
    public List<LevelConf> levels; // Configuraci�n de los niveles
    [XmlArray]
    [XmlArrayItem]
    public List<PoolConf> pools; // Datos de las pools de habitaciones
}
