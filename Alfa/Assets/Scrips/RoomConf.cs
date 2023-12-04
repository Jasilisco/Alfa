using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public struct LevelEnd
{
    public Vector2 position;
    public int layer;
    
    public LevelEnd(Vector2 position, int layer)
    {
        this.position = position;
        this.layer = layer;
    }
}

public struct EntitySpawner
{
    public List<string> keys; // Posibles entidades a spawnear, la probabilidad es la misma para todos
    public Vector2 position; // Punto donde spawnear�n

    public EntitySpawner(List<string> keys, Vector2 position)
    {
        this.keys = keys;
        this.position = position;
    }
}

[XmlRoot]
public class RoomConf : Deserializer
{
    [XmlElement]
    public string name; // Nombre del nivel
    [XmlElement]
    public string theme; // Ruta del tema de la habitaci�n
    [XmlElement]
    public int probability; // Probabilidad de aparici�n
    [XmlElement]
    public int gridUnits; // N�mero de Unidades que ocupa en el grid
    [XmlElement]
    public Vector2 size; // Tama�o en p�xeles. Orden -> [Altura, Base]
    [XmlArray]
    [XmlArrayItem]
    public List<Vector4> doors; // Lista ordenada de arriba a abajo y izquierda a derecha
                                // Cada Vector representa una unidad de grid que forma la habitacion
                                // Contiene Vector4 de 1, 2 o 0. Orden -> [T, B, L, R]              
    [XmlElement]
    public LevelEnd levelEnd; // Coordenadas sobre el gameWorld del item que har� de salto de nivel(Solo cubrir si la habitaci�n debe tener salto de nivel)
    [XmlArray]
    [XmlArrayItem]
    public List<List<string>> gameWorld; // Almacena los niveles de profundidad de la habitaci�n
    [XmlArray]
    [XmlArrayItem]
    public List<EntitySpawner> entities; // Entidades(si las hay) y donde est�n colocados
}
