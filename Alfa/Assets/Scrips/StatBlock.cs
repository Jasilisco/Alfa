public class StatBlock
{
    private float strength, defense, speed, hp, coolDown;

    public float getStrength()
    {
        return strength;
    }

    public float getDefense()
    {
        return defense;
    }

    public float getSpeed()
    {
        return speed;
    }

    public float getHp()
    {
        return hp;
    }

    public float getCoolDown()
    {
        return coolDown;
    }

    public void setStrength(float strength)
    {
        this.strength = strength;
    }

    public void setDefense(float defense)
    {
        this.defense = defense;
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    public StatBlock(Stats stats)
    {
        strength = stats.strength;
        defense = stats.defense;
        speed = stats.speed;
        hp = stats.hp;
        coolDown = stats.coolDown;
    }

    public StatBlock(int strength, int defense, int speed, int hp, int coolDown)
    {
        this.strength = strength;
        this.defense = defense;
        this.speed = speed;
        this.hp = hp;
        this.coolDown = coolDown;
    }

    public float takeDamage(float strength)
    {
        float damage = strength - defense;
        hp -= damage >= 0 ? damage : 0;
        return hp;
    }
}
