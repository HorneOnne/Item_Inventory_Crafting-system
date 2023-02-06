using System.Collections.Concurrent;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Arrow : Item, IConsumability, ICanCauseDamage
{
    [Header("References")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Transform explosionPosition;
    private Rigidbody2D rb;



    [field:SerializeField]
    public bool Consumability { get; set; }

    public BowData bowData;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(BowData bowData, float shootSpeed)
    {
        this.bowData = bowData;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = Quaternion.Euler(0, 0, OffsetZAngle) * transform.right * shootSpeed;
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
        var explosionObject = Instantiate(explosionPrefab, explosionPosition.position, Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(explosionObject, 0.5f);
    }

    public void Consume(Player player)
    {
        if (Consumability)
        {
            ItemSlot.RemoveItem();
            var playerInventory = player.PlayerInventory;
            var slotIndex = playerInventory.GetSlotIndex(ItemSlot);
            UIPlayerInventory.Instance.UpdateInventoryUIAt((int)slotIndex);

        }
    }

    public int GetDamage()
    {
        return ((ArrowData)ItemData).damage + bowData.baseAttackDamage;
    }
}
