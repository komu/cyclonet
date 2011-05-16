using System;
using System.Collections.Generic;

namespace CycloNet.Physics.Particles
{
    /// <summary>
    /// Contact generator for particles.
    /// </summary>
    public interface IParticleContactGenerator
    {
        /// <summary>
        /// Adds contacts to given list of contact.
        /// </summary>
        void AddContacts(List<ParticleContact> contacts, int maxContacts);
    }
}

