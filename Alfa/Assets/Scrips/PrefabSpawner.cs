using UnityEngine;

public class PrefabSpawner
{
    private EntityController entityControl;
    private Prefab entityConf;
    private Vector2 scenePosition;

    public Vector2 getscenePosition()
    {
        return scenePosition;
    }

    public PrefabSpawner(Vector2 gridPosition, Prefab entityConf, int tileSize)
    {
        this.entityConf = entityConf;
        scenePosition = new Vector2((gridPosition.x + 0.5f) * (tileSize / 16), (gridPosition.y + 0.5f) * (tileSize / 16));
        entityControl = createEntity();
    }

    private EntityController createEntity()
    {
        var target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().getPlayerEntity();
        var entityGO = Resources.Load("Prefab/" + entityConf.prefab, typeof(GameObject)) as GameObject;
        entityGO.name = entityConf.name;
        entityGO = Object.Instantiate(entityGO, new Vector3(scenePosition.x, scenePosition.y, 0), entityGO.transform.rotation);
        EntityController entityControl = entityGO.AddComponent<EntityController>();
        entityControl.setscenePosition(scenePosition);
        entityControl.setEntity(new Entity(entityGO, entityConf, target));
        return entityControl;
    }

    public void spawn(GameObject parent)
    {
        entityControl.activate(parent);
    }

    public void despawn()
    {
        entityControl.deactivate();
    }
}
