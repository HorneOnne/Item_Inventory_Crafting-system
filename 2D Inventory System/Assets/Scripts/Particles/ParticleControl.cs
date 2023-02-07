using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleControl : MonoBehaviour
{
    private ParticleSystem m_particleSystem;

    private void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();    
    }


    public void SetParticles(List<Sprite> frames)
    {
        for(int i = 0; i < frames.Count; i++) 
        {
            m_particleSystem.textureSheetAnimation.SetSprite(i, frames[i]);
        }
    }



}

