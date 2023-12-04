
using System.Runtime.Serialization;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Linq;

public class Serializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        List<string> ground = new List<string> { "00100", "00100", "00100", "00200", "00200" };
        List<string> solid = new List<string>(ground);
        List<string> air = new List<string>(ground);
        
        List <List<string>> aha = new List<List<string>>
        {
            ground, solid, air
        };
        room primero  = new room();
        primero.nombre = "hola";
        primero.doors = new Vector4(1, 0, 0, 0);
        primero.size = new Vector2(16, 16);
        primero.gameWorld = aha;
        primero.coord = new List<Coordinate>();
        primero.coord.Add(new Coordinate("name", "tileset", "tileinfo", new List<string> { "rooms" }));

        List<List<string>> aha1 = new List<List<string>>
        {
            ground, solid, air
        };

        room segundo = new room();

        segundo.nombre = "hola2";
        segundo.doors = new Vector4(2, 1, 0, 0);
        segundo.size = new Vector2(17, 17);
        segundo.gameWorld = aha1;
        segundo.coord = new List<Coordinate>();
        segundo.coord.Add(new Coordinate("name", "tileset", "tileinfo", new List<string> { "rooms" }));

        tests test = new tests();
        test.test.Add(primero);
        test.test.Add(segundo);

        Serializador serializador= new Serializador();
        serializador.Serializar(test);
        
        tests emptybd = new tests();
        emptybd = emptybd.Deserializar();
        Debug.Log(emptybd.test[1].nombre);

        Debug.Log("!");
    }
}

public struct Coordinate
{
    public string name;
    public string tileset;
    public string tileinfo;
    public List<string> rooms;

    public Coordinate(string name, string tileset, string tileinfo, List<string> rooms)
    {
        this.name = name;
        this.tileinfo = tileinfo;
        this.tileset = tileset;
        this.rooms = rooms;
    }
}

[XmlRoot]
public class room {
    [XmlElement]
    public string nombre;
    [XmlElement]
    public Vector4 doors;
    [XmlArray]
    [XmlArrayItem]
    public List<List<string>> gameWorld;
    [XmlElement]
    public Vector2 size;
    [XmlArray]
    [XmlArrayItem]
    public List<Coordinate> coord;
    //public string[,] gameWorld;
}

[XmlRoot]
public class tests{
    [XmlArray]
    [XmlArrayItem]
    public List<room> test =  new List<room>();

    public tests Deserializar(){
        XmlSerializer deserializador = new XmlSerializer(typeof(tests));
        Stream stream = new FileStream(Application.dataPath + "/Resources/XML/pruebas.xml", FileMode.Open);

        var test = deserializador.Deserialize(stream) as tests;
        stream.Close();

        return test;
    }
}
public class Serializador{
    public void Serializar(tests test){
        XmlSerializer serializer = new XmlSerializer(typeof(tests));
        FileStream stream = new FileStream(Application.dataPath + "/Resources/XML/pruebas.xml", FileMode.Create);

        serializer.Serialize(stream, test);
        stream.Close();
    }
}