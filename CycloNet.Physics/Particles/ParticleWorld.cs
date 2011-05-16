using System;
using System.Collections.Generic;

namespace CycloNet.Physics.Particles
{
    public class ParticleWorld
    {
        readonly List<Particle> particles = new List<Particle>();
        readonly bool calculateIterations;
        readonly ParticleForceRegistry registry = new ParticleForceRegistry();
        readonly ParticleContactResolver resolver;
        readonly List<IParticleContactGenerator> contactGenerators = new List<IParticleContactGenerator>();
        readonly List<ParticleContact> contacts;
        readonly int maxContacts;

        public ParticleWorld(int maxContacts):
            this(maxContacts, 0)
        {
        }

        public ParticleWorld(int maxContacts, int iterations)
        {
            this.maxContacts = maxContacts;
            this.contacts = new List<ParticleContact>(maxContacts);
            resolver = new ParticleContactResolver(iterations);
            calculateIterations = (iterations == 0);
        }

        private void GenerateContacts()
        {
            contacts.Clear();

            foreach (var g in contactGenerators)
            {
                g.AddContacts(contacts, maxContacts);

                // We've run out of contacts to fill. This means we're missing contacts.
                if (contacts.Count >= maxContacts)
                    break;
            }
        }

        /// <summary>
        /// Integrates all the particles in this world forward in time by given duration.
        /// </summary>
        public void Integrate(float duration)
        {
            foreach (var p in particles)
                p.Integrate(duration);
        }

        /// <summary>
        /// Processes all the physics for the particle world.
        /// </summary>
        public void RunPhysics(float duration)
        {
            registry.UpdateForces(duration);

            Integrate(duration);

            GenerateContacts();

            if (contacts.Count != 0)
            {
                if (calculateIterations)
                    resolver.Iterations = contacts.Count * 2;
                resolver.ResolveContacts(contacts, duration);
            }
        }

        /// <summary>
        /// Initializes the world for a simulation frame. This clears
        /// the force accumulators for particles in the world. After
        /// calling this, the particles can have their forces for this
        /// frame added.
        /// </summary>
        public void StartFrame()
        {
            foreach (var p in particles)
                p.ClearAccumulator();
        }

        public List<Particle> Particles
        {
            get { return particles; }
        }

        public List<IParticleContactGenerator> ContactGenerators
        {
            get { return contactGenerators; }
        }
    }
}
