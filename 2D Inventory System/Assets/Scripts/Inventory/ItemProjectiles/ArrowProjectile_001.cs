using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(BoxCollider2D))]
public class ArrowProjectile_001 : Projectile, ICanCauseDamage
{
    [Header("References")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Transform explosionPosition;

    private BowData bowData;
    private ArrowData arrowData;


    private BoxCollider2D boxCollider2D;

    

    protected override void Start()
    {
        base.Start();

        boxCollider2D = GetComponent<BoxCollider2D>();

        this.arrowData = (ArrowData)ItemData;
        SetDust(arrowData.particle);
    }



    public void Shoot(BowData bowData, ArrowData arrowData)
    {
        this.bowData = bowData;
        this.arrowData = arrowData; 

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = Quaternion.Euler(0, 0, OffsetZAngle) * transform.right * bowData.releaseSpeed;
    }


    /*public void SetBowData(BowData bowData)
    {
        this.bowData = bowData;
    }*/

    private void Update()
    {
        Vector2 direction = rb.velocity;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, -OffsetZAngle) * Quaternion.AngleAxis(angle, Vector3.forward);

        Destroy(this.gameObject, 10f);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.isKinematic = true;
        spriteRenderer.enabled = false;    
        rb.velocity = Vector2.zero;
        boxCollider2D.enabled = false;

        var explosionObject = Instantiate(explosionPrefab, explosionPosition.position, Quaternion.identity);
        Destroy(explosionObject, 0.5f);
        Destroy(gameObject, 3.0f);
    }

  

    public int GetDamage()
    {
        return arrowData.damage + bowData.baseAttackDamage;
    }

    
}
