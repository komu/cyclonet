using System;
using OpenTK;

namespace CycloNet.Physics.Particles
{
    public class ParticleContact
    {
        public Vector3 ContactNormal;
        public Particle Particle0;
        public Particle Particle1;
        public float Penetration;
        public float Restitution;

        public Vector3 ParticleMovement0 { get; private set; }
        public Vector3 ParticleMovement1 { get; private set; }

        public float CalculateSeparatingVelocity()
        {
            var relativeVelocity = Particle0.Velocity;
            if (Particle1 != null)
                relativeVelocity -= Particle1.Velocity;

            return Vector3.Dot(relativeVelocity, ContactNormal);
        }

        public void Resolve(float duration)
        {
            ResolveVelocity(duration);
            ResolveInterpenetration(duration);
        }

        private void ResolveVelocity(float duration)
        {
            var separatingVelocity = CalculateSeparatingVelocity();

            // If the contact is either stationary or separating, no impulse is required.
            if (separatingVelocity > 0)
                return;

            // Calculate the new separating velocity
            var newSepVelocity = -separatingVelocity * Restitution;

            // Check the velocity build-up due to acceleration only
            var accCausedVelocity = Particle0.Acceleration;

            if (Particle1 != null)
                accCausedVelocity -= Particle1.Acceleration;

            var accCausedSepVelocity = Vector3.Dot(accCausedVelocity, ContactNormal) * duration;

            // If we've got a closing velocity due to acceleration buildup,
            // remove it from the new separating velocity
            if (accCausedSepVelocity < 0)
            {
                newSepVelocity += Restitution * accCausedSepVelocity;

                // Make sure we haven't removed more than we have to remove.
                if (newSepVelocity < 0)
                    newSepVelocity = 0;
            }

            var deltaVelocity = newSepVelocity - separatingVelocity;

            // We apply the change in velocity to each object in proportion to
            // their inverse mass (i.e. those with lower inverse mass [higher
            // actual mass] get less change in velocity)..
            var totalInverseMass = CalculateTotalInverseMass();

            // If all particles have infinite mass, then impulses have no effect
            if (totalInverseMass <= 0) return;

            // Calculate the impulse to apply
            var impulse = deltaVelocity / totalInverseMass;

            // Find the amount of impulse per unit of inverse mass
            var impulsePerIMass = ContactNormal * impulse;

            // Apply impulses: they are applied in the direction of the contact,
            // and are proportional to the inverse mass.
            Particle0.Velocity += impulsePerIMass * Particle0.InverseMass;
            if (Particle1 != null)
            {
                // Particle 1 goes in the opposite direction
                Particle1.Velocity += impulsePerIMass * -Particle1.InverseMass;
            }
        }

        private void ResolveInterpenetration(float duration)
        {
            // If we don't have any penetration, skip this step.
            if (Penetration <= 0) return;

            // The movement of each object is based on their inverse mass, so
            // total that.
            var totalInverseMass = CalculateTotalInverseMass();

            // If all particles have infinite mass, then we do nothing
            if (totalInverseMass <= 0) return;

            // Find the amount of penetration resolution per unit of inverse mass
            Vector3 movePerIMass = ContactNormal * (Penetration / totalInverseMass);

            // Calculate the the movement amounts
            ParticleMovement0 = movePerIMass * Particle0.InverseMass;
            if (Particle1 != null) {
                ParticleMovement1 = movePerIMass * -Particle1.InverseMass;
            } else {
                ParticleMovement1 = Vector3.Zero;
            }

            // Apply the penetration resolution
            Particle0.Position += ParticleMovement0;
            if (Particle1 != null)
                Particle1.Position += ParticleMovement1;
        }

        private float CalculateTotalInverseMass()
        {
            return Particle0.InverseMass + (Particle1 != null ? Particle1.InverseMass : 0);
        }
    }
}

