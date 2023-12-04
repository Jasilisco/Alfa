using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public struct TilePrefab
{
    public string name; // Nombre de la tile(el que tiene en el editor cuando aplicas slice)
    public string code; // 1 dígito
    public int solid; // 1 o 0

    public TilePrefab(string name, string code, int solid)
    {
        this.name = name;
        this.code = code;
        this.solid = solid;
    }
}

public struct ObjectPrefab
{
    public string name; // Nombre de la tile(el que tiene en el editor cuando aplicas slice)
    public Vector2 size; // Tamaño en píxeles del objeto(se puede comprobar en el inspector)
    public string code; // 1 dígito
    public int solid; // 1 o 0

    public ObjectPrefab(string name, Vector2 size, string code, int solid)
    {
        this.name = name;
        this.size = size;
        this.code = code;
        this.solid = solid;
    }
}

[XmlRoot]
public class SpriteConf : Deserializer
{
    [XmlArray]
    [XmlArrayItem]
    public List<TilePrefab> tiles = new List<TilePrefab>();
    [XmlArray]
    [XmlArrayItem]
    public List<ObjectPrefab> objects = new List<ObjectPrefab>();
}