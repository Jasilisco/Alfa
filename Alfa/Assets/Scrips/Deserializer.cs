using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class Deserializer
{
    public RoomConf RoomDeserializer(string name) 
    {
        if (name != null && name != "")
        {
            TextAsset roomXml = Resources.Load<TextAsset>("XML/" + name);

            XmlSerializer deserializer = new XmlSerializer(typeof(RoomConf));
            StringReader stream = new StringReader(roomXml.ToString());

            var room = deserializer.Deserialize(stream) as RoomConf;

            stream.Close();
            return room;
        }
        else
        {
            throw new Exception("Ruta del XML de la habitación no especificada");
        }
    }

    public SpriteConf SpriteDeserializer(string name) 
    {
        if (name != null && name != "")
        {
            TextAsset spriteXml = Resources.Load<TextAsset>("XML/" + name);

            XmlSerializer deserializer = new XmlSerializer(typeof(SpriteConf));
            StringReader stream = new StringReader(spriteXml.ToString());

            var builder = deserializer.Deserialize(stream) as SpriteConf;

            stream.Close();
            return builder;
        }
        else
        {
            throw new Exception("Ruta del XML de los sprites no especificada");
        }
    }

    public GameConf GameConfDeserializer(string name)
    {
        if (name != null && name != "")
        {
            TextAsset gameConfXml = Resources.Load<TextAsset>("XML/" + name);

            XmlSerializer deserializer = new XmlSerializer(typeof(GameConf));
            StringReader stream = new StringReader(gameConfXml.ToString());

            var gameConf = deserializer.Deserialize(stream) as GameConf;

            stream.Close();
            return gameConf;
        }
        else
        {
            throw new Exception("Ruta del XML de la configuración del juego no especificada");
        }
    }

    public PrefabConf PrefabConfDeserializer(string name)
    {
        if (name != null && name != "")
        {
            TextAsset prefabConfXml = Resources.Load<TextAsset>("XML/" + name);

            XmlSerializer deserializer = new XmlSerializer(typeof(PrefabConf));
            StringReader stream = new StringReader(prefabConfXml.ToString());

            var prefabConf = deserializer.Deserialize(stream) as PrefabConf;

            stream.Close();
            return prefabConf;
        }
        else
        {
            throw new Exception("Ruta del XML del prefab no especificada");
        }
    }

    public ControlsConf ControlsDeserializer(string name)
    {
        if (name != null && name != "")
        {
            TextAsset prefabConfXml = Resources.Load<TextAsset>("XML/" + name);

            XmlSerializer deserializer = new XmlSerializer(typeof(ControlsConf));
            StringReader stream = new StringReader(prefabConfXml.ToString());

            var Controls = deserializer.Deserialize(stream) as ControlsConf;

            stream.Close();
            return Controls;
        }
        else
        {
            throw new Exception("Ruta del XML del prefab no especificada");
        }
    }
}
