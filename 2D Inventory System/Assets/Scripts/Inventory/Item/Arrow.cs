using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Arrow : Item
{
    [Header("References")]
    [SerializeField] private GameObject explosionPrefab;
    Rigidbody2D rb;


    public override void Use(Player player)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();


        rb.velocity = Quaternion.Euler(0, 0, OffsetZAngle) * transform.right * 50;


    }


    private void Update()
    {
        Vector2 direction = rb.velocity;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, -OffsetZAngle) * Quaternion.AngleAxis(angle, Vector3.forward);

        Destroy(this.gameObject, 10f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {    
        var explosionObject = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(explosionObject, 0.5f);
    }
}
