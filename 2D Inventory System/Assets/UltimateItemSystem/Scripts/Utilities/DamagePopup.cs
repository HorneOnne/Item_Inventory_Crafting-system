using TMPro;
using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// Class representing a popup for displaying damage values.
    /// </summary>
    public class DamagePopup : MonoBehaviour
    {
        private const float DISAPPEAR_TIMER_MAX = 1f;
        private float disappearTimer;
        private Color textColor;
        private Vector3 moveVector;
        private TextMeshPro textMesh;

        private bool wasReturnToPool;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
        }


        /// <summary>
        /// Sets up the damage popup with the given parameters.
        /// </summary>
        /// <param name="damage">The amount of damage to be displayed.</param>
        /// <param name="color">The color of the damage text.</param>
        /// <param name="size">The size of the damage text.</param>
        /// <param name="moveVector">The movement vector for the damage popup.</param>
        /// <param name="rotation">The rotation of the damage popup.</param>
        public void SetUp(int damage, Color color, float size, Vector3 moveVector, Vector3 rotation)
        {
            wasReturnToPool = false;

            this.textColor = color;
            textMesh.color = textColor;
            textMesh.fontSize = size;
            this.transform.rotation = Quaternion.Euler(rotation);

            textMesh.text = damage.ToString();

            disappearTimer = DISAPPEAR_TIMER_MAX;
            this.moveVector = moveVector * 60f;
        }

        private void Update()
        {
            transform.position += moveVector * Time.deltaTime;
            moveVector -= moveVector * 8f * Time.deltaTime;

            if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
            {
                // First half of the popup lifetime
                float increaseScaleAmount = 1f;
                transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            }
            else
            {
                // Second half of the popup lifetime
                float decreaseScaleAmount = 1f;
                transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            }

            disappearTimer -= Time.deltaTime;
            if (disappearTimer < 0)
            {
                // Start disappearing
                float disappearSpeed = 3f;
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMesh.color = textColor;
                if (textColor.a < 0)
                {
                    //Destroy(gameObject);
                    ReturnToPool();
                }
            }
        }


        /// <summary>
        /// Returns the damage popup to the object pool.
        /// </summary>
        private void ReturnToPool()
        {
            if (wasReturnToPool == true) return;

            ResetProperties();
            DamagePopupSpawner.Instance.Pool.Release(this.gameObject);
            wasReturnToPool = true;
        }

        /// <summary>
        /// Resets the properties of the damage popup.
        /// </summary>
        private void ResetProperties()
        {
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}