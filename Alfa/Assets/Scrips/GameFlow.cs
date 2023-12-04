using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameFlow
{
    private StateMachine sm;
    private List<State> states;
    private Dictionary<string, GameObject> UI;

    public GameFlow(GameConf gameConf, PrefabConf prefabConf, Canvas canvas)
    {
        UI = getUI(gameConf.UI, canvas);
        createPlayer(gameConf.playerConf, gameConf.controlsConf, buildHealthBar(canvas));
        states = buildStates(gameConf, prefabConf);
        createCamera();
        sm = new StateMachine(states.First().getName(), states);
    }

    private healthBar buildHealthBar(Canvas canvas)
    {
        var rectTransform = UI["HealthBar"].GetComponent<RectTransform>();
        Rect canvasRect = canvas.GetComponent<RectTransform>().rect;
        Vector2 position = new Vector2(-canvasRect.width / 5, canvasRect.height / 5);
        rectTransform.anchoredPosition = position;
        UI["HealthBar"].SetActive(true);
        return UI["HealthBar"].GetComponent<healthBar>();
    }

    private Dictionary<string, GameObject> getUI(List<UIElement> UI, Canvas canvas)
    {
        var dict = new Dictionary<string, GameObject>();
        foreach (UIElement element in UI)
        {
            var uiObject = Resources.Load<GameObject>("UI/Prefabs/" + element.path);
            var UIelement = Object.Instantiate(uiObject, canvas.transform);
            UIelement.SetActive(false);
            dict.Add(element.id, UIelement);
        }
        return dict;
    }

    public IEnumerator start()
    {
        State currentState = sm.startExecution();
        var load = UI["LoadScreen"].GetComponent<LoadScreen>();
        while (!sm.executionFinished())
        {
            while (sm.stateRunning())
            {
                yield return null;
            }
            if (!currentState.isEnd())
            {
                load.reloadLevel(sm.getState(currentState.getNext()));
                currentState = sm.advance(sm.getState(currentState.getNext()));
            }
            else
            {
                sm.endExecution();
                UI["VictoryScreen"].GetComponent<VictoryScreen>().display();
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().setRunFinished(true);
            }
        }
    }

    private Player createPlayer(PlayerConf playerConf, string controlsConfPath, healthBar healthBar)
    {
        GameObject playerGO = Resources.Load("Prefab/" + playerConf.playerPrefab, typeof(GameObject)) as GameObject;
        playerGO.name = "Player";
        playerGO.tag = "Player";
        playerGO = Object.Instantiate(playerGO, new Vector3(0, 0, 0), Quaternion.identity);
        Player player = playerGO.AddComponent<Player>();
        var controlsConf = new ControlsConf().ControlsDeserializer(controlsConfPath);
        player.setInputManager(new input(controlsConf, UI["PauseMenu"].GetComponent<PauseMenu>()));
        player.setDeathScreen(UI["DeathScreen"].GetComponent<DeathScreen>());
        player.setPlayerEntity(new Entity(player, playerConf.playerBehaviour, playerConf.stats, healthBar));
        return player;
    }

    private void createCamera()
    {
        GameObject camera = new GameObject("VirtualCamera");
        camera.tag = "VirtualCamera";
        camera.AddComponent<Cinemachine.CinemachineVirtualCamera>();
        var cameraSetUp = camera.AddComponent<CameraSetUp>();
        cameraSetUp.initializeCamera();
    }

    private List<State> buildStates(GameConf gameConf, PrefabConf prefabConf)
    {
        List<State> list = new List<State>();
        StateBuilder sb = new StateBuilder();
        LevelState ls;
        string next = "";
        for(int i = 0; i < gameConf.levels.Count; i++)
        {
            if (i + 1 < gameConf.levels.Count)
                next = gameConf.levels[i + 1].name;
            else
                next = "";
            ls = (LevelState) sb.getState("LevelState");
            list.Add(ls.buildState(gameConf.pools.Find(pool => pool.name == gameConf.levels[i].pool), gameConf.levels[i], prefabConf, next));
        }
        return list;
    }
}
