using UnityEngine;

public class Player : MonoBehaviour
{
    private input inputManager;
    private Vector2 moveDirection;
    private bool levelCompleted;
    private Entity playerEntity;
    private DeathScreen deathScreen;
    private bool dead;
    private bool runFinished;
    public void setRunFinished(bool runFinished)
    {
        this.runFinished = runFinished;
    }

    public void setDead(bool dead)
    {
        this.dead = dead;
    }

    public void setLevelEnded(bool levelEnd)
    {
        levelCompleted = levelEnd;
    }

    public void setInputManager(input inputManager)
    {
        this.inputManager = inputManager;
    }

    public void setDeathScreen(DeathScreen deathScreen)
    {
        this.deathScreen = deathScreen;
    }

    public void setPlayerEntity(Entity playerEntity)
    {
        this.playerEntity = playerEntity;
    }

    public Entity getPlayerEntity()
    {
        return playerEntity;
    }

    private void Awake()
    {
        runFinished = false;
        dead = false;
        levelCompleted = false;
    }

    public void setPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public Vector3 getPosition()
    {
        return gameObject.transform.position;
    }

    void Update()
    {
        if (!runFinished)
        {
            // Input
            inputManager.processInput(this);
            if (dead)
                deathScreen.display();
        }

    }

    private void FixedUpdate()
    {
        // Physics
        if (moveDirection.x == 0 && moveDirection.y == 0)
            playerEntity.idle();
        else
            playerEntity.moveEntity(moveDirection);
    }

    public void attack()
    {
        playerEntity.attack();
    }

    public void takeDamage(float strength)
    {
        playerEntity.takeDamage(strength);
    }

    public void Move(Vector2 moveDirection)
    {
        this.moveDirection = moveDirection.normalized;
    }

    public bool reachedEnd()
    {
        if (levelCompleted)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
