using System.Collections.Generic;
using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Component that controls a ParticleSystem and sets the texture sheet animation sprites.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleControl : MonoBehaviour
    {
        private ParticleSystem m_particleSystem;

        private void Awake()
        {
            m_particleSystem = GetComponent<ParticleSystem>();
        }


        /// <summary>
        /// Sets the list of sprite frames for the texture sheet animation of the ParticleSystem.
        /// </summary>
        /// <param name="frames">The list of sprites to set as frames for the texture sheet animation.</param>
        public void SetParticles(List<Sprite> frames)
        {
            for (int i = 0; i < frames.Count; i++)
            {
                m_particleSystem.textureSheetAnimation.SetSprite(i, frames[i]);
            }
        }
    }
}

