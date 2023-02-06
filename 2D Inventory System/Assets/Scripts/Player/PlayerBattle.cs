using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerBattle : MonoBehaviour
{
    [Header("References")]
    private Player player;
    private ItemInHand itemInHand;
    private PlayerData playerData;

    private void Start()
    {
        player = GetComponent<Player>();
        playerData = player.playerData;
        itemInHand = player.ItemInHand;
    }

    public int GetPlayerDamage()
    {
        int itemInHandDamage = 0;
        if (itemInHand.GetItemObject() is ICanCauseDamage)
        {
            itemInHandDamage = itemInHand.GetItemObject().GetComponent<ICanCauseDamage>().GetDamage();
        }

        return playerData.baseAttackDamage + itemInHandDamage;
    }

    public void AddWeaponDamage()
    {
        
    }
}
