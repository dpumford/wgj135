using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Orbit destinationQueue;
    public LaserState state;

    public AsteroidController minedBody = null;

    public float maxMiningDamagePerFrame = 1f;
    // mining damage is max / squaredDistance * damageDropoff,
    // so this number should be the square of the distance you want to be max damage
    public float damageDropoffModifier = 16f;

    LineRenderer line;
    BoxCollider2D myCollider;
    ConsumableController consumer;
    ParticleSystem particles;

    Vector3 originalScale;

    public Transform laserStart;
    public InterceptionPoint interceptionPoint;
    InterceptionPoint myPoint;

    public SpriteRenderer mousePointIcon;

    float baseEmissionRate;

    List<RaycastHit2D> results = new List<RaycastHit2D>();
    ContactFilter2D filter = new ContactFilter2D();

    void Start()
    {
        // we do this so the interception point will be working with world coordinates
        myPoint = Instantiate(interceptionPoint);
        myPoint.myLaser = this;

        state = LaserState.Free;
        line = GetComponentInChildren<LineRenderer>();

        myCollider = GetComponent<BoxCollider2D>();
        myCollider.enabled = false;
        originalScale = transform.localScale;

        consumer = GetComponentInParent<ConsumableController>();
        particles = GetComponentInChildren<ParticleSystem>();
        baseEmissionRate = particles.emission.rateOverTime.constant;
    }


    private void Update()
    {
        var mousePoint = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePointIcon.transform.position = mousePoint;

        var hitThing = false;

        if (state != LaserState.Aiming)
        {
            var direction = mousePoint - (Vector2)transform.position;
            Physics2D.Raycast(laserStart.position, direction.normalized, filter, results);

            foreach (var raycast in results)
            {
                if (raycast.collider != null && raycast.collider.tag != "Galaxy" && raycast.collider.tag != "Laser")
                {
                    hitThing = true;
                    myPoint.transform.position = raycast.collider.ClosestPoint(transform.position);
                    break;
                }
            }
        }

        if(!hitThing)
        {
            myPoint.transform.position = mousePoint;
        }
        particles.transform.position = myPoint.transform.position;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && state == LaserState.Mining)
        {
            if (minedBody != null)
            {
                if (minedBody.IsMinedOut())
                {
                    destinationQueue.AddOrbiter(minedBody);
                    GetComponentInParent<ShootController>().AddedOrbiter();
                }
                else
                {
                    var damage = maxMiningDamagePerFrame / (minedBody.gameObject.transform.position - transform.position).sqrMagnitude * damageDropoffModifier;

                    if (damage > maxMiningDamagePerFrame)
                    {
                        damage = maxMiningDamagePerFrame;
                    }

                    var particleVelocity = myPoint.transform.position - transform.position;

                    var vlt = particles.velocityOverLifetime;
                    vlt.x = -particleVelocity.magnitude;

                    var particleRate = damage / maxMiningDamagePerFrame * baseEmissionRate;
                    var pr = particles.emission.rateOverTime;
                    pr.constant = particleRate;

                    minedBody.DealMiningDamage(damage);
                }
            }
        }
        else if (Input.GetMouseButton(0) && state == LaserState.Free)
        {
            transform.localScale = originalScale + consumer.CurrentShipModifications.percentLazerRangeIncrease * originalScale;

            myPoint.reticleCollider.enabled = true;

            state = LaserState.Mining;
        }
        else if (Input.GetMouseButton(1))
        {
            state = LaserState.Aiming;
        }
        else
        {
            myPoint.reticleCollider.enabled = false;

            if (minedBody != null)
            {
                minedBody.StopMining();
                minedBody = null;
            }

            state = LaserState.Free;
        }

        particles.gameObject.SetActive(minedBody != null);
    }
}
