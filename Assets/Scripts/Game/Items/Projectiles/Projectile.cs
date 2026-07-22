using UnityEngine;
using System;

public abstract class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] [Tooltip("Damage will be overwriten by item")] protected float damage;
    [SerializeField] public float speed = 5f;
    [SerializeField] protected float lifetime = 5f;
    [SerializeField] protected GameObject explosionVFX;
    [SerializeField] protected string projectileSound;

    // [SerializeField] protected GameObject explosionVFX;

    protected Vector2 direction = Vector2.right;

    public Action onDestroyed;

    public virtual void Init(float damage, Vector2? dir = null)
    {
        this.damage = damage;
        if (dir.HasValue)
            direction = dir.Value.normalized;

        AudioManager.Instance?.PlaySound(projectileSound);

        Destroy(gameObject, lifetime);
    }

    protected virtual void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) => HandleHit(collision);

    protected virtual void Explode()
    {
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    protected virtual void HandleHit(Collider2D collision) => Explode();

    public virtual void OnDestroy()
    {
        onDestroyed?.Invoke();
    }
}
