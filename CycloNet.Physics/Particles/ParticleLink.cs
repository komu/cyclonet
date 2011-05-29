using System;
using System.Collections.Generic;

namespace CycloNet.Physics.Particles
{
    /// <summary>
    /// Links connect two particles together, generating a contact if
    /// they violate the constraints of their link. It is used as a
    /// base class for cables and rods, and could be used as a base
    /// class for springs with a limit to their extension..
    /// </summary>
    public abstract class ParticleLink : IParticleContactGenerator
    {
        public Particle Particle0;
        public Particle Particle1;

        protected ParticleLink()
        {
        }

        protected ParticleLink(Particle particle0, Particle particle1)
        {
            Particle0 = particle0;
            Particle1 = particle1;
        }

        protected float CurrentLength
        {
            get { return (Particle0.Position - Particle1.Position).Length; }
        }

        public abstract void AddContacts(List<ParticleContact> contacts, int maxContacts);
    }

    public class ParticleCable : ParticleLink
    {
        public float MaxLength;
        public float Restitution;

        public ParticleCable()
        {
        }

        public ParticleCable(Particle particle0, Particle particle1):
            base(particle0, particle1)
        {
        }

        public override void AddContacts(List<ParticleContact> contacts, int maxContacts)
        {
            // Find the length of the cable
            var length = CurrentLength;

            // Check if we're over-extended
            if (length < MaxLength)
                return;

            // Calculate the normal
            var normal = Particle1.Position - Particle0.Position;
            normal.Normalize();

            // Otherwise return the contact
            contacts.Add(new ParticleContact
            {
                Particle0 = Particle0,
                Particle1 = Particle1,
                ContactNormal = normal,
                Penetration = length - MaxLength,
                Restitution = Restitution
            });
        }
    }

    public class ParticleRod : ParticleLink
    {
        public float Length;

        public ParticleRod()
        {
        }

        public ParticleRod(Particle particle0, Particle particle1):
            base(particle0, particle1)
        {
        }

        public override void AddContacts(List<ParticleContact> contacts, int maxContacts)
        {
            // Find the length of the rod
            float currentLen = CurrentLength;

            // Check if we're over-extended
            if (currentLen == Length)
                return;

            var contact = new ParticleContact
            {
                Particle0 = Particle0,
                Particle1 = Particle1,
                Restitution = 0 // no bounciness
            };

            // Calculate the normal
            var normal = Particle1.Position - Particle0.Position;
            normal.Normalize();

            // The contact normal depends on whether we're extending or compressing
            if (currentLen > Length) {
                contact.ContactNormal = normal;
                contact.Penetration = currentLen - Length;
            } else {
                contact.ContactNormal = -normal;
                contact.Penetration = Length - currentLen;
            }

            contacts.Add(contact);
        }
    }
}

