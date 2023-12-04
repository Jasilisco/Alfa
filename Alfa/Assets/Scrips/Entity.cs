using System;
using UnityEngine;

public class Entity
{
    private StatBlock stats;
    private BehaviourConf behaviour;
    private GameObject entity;
    private bool dead;
    private float nextAttackSeconds;
    private Entity target;
    private int maxHealth;
    private healthBar healthBar;

    public GameObject getGOEntity()
    {
        if (entity != null)
            return entity;
        else return null;
    }

    public Entity(GameObject entity, Prefab entityConf, Entity target)
    {
        this.target = target;
        dead = false;
        this.entity = entity;
        this.entity.SetActive(false);
        stats = new StatBlock(entityConf.stats); 
        behaviour = chargeBehaviour(entityConf.behaviour);
    }

    public Entity(Player player, string behaviour, Stats stats, healthBar healthBar)
    {
        this.stats = new StatBlock(stats);
        maxHealth = (int)stats.hp;
        this.healthBar = healthBar;
        entity = player.gameObject;
        this.behaviour = chargeBehaviour(behaviour);
    }

    private BehaviourConf chargeBehaviour(string path)
    {
        Type scriptType = Type.GetType(path);

        if (scriptType != null)
        {

            BehaviourConf behaviour = Activator.CreateInstance(scriptType) as BehaviourConf;
            return behaviour;
        }
        else
        {
            throw new Exception("Behaviour not found");
        }
    }

    public void activateEntity(GameObject parent)
    {
        if (!dead)
        {
            if (entity != null)
            {
                entity.SetActive(true);
            }
            if (parent != null)
            {
                entity.transform.SetParent(parent.transform, false);
            }
        }
    }

    public void moveEntity(Vector2 moveDirection)
    {
        if(!dead)
            behaviour.move(entity, moveDirection, stats.getSpeed());
    }

    public void attack()
    {
        if (Time.time >= nextAttackSeconds)
        {
            behaviour.attack(entity, stats.getStrength());
            nextAttackSeconds = Time.time + stats.getCoolDown();
        }
    }

    public void idle()
    {
        if (!dead)
            behaviour.idle(entity);
    }

    public void npcBehave()
    {
        if (!dead)
        {
            var target = this.target.getGOEntity();
            if (target != null)
                behaviour.npcBehave(entity, target, stats, dead);
        }
    }

    public void takeDamage(float strength)
    {
        if (!dead)
        {
            if (stats.getHp() == maxHealth)
                healthBar.setMaxHealth(maxHealth);
            stats.takeDamage(strength);
            behaviour.takeDamage(entity);
            if (entity.tag == "Player")
                healthBar.UpdateHp((int)stats.getHp());
            if (stats.getHp() <= 0)
            {
                dead = true;
                behaviour.die(entity);
                if (entity.tag == "Player")
                    entity.GetComponent<Player>().setDead(true);
            }
        }
    }

    public void deactivateEntity(Vector2 scenePosition)
    {
        if (!dead && entity != null)
        {
            entity.SetActive(false);
            entity.transform.localPosition = new Vector3 (scenePosition.x, scenePosition.y, 0);
        }
    }
}
