namespace UltimateItemSystem
{
    public class ItemForDrop : Item
    {

        public void Set(ItemSlot itemSlot)
        {
            base.SetData(itemSlot);
        }


        public override void Collect(Player player)
        {
            var returnSlot = player.PlayerInventory.AddItem(ItemSlot);


            if (returnSlot.HasItem() == true)
            {
                Set(returnSlot);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
