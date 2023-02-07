using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticleDataManager : Singleton<ProjectileParticleDataManager>   
{
    public List<ProjectileParticleData> projectileParticleDatas;

    public List<Sprite> GetProjectileParticleFrames(int index)
    {
        return projectileParticleDatas[index].frames;
    }


}

[System.Serializable]
public struct ProjectileParticleData
{
    public List<Sprite> frames;
}
