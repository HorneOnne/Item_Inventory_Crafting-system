using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class MagicStaffProjectile_001 : Projectile, ICanCauseDamage
{
    private MagicStaffData magicStaffData;
    private Vector2 startPosition;


    protected override void Start()
    {
        base.Start();
        this.magicStaffData = (MagicStaffData)ItemData;
        startPosition = transform.position;
        SetDust(magicStaffData.particle);


        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - startPosition;
        rb.velocity = direction.normalized * magicStaffData.releaseProjectileSpeed;
        Destroy(this.gameObject, 10f);
    }


    public int GetDamage()
    {
        return magicStaffData.damage;
    }
}
