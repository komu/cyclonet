using System;
using System.Collections.Generic;
using OpenTK;

namespace CycloNet.Physics.Particles
{
    public abstract class ParticleConstraint : IParticleContactGenerator
    {
        /// <summary>
        /// Particle connected to this constraint.
        /// </summary>
        public Particle Particle;

        /// <summary>
        /// The point to which the particle is anchored.
        /// </summary>
        public Vector3 Anchor;

        protected ParticleConstraint()
        {
        }

        protected ParticleConstraint(Particle particle, Vector3 anchor)
        {
            this.Particle = particle;
            this.Anchor = anchor;
        }

        protected float CurrentLength
        {
            get { return (Particle.Position - Anchor).Length; }
        }

        public abstract void AddContacts(List<ParticleContact> contacts, int maxContacts);
    }

    public class ParticleCableConstraint : ParticleConstraint
    {
        public float MaxLength;
        public float Restitution;

        public ParticleCableConstraint()
        {
        }
        
        public ParticleCableConstraint(Particle particle, Vector3 anchor):
            base(particle, anchor)
        {
        }

        public override void AddContacts(List<ParticleContact> contacts, int maxContacts)
        {
            float length = CurrentLength;
            if (length < MaxLength)
                return;

            var normal = Anchor - Particle.Position;
            normal.Normalize();

            contacts.Add(new ParticleContact
            {
                Particle0 = Particle,
                Particle1 = null,
                ContactNormal = normal,
                Penetration = length - MaxLength,
                Restitution = Restitution
            });
        }
    }

    public class ParticleRodConstraint : ParticleConstraint
    {
        public float Length;

        public override void AddContacts(List<ParticleContact> contacts, int maxContacts)
        {
            // Find the length of the rod
            float currentLen = CurrentLength;

            // Check if we're over-extended
            if (currentLen == Length)
                return;

            var contact = new ParticleContact
            {
                Particle0 = Particle,
                Particle1 = null,
                Restitution = 0 // no bounciness
            };

            // Calculate the normal
            var normal = Anchor - Particle.Position;
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
