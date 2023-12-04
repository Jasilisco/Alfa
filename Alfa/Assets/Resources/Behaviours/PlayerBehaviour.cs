using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : BehaviourConf
{
    private bool isAtacking = false;
    private bool isTakingDamage = false;
    private bool flipped = false;
    private AudioController audioController = GameObject.FindGameObjectWithTag("Director").GetComponent<AudioController>();
    private Rigidbody2D rb;
    private List<RaycastHit2D> collisions = new List<RaycastHit2D>();
    private float offset = 0.2f;

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
        throw new Exception("Rango del personaje no delimitado");
    }

    private int checkAttackRange(GameObject range, Collider2D[] results)
    {
        LayerMask layerMask = LayerMask.GetMask("Enemies");
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(layerMask);
        BoxCollider2D collider = range.GetComponent<BoxCollider2D>();
        int collisions = Physics2D.OverlapCollider(collider, contactFilter2D, results);
        range.SetActive(false);
        return collisions;
    }

    public IEnumerator attackRoutine(GameObject self, float strength, float animationLength)
    {
        GameObject range = getChildByTag(self, "AttackRange");
        range.SetActive(true);
        Collider2D[] hitEntities = new Collider2D[50];
        int collisions = checkAttackRange(range, hitEntities);
        if (collisions != 0)
        {
            for (int i = 0; i < collisions; i++)
            {
                var entityTarget = hitEntities[i].GetComponent<EntityController>();
                if (entityTarget != null)
                    entityTarget.takeDamage(strength);
            }
        }
        yield return new WaitForSeconds(animationLength/2);
        isAtacking = false;
    }

    public override void attack(GameObject self, float strength)
    {
        if (!isTakingDamage)
        {
            var animator = getAnimator(self);
            animator.SetInteger("AnimatePlayer", 1);
            audioController.PlayAudio(getEffect("sword2"));
            isAtacking = true;
            self.transform.GetComponent<Player>().StartCoroutine(attackRoutine(self, strength, animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex("Base Layer")).Length));
        }
    }

    public override void npcBehave(GameObject self, GameObject target, StatBlock stats, bool dead)
    {
        throw new NotImplementedException();
    }

    private IEnumerator dieRoutine(float animationLength, GameObject self)
    {
        yield return new WaitForSeconds(animationLength / 2);
        UnityEngine.Object.Destroy(self);
    }

    public override void die(GameObject self)
    {
        var animator = getAnimator(self);
        animator.SetInteger("AnimatePlayer", 4);
        self.GetComponent<Player>().StartCoroutine(dieRoutine(animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex("Base Layer")).Length, self));
    }

    public override void idle(GameObject self)
    {
        if (!isAtacking && !isTakingDamage)
        {
            var animator = getAnimator(self);
            animator.SetInteger("AnimatePlayer", 0);
        }
    }


    private void flip(GameObject gO)
    {
        Vector3 actualScale = gO.transform.localScale;
        actualScale.x *= -1;
        gO.transform.localScale = actualScale;
        flipped = !flipped;
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
            foreach (RaycastHit2D collision in this.collisions)
            {

                switch (collision.rigidbody.gameObject.tag)
                {
                    case "End":
                        gO.transform.GetComponent<Player>().setLevelEnded(true);
                        continue;
                    default:
                        continue;
                }
            }
            return false;
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
                animator.SetInteger("AnimatePlayer", 2);
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

    private IEnumerator takeDamageRoutine(float animationLength)
    {
        yield return new WaitForSeconds(animationLength / 2);
        isTakingDamage = false;
    }

    public override void takeDamage(GameObject self)
    {
        isTakingDamage = true;
        var animator = getAnimator(self);
        animator.SetInteger("AnimatePlayer", 3);
        self.transform.GetComponent<Player>().StartCoroutine(takeDamageRoutine(animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex("Base Layer")).Length));
    }
}


