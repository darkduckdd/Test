using Game.Level.Unit;
using UnityEngine;

public class CleanerUnitView : UnitView
{
    [SerializeField] private ParticleSystem _particles;

    internal void PlayUnitParticles()
    {
        _particles.Play();
    }
}
