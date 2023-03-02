using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// Controls the UI guide for the player, allowing them to toggle the display of various UI elements and moving the guide up and down.
    /// </summary>
    public class UIGuide : MonoBehaviour
    {
        private RectTransform rect;
        private UIManager uiManager;
        private enum CurrentUIGuideState
        {
            Show,
            Hide
        }

        private CurrentUIGuideState currentUIGuideState;

        void Start()
        {
            rect = GetComponent<RectTransform>();
            uiManager = UIManager.Instance;
            currentUIGuideState = CurrentUIGuideState.Show;
        }

        private void Update()
        {
            // Toggle the player inventory canvas when the "E" key is pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (uiManager.PlayerInventoryCanvas.activeInHierarchy)
                    uiManager.PlayerInventoryCanvas.SetActive(false);
                else
                    uiManager.PlayerInventoryCanvas.SetActive(true);
            }


            // Toggle the creative inventory canvas when the "R" key is pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (uiManager.CreativeInventoryCanvas.activeInHierarchy)
                    uiManager.CreativeInventoryCanvas.SetActive(false);
                else
                    uiManager.CreativeInventoryCanvas.SetActive(true);
            }


            // Toggle the crafting table canvas when the "C" key is pressed
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (uiManager.CraftingTableCanvas.activeInHierarchy)
                    uiManager.CraftingTableCanvas.SetActive(false);
                else
                    uiManager.CraftingTableCanvas.SetActive(true);
            }


            // Toggle the menu canvas when the "Escape" key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (uiManager.MenuCanvas.activeInHierarchy)
                    uiManager.MenuCanvas.SetActive(false);
                else
                    uiManager.MenuCanvas.SetActive(true);
            }

        }


        /// <summary>
        /// Toggles the UI guide up and down based on its current state.
        /// </summary>
        public void Toggle()
        {
            if (currentUIGuideState == CurrentUIGuideState.Show)
            {
                MoveDown();
                currentUIGuideState = CurrentUIGuideState.Hide;
                return;
            }

            if (currentUIGuideState == CurrentUIGuideState.Hide)
            {
                MoveUp();
                currentUIGuideState = CurrentUIGuideState.Show;
                return;
            }
        }


        /// <summary>
        /// Moves the UI guide down by 270 units in the Y direction.
        /// </summary>
        private void MoveDown()
        {
            // Get the current position of the UI element
            Vector3 currentPosition = rect.localPosition;

            // Move the UI element down by 300 units in the Y direction
            currentPosition.y -= 270f;

            // Update the position of the UI element
            rect.localPosition = currentPosition;
        }


        /// <summary>
        /// Moves the UI guide up by 270 units in the Y direction.
        /// </summary>
        private void MoveUp()
        {
            // Get the current position of the UI element
            Vector3 currentPosition = rect.localPosition;

            // Move the UI element down by 300 units in the Y direction
            currentPosition.y += 270f;

            // Update the position of the UI element
            rect.localPosition = currentPosition;
        }
    }
}