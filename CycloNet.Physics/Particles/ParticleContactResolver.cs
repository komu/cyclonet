using System;
using System.Collections.Generic;
using OpenTK;

namespace CycloNet.Physics.Particles
{
    public class ParticleContactResolver
    {
        /// <summary>
        /// The number of iterations through the resolution algorithm.
        /// This should be at least the number of contacts (otherwise
        /// some constraints will not be resolved - although sometimes
        /// this is not noticable). If the iterations are not needed they
        /// will not be used, so adding more iterations may not make any
        /// difference. But in some cases you would need millions of
        /// iterations. Think about the number of iterations as a bound:
        /// if you specify a large number, sometimes the algorithm WILL use
        /// it, and you may drop frames.
        /// </summary>
        public int Iterations { get; set; }

        public ParticleContactResolver(int iterations)
        {
            Iterations = iterations;
        }

        /// <summary>
        /// Resolves a set of particle contacts for both penetration
        /// and velocity.
        ///
        /// Contacts that cannot interact with each other should be
        /// passed to separate calls to resolveContacts, as the
        /// resolution algorithm takes much longer for lots of contacts
        /// than it does for the same number of contacts in small sets.
        ///
        /// <param name="contacts">
        /// Contacts to resolve.
        /// </param>
        /// <param name="duration">
        /// The duration of the previous integration step.
        /// This is used to compensate for forces applied.
        /// </param>
        public void ResolveContacts(List<ParticleContact> contacts, float duration)
        {
            for (int iterationsUsed = 0; iterationsUsed < Iterations; iterationsUsed++)
            {
                // Find the contact with the largest closing velocity;
                var max = float.MaxValue;
                ParticleContact maxContact = null;

                foreach (var contact in contacts)
                {
                    var sepVel = contact.CalculateSeparatingVelocity();
                    if (sepVel < max && (sepVel < 0 || contact.Penetration > 0))
                    {
                        max = sepVel;
                        maxContact = contact;
                    }
                }

                // Do we have anything worth resolving?
                if (maxContact == null) break;

                // Resolve this contact
                maxContact.Resolve(duration);

                // Update the interpenetrations for all particles
                foreach (var contact in contacts)
                {
                    if (contact.Particle0 == maxContact.Particle0)
                    {
                        contact.Penetration -= Vector3.Dot(maxContact.ParticleMovement0, contact.ContactNormal);
                    }
                    else if (contact.Particle0 == maxContact.Particle1)
                    {
                        contact.Penetration -= Vector3.Dot(maxContact.ParticleMovement1, contact.ContactNormal);
                    }

                    if (contact.Particle1 != null)
                    {
                        if (contact.Particle1 == maxContact.Particle0)
                        {
                            contact.Penetration += Vector3.Dot(maxContact.ParticleMovement0, contact.ContactNormal);
                        }
                        else if (contact.Particle1 == maxContact.Particle1)
                        {
                            contact.Penetration += Vector3.Dot(maxContact.ParticleMovement1, contact.ContactNormal);
                        }
                    }
                }
            }
        }
    }
}

