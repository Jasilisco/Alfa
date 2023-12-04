using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Game Configuration")]
    public string GameSheet;
    private Canvas canvas;
    private GameFlow gameFlow;
    private GameConf gameConf = new GameConf();
    private PrefabConf prefabConf = new PrefabConf();

    private void Awake()
    {
        gameConf = gameConf.GameConfDeserializer(GameSheet);
        prefabConf = prefabConf.PrefabConfDeserializer(gameConf.prefabConf);
        canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            gameFlow = new GameFlow(gameConf, prefabConf, canvas);
            StartCoroutine(gameFlow.start());
        }
    }
}
