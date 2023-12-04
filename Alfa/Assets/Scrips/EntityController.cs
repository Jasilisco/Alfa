using UnityEngine;

public class EntityController : MonoBehaviour
{
    private Entity entity;
    private Vector2 scenePosition;
    private bool active = false;

    public void setscenePosition(Vector2 scenePosition)
    {
        this.scenePosition = scenePosition;
    }

    public void deactivate()
    {
        active = false;
        entity.deactivateEntity(scenePosition);
    }

    public void activate(GameObject parent)
    {
        active = true;
        entity.activateEntity(parent);
    }

    public void setEntity(Entity entity)
    {
        this.entity = entity;
    }

    private void Update()
    {
        if(active)
            entity.npcBehave();
    }

    public Entity getEntity()
    {
        return entity;
    }

    public void takeDamage(float strength)
    {
        entity.takeDamage(strength);
    }
}
