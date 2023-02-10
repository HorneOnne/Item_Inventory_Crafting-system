using System.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(BoxCollider2D))]
public class ArrowProjectile_001 : Projectile, ICanCauseDamage
{
    [Header("References")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Transform explosionPosition;

    
    // Cached
    private BoxCollider2D boxCollider2D;
    private bool wasReturnToPool;
    private float timeToReturnElapse = 0.0f;
    private const float TIME_TO_RETURN = 5.0f;
    private const float TIME_TO_RETURN_WHEN_COLLIDE = 3.0f;
    private WaitForSeconds waitForReturnToPool;
    private BowData bowData;
    private ArrowData arrowData;



    protected override void Start()
    {
        base.Start();
        boxCollider2D = GetComponent<BoxCollider2D>();
        waitForReturnToPool = new WaitForSeconds(TIME_TO_RETURN_WHEN_COLLIDE);
    }



    public void Shoot(BowData bowData, ArrowData arrowData)
    {
        //StopAllCoroutines();
        wasReturnToPool = false;
        timeToReturnElapse = 0.0f;


        this.bowData = bowData;
        this.arrowData = arrowData;
        SetDust(arrowData.particle);

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = Quaternion.Euler(0, 0, OffsetZAngle) * transform.right * bowData.releaseSpeed;
    }


    private void Update()
    {
        timeToReturnElapse += Time.deltaTime;
        if (timeToReturnElapse > TIME_TO_RETURN)
        {
            ReturnToPool();
        }

        Vector2 direction = rb.velocity;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, -OffsetZAngle) * Quaternion.AngleAxis(angle, Vector3.forward);    
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        ArrowPropertiesWhenCollide();
        StartCoroutine(PerformReturnToPool());

        var explosionObject = Instantiate(explosionPrefab, explosionPosition.position, Quaternion.identity);
        Destroy(explosionObject, 0.5f);       
    }


    IEnumerator PerformReturnToPool()
    {
        yield return waitForReturnToPool;
        ReturnToPool();
    }

    
    private void ReturnToPool()
    {
        if (wasReturnToPool == true) return;
   
        ResetArrowProperties();
        ArrowSpawner.Instance.Pool.Release(this.gameObject);
        wasReturnToPool = true;
    }
    private void ResetArrowProperties()
    {
        rb.isKinematic = false;
        spriteRenderer.enabled = true;
        rb.velocity = Vector2.zero;
        boxCollider2D.enabled = true;
    }

    private void ArrowPropertiesWhenCollide()
    {
        rb.isKinematic = true;
        spriteRenderer.enabled = false;
        rb.velocity = Vector2.zero;
        boxCollider2D.enabled = false;
    }


    public int GetDamage()
    {
        return arrowData.damage + bowData.baseAttackDamage;
    }

    
}
