using System.Collections.Generic;
using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// Represents a collection of sprite frames that define a projectile particle.
    /// </summary>
    [System.Serializable]
    public struct ProjectileParticleData
    {
        /// <summary>
        /// A list of sprite frames that define the particle animation.
        /// </summary>
        public List<Sprite> frames;
    }
}