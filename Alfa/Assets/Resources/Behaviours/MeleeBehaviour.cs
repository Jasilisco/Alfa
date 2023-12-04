using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : BehaviourConf
{
    private bool isAtacking = false;
    private bool flipped = false;
    private bool isTakingDamage = false;
    private Rigidbody2D rb;
    private AudioController audioController;
    private List<RaycastHit2D> collisions = new List<RaycastHit2D>();
    private float offset = .5f;
    private float nextAttackSeconds;

    private Animator getAnimator(GameObject gameObject)
    {
        var anim = gameObject.GetComponent<Animator>();
        if (anim != null)
            return anim;
        else
            throw new Exception("La entidad no tiene animator asignado");
    }

    private AudioConf getEffect(string effect)
    {
        AudioClip audioClip = Resources.Load<AudioClip>("SoundSource/Effects/" + effect);
        return new AudioConf(effect, AudioResourceType.Effect, audioClip);
    }

    private GameObject getChildByTag(GameObject parent, string tag)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).gameObject.tag == tag)
            {
                return parent.transform.GetChild(i).gameObject;
            }
        }
        throw new Exception("Rango de la entidad no delimitado");
    }

    public override void npcBehave(GameObject self, GameObject target, StatBlock stats, bool dead)
    {
        audioController = GameObject.FindGameObjectWithTag("Director").GetComponent<AudioController>();
        if (!dead) {
            if (!isAtacking && !isTakingDamage)
            {
                GameObject range = getChildByTag(self, "AttackRange");
                range.SetActive(true);
                Collider2D[] hitEntities = new Collider2D[50];
                int collisions = checkAttackRange(range, hitEntities);
                if (collisions == 0)
                {
                    Vector2 direction = target.transform.position - self.transform.position;
                    move(self, direction, stats.getSpeed());
                }
                else
                {
                    if (Time.time >= nextAttackSeconds)
                    {
                        attackWrapper(collisions, hitEntities, self, stats.getStrength());
                        nextAttackSeconds = Time.time + stats.getCoolDown();
                    }
                    else
                    {
                        Vector2 direction = target.transform.position - self.transform.position;
                        move(self, direction, stats.getSpeed());
                    }
                }
            }
        }
    }

    private int checkAttackRange(GameObject range, Collider2D[] results)
    {
        LayerMask layerMask = LayerMask.GetMask("Default");
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(layerMask);
        BoxCollider2D collider = range.GetComponent<BoxCollider2D>();
        int collisions = Physics2D.OverlapCollider(collider, contactFilter2D, results);
        range.SetActive(false);
        return collisions;
    }

    public IEnumerator attackRoutine(float animationLength)
    {
        yield return new WaitForSeconds(animationLength / 1.2f);
        isAtacking = false;
    }

    public override void attack(GameObject self, float strength)
    {
        isAtacking = true;
        var animator = getAnimator(self);
        if (self.tag != "Boss") 
        {
            animator.SetInteger("Animate", 1);
            audioController.PlayAudio(getEffect("sword1"));
            self.transform.GetComponent<EntityController>().StartCoroutine(attackRoutine(animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex("Base Layer")).Length));
        }
        else
        {
            var rand = UnityEngine.Random.Range(0, 1000);
            if(rand % 2 == 0)
                animator.SetInteger("Animate", 1);
            else
                animator.SetInteger("Animate", 5);
            audioController.PlayAudio(getEffect("sword3"));
            self.transform.GetComponent<EntityController>().StartCoroutine(attackRoutine(animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex("Base Layer")).Length));
        }
    }

    private void attackWrapper(int collisions, Collider2D[] hitEntities, GameObject self, float strength)
    {
        if (!isTakingDamage)
        {
            for (int i = 0; i < collisions; i++)
            {
                if (hitEntities[i].tag == "Player")
                    hitEntities[i].GetComponent<Player>().takeDamage(strength);
            }
            attack(self, strength);
        }

    }

    private IEnumerator dieRoutine(float animationLength, GameObject self)
    {
        yield return new WaitForSeconds(animationLength / 1.5f);
        UnityEngine.Object.Destroy(self);
    }

    public override void die(GameObject self)
    {
        if (self.tag == "Boss")
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().setLevelEnded(true);
        var animator = getAnimator(self);
        animator.SetInteger("Animate", 3);
        self.GetComponent<EntityController>().StartCoroutine(dieRoutine(animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex("Base Layer")).Length, self));
    }

    public override void idle(GameObject self)
    {
        if (!isAtacking && !isTakingDamage)
        {
            var animator = getAnimator(self);
            animator.SetInteger("Animate", 0);
        }
    }

    public override void move(GameObject self, Vector2 moveDirection, float velocity)
    {
        rb = self.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if (!isAtacking && !isTakingDamage)
            {
                var animator = getAnimator(self);
                animator.SetInteger("Animate", 4);
                LayerMask layerMask = LayerMask.GetMask("Foreground");
                ContactFilter2D layerFilter = new ContactFilter2D();
                layerFilter.SetLayerMask(layerMask);
                bool moveCheck = MoveAux(self, moveDirection, velocity, layerFilter);
                if (!moveCheck)
                {
                    moveCheck = MoveAux(self, new Vector2(moveDirection.x, 0), velocity, layerFilter);

                    if (!moveCheck)
                    {
                        MoveAux(self, new Vector2(0, moveDirection.y), velocity, layerFilter);
                    }
                }
            }
        }
        else throw new Exception("Rigidbody no añadido");
    }

    private bool MoveAux(GameObject gO, Vector2 moveDirection, float speedMovement, ContactFilter2D layerFilter)
    {
        var moveMagnitude = speedMovement * Time.fixedDeltaTime;
        int collisions = rb.Cast(moveDirection, layerFilter, this.collisions, moveMagnitude + offset);
        if (collisions == 0)
        {
            return executeMovement(gO, moveDirection, moveMagnitude);
        }
        else
        {
            return false;
        }

    }

    private bool executeMovement(GameObject gO, Vector2 moveDirection, float moveMagnitude)
    {
        Vector2 movement = moveDirection * moveMagnitude;

        rb.MovePosition(rb.position + movement);
        if (moveDirection.x < 0 && !flipped)
            flip(gO);
        if (moveDirection.x > 0 && flipped)
            flip(gO);
        return true;
    }

    private void flip(GameObject gO)
    {
        Vector3 actualScale = gO.transform.localScale;
        actualScale.x *= -1;
        gO.transform.localScale = actualScale;
        flipped = !flipped;
    }

    private IEnumerator takeDamageRoutine(float animationLength)
    {
        yield return new WaitForSeconds(animationLength / 2);
        isTakingDamage = false;
    }

    public override void takeDamage(GameObject self)
    {
        isTakingDamage = true;
        var animator = getAnimator(self);
        animator.SetInteger("Animate", 2);
        self.transform.GetComponent<EntityController>().StartCoroutine(takeDamageRoutine(animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex("Base Layer")).Length));
    }
}
