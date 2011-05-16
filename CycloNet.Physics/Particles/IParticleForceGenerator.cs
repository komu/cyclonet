using System;
namespace CycloNet.Physics.Particles
{
    public interface IParticleForceGenerator
    {
        void UpdateForce(Particle particle, float duration);
    }
}

