using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public struct Stats
{
    public float strength, defense, speed, hp, coolDown;
}
public struct Prefab
{
    public string name; // Nombre del prefab
    public string key; // Código del prefab(Es el que hará el match con las roomConf)
    public string type; // Tipos aceptados: NPC o OBJECT
    public string prefab; // Ruta del prefab(debe tener su animator)
    public string behaviour; // Nombre de la clase que hereda de BehaviourConf
    public Stats stats;
}

[XmlRoot]
public class PrefabConf : Deserializer
{
    [XmlArray]
    [XmlArrayItem]
    public List<Prefab> prefabs;
}
