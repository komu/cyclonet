using System;
using System.Collections.Generic;
using OpenTK;

namespace CycloNet.Physics.Particles
{
    /// <summary>
    /// A contact generator that takes particles and
    /// collides them against the ground.
    /// </summary>
    public class GroundContacts : IParticleContactGenerator
    {
        // Reference to a mutable list
        readonly List<Particle> particles;

        public GroundContacts(List<Particle> particles)
        {
            this.particles = particles;
        }

        public void AddContacts(List<ParticleContact> contacts, int maxContacts)
        {
            foreach (var p in particles)
            {
                if (contacts.Count >= maxContacts) break;

                var y = p.Position.Y;
                if (y < 0.0f)
                {
                    contacts.Add(new ParticleContact
                    {
                        ContactNormal = Vector3.UnitY,
                        Particle0 = p,
                        Particle1 = null,
                        Penetration = -y,
                        Restitution = 0.2f
                    });
                }
            }
        }
    }
}

