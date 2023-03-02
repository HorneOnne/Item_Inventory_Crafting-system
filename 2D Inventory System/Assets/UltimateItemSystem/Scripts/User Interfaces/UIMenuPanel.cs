using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltimateItemSystem
{
    /// <summary>
    /// Represents a UI menu panel.
    /// </summary>
    public class UIMenuPanel : MonoBehaviour
    {
        private void Start()
        {
            UIManager.Instance.MenuCanvas.SetActive(false);
        }

        /// <summary>
        /// Reloads the current scene.
        /// </summary>
        public void ReloadScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Single);
        }


        /// <summary>
        /// Saves the current game state.
        /// </summary>
        public void Save()
        {
            SaveManager.Instance.Save();
        }


        /// <summary>
        /// Loads the last saved game state.
        /// </summary>
        public void Load()
        {
            SaveManager.Instance.Load();
        }
    }

}
