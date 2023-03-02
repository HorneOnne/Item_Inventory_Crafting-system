using TMPro;

namespace UltimateItemSystem
{
    /// <summary>
    /// A UI item slot that displays its slot index using a TextMeshProUGUI component.
    /// Inherits from UIItemSlot.
    /// </summary>
    public class UINumericItemSlot : UIItemSlot
    {
        /// <summary>
        /// The TextMeshProUGUI component used to display the slot index.
        /// </summary>
        public TextMeshProUGUI slotIndexText;
    }
}
