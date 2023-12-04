using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public struct controlConf
{
    public string control; // Comando
    public string key; // KeyCode del control
}

public class ControlsConf : Deserializer
{
    [XmlArray]
    [XmlArrayItem]
    public List<controlConf> controls; // Tecla para atacar
}
