using UnityEngine;

public class MagicStaff : Item
{
    private GameObject magicStaffProjectilePrefab;
    private GameObject magicStaffProjectileObject;
    private MagicStaffData magicStaffData;


    [field: SerializeField]
    public bool UseGravity { get; set; }


    protected override void Start()
    {
        base.Start();
        base.SetOffsetPosition();

        magicStaffData = (MagicStaffData)ItemData;
    }


    public override bool Use(Player player)
    {
        magicStaffProjectilePrefab = ItemContainerManager.Instance.GetItemPrefab("MagicStaffProjectile_001");
        magicStaffProjectileObject = Instantiate(magicStaffProjectilePrefab, transform.position, transform.rotation);
        magicStaffProjectileObject.transform.localScale = new Vector3(1, 1, 1);
        magicStaffProjectileObject.SetActive(true);
        magicStaffProjectileObject.GetComponent<Projectile>().SetData(this.ItemSlot.ItemObject, magicStaffData.projectile, UseGravity);


        return true;
    }
}
