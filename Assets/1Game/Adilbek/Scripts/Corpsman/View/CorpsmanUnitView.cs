using Game.Level.Unit;
using UnityEngine;

public class CorpsmanUnitView : UnitView
{
    [SerializeField] private ParticleSystem _particles;

    internal void PlayUnitParticles()
    {
        _particles.Play();
    }
}
