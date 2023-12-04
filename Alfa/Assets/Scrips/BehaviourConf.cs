using UnityEngine;


public abstract class BehaviourConf
{
    public abstract void move(GameObject self, Vector2 moveDirection, float velocity);
    public abstract void idle(GameObject self);
    public abstract void npcBehave(GameObject self, GameObject target, StatBlock stats, bool dead);
    public abstract void takeDamage(GameObject self);
    public abstract void die(GameObject self);
    public abstract void attack(GameObject self, float strength);
}
