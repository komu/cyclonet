using System;
using System.Collections.Generic;

namespace CycloNet.Physics.Particles
{
    public class ParticleForceRegistry
    {
        readonly List<ParticleForceRegistration> registrations = new List<ParticleForceRegistration>();

        public void Add(Particle particle, IParticleForceGenerator fg)
        {
            registrations.Add(new ParticleForceRegistration(particle, fg));
        }


        //public void Remove(Particle particle, IParticleContactGenerator fg)
        //{

        //}

        public void Clear()
        {
            registrations.Clear();
        }

        public void UpdateForces(float duration)
        {
            foreach (var reg in registrations)
                reg.fg.UpdateForce(reg.particle, duration);
        }

        struct ParticleForceRegistration
        {
            internal Particle particle;
            internal IParticleForceGenerator fg;

            internal ParticleForceRegistration(Particle particle, IParticleForceGenerator fg)
            {
                this.particle = particle;
                this.fg = fg;
            }
        }
    }
}
