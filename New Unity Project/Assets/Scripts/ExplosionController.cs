using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float flashLifetime, smokeLifetime;
    private float lifeTimer;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer > flashLifetime)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, Random.Range(0, 360), 0);
        }

        if (lifeTimer > smokeLifetime)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
