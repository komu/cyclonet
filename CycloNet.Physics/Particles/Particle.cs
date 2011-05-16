using System;
using System.Diagnostics;
using OpenTK;

namespace CycloNet.Physics.Particles
{
    /// <summary>
    /// A particle is the simplest object that can be simulated in the
    /// physics system.
    ///
    /// It has position data (no orientation data), along with
    /// velocity. It can be integrated forward through time, and have
    /// linear forces, and impulses applied to it.
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// Holds the inverse of the mass of the particle. It
        /// is more useful to hold the inverse mass because
        /// integration is simpler, and because in real time
        /// simulation it is more useful to have objects with
        /// infinite mass (immovable) than zero mass
        /// (completely unstable in numerical simulation).
        /// </summary>
        public float InverseMass { get; set; }

        /// <summary>
        /// Holds the amount of damping applied to linear
        /// motion. Damping is required to remove energy added
        /// through numerical instability in the integrator.
        /// </summary>
        public float Damping { get; set; }

        /// <summary>
        /// Position of the particle in world space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Linear velocity of the particle in world space.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Holds the acceleration of the particle.  This value
        /// can be used to set acceleration due to gravity (its primary
        /// use), or any other constant acceleration.
        /// </summary>
        public Vector3 Acceleration { get; set; }

        /// <summary>
        /// Holds the accumulated force to be applied at the next
        /// simulation iteration only. This value is zeroed at each
        /// integration step.
        /// </summary>
        Vector3 forceAccum;

        public void Integrate(float duration)
        {
            Debug.Assert(duration > 0.0f);

            // We don't integrate things with zero mass.
            if (InverseMass <= 0.0f) return;

            // Update linear position.
            Position += duration * Velocity;

            // Work out the acceleration from the force
            Vector3 resultingAcc = Acceleration + InverseMass * forceAccum;

            // Update linear velocity from the acceleration.
            Velocity += duration * resultingAcc;

            // Impose drag.
            Velocity *= MathUtils.Pow(Damping, duration);

            // Clear the forces.
            ClearAccumulator();
        }

        public float Mass
        {
            get
            {
                return (InverseMass == 0)
                    ? float.MaxValue
                    : (1.0f/InverseMass);
            }
            set
            {
                Debug.Assert(value != 0);
                InverseMass = 1.0f/value;
            }
        }

        public bool HasFiniteMass
        {
            get { return InverseMass >= 0.0f; }
        }

        public void ClearAccumulator()
        {
            forceAccum = Vector3.Zero;
        }

        public void AddForce(Vector3 force)
        {
            forceAccum += force;
        }
    }
}

