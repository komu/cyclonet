using System;
using OpenTK;

namespace CycloNet.Physics.Particles
{
    public class ParticleGravity : IParticleForceGenerator
    {
        readonly Vector3 gravity;

        public ParticleGravity(Vector3 gravity)
        {
            this.gravity = gravity;
        }

        public void UpdateForce(Particle particle, float duration)
        {
            if (!particle.HasFiniteMass) return;

            particle.AddForce(gravity * particle.Mass);
        }
    }

    public class ParticleDrag : IParticleForceGenerator
    {
        readonly float k1;
        readonly float k2;

        public ParticleDrag(float k1, float k2)
        {
            this.k1 = k1;
            this.k2 = k2;
        }

        public void UpdateForce(Particle particle, float duration)
        {
            var force = particle.Velocity;

            // Calculate the total drag coefficient
            var length = force.Length;
            var dragCoeff = k1 * length + k2 * length * length;

            // Calculate the final force and apply it
            force.Normalize();
            force *= -dragCoeff;

            particle.AddForce(force);
        }
    }

    public class ParticleSpring : IParticleForceGenerator
    {
        readonly Particle other;
        readonly float springConstant;
        readonly float restLength;

        public ParticleSpring(Particle other, float springConstant, float restLength)
        {
            this.other = other;
            this.springConstant = springConstant;
            this.restLength = restLength;
        }

        public void UpdateForce(Particle particle, float duration)
        {
            // Calculate the vector of the spring
            var force = particle.Position - other.Position;

            // Calculate the magnitude of the force
            var magnitude = springConstant * Math.Abs(force.Length - restLength);

            // Calculate the final force and apply it
            force.Normalize();
            force *= -magnitude;

            particle.AddForce(force);
        }
    }

    public class ParticleBuoyancy : IParticleForceGenerator
    {
        readonly float maxDepth;
        readonly float volume;
        readonly float waterHeight;
        public float LiquidDensity { get; set; }

        public ParticleBuoyancy(float maxDepth, float volume, float waterHeight)
        {
            this.maxDepth = maxDepth;
            this.volume = volume;
            this.waterHeight = waterHeight;
            LiquidDensity = 1000;
        }

        public void UpdateForce(Particle particle, float duration)
        {
            // Calculate the submersion depth
            var depth = particle.Position.Y;

            // Check if we're out of the water
            if (depth >= waterHeight + maxDepth) return;

            Vector3 force = Vector3.Zero;

            // Check if we are at maximum depth (1st clause), or only partly submerged (2nd clause)
            if (depth <= waterHeight - maxDepth)
            {
                force.Y = LiquidDensity * volume;
            }
            else
            {
                force.Y = LiquidDensity * volume *
                    (depth - maxDepth - waterHeight) / 2 * maxDepth;
            }
            particle.AddForce(force);
        }
    }

    public class ParticleBungee : IParticleForceGenerator
    {
        readonly Particle other;
        readonly float springConstant;
        readonly float restLength;

        public ParticleBungee(Particle other, float springConstant, float restLength)
        {
            this.other = other;
            this.springConstant = springConstant;
            this.restLength = restLength;
        }


        public void UpdateForce(Particle particle, float duration)
        {
            // Calculate the vector of the spring
            var force = particle.Position - other.Position;

            var magnitude = force.Length;

            // Check if the bungee is compressed
            if (magnitude <= restLength) return;

            // Calculate the magnitude of the force
            magnitude = springConstant * (restLength - magnitude);

            // Calculate the final force and apply it
            force.Normalize();
            force *= -magnitude;

            particle.AddForce(force);
        }
    }

    public class ParticleFakeSpring : IParticleForceGenerator
    {
        readonly Func<Vector3> anchor;
        readonly float springConstant;
        readonly float damping;

        public ParticleFakeSpring(Vector3 vector, float springConstant, float damping):
            this(() => vector, springConstant, damping)
        {

        }

        public ParticleFakeSpring(Func<Vector3> anchor, float springConstant, float damping)
        {
            this.anchor = anchor;
            this.springConstant = springConstant;
            this.damping = damping;
        }

        public void UpdateForce(Particle particle, float duration)
        {
            // Check that we do not have infinite mass
            if (!particle.HasFiniteMass) return;

            // Calculate the relative position of the particle to the anchor
            Vector3 position = particle.Position - anchor();

            // Calculate the constants and check they are in bounds.
            var gamma = 0.5f * MathUtils.Sqrt(4 * springConstant - damping*damping);
            if (gamma == 0.0f) return;

            Vector3 c = position * (damping / (2.0f * gamma)) +
                particle.Velocity * (1.0f / gamma);

            // Calculate the target position
            Vector3 target = position * MathUtils.Cos(gamma * duration) +
                c * MathUtils.Sin(gamma * duration);
            target *= MathUtils.Exp(-0.5f * duration * damping);

            // Calculate the resulting acceleration and therefore the force
            Vector3 accel = (target - position) * (1.0f / duration*duration) - particle.Velocity * duration;
            particle.AddForce(accel * particle.Mass);
        }
    }

    public class ParticleAnchoredSpring : IParticleForceGenerator
    {
        readonly Func<Vector3> anchor;
        readonly float springConstant;
        readonly float restLength;

        public ParticleAnchoredSpring(Vector3 anchor, float springConstant, float restLength):
            this(() => anchor, springConstant, restLength)
        {
        }

        public ParticleAnchoredSpring(Func<Vector3> anchor, float springConstant, float restLength)
        {
            this.anchor = anchor;
            this.springConstant = springConstant;
            this.restLength = restLength;
        }

        public void UpdateForce(Particle particle, float duration)
        {
            // Calculate the vector of the spring
            var force = particle.Position - anchor();

            // Calculate the magnitude of the force
            var magnitude = (restLength - force.Length) * springConstant;
        
            // Calculate the final force and apply it
            force.Normalize();
            force *= magnitude;

            particle.AddForce(force);
        }
    }

    public class ParticleAnchroredBungee : IParticleForceGenerator
    {
        readonly Func<Vector3> anchor;
        readonly float springConstant;
        readonly float restLength;

        public ParticleAnchroredBungee(Vector3 anchor, float springConstant, float restLength):
            this(() => anchor, springConstant, restLength)
        {
        }

        public ParticleAnchroredBungee(Func<Vector3> anchor, float springConstant, float restLength)
        {
            this.anchor = anchor;
            this.springConstant = springConstant;
            this.restLength = restLength;
        }

        public void UpdateForce(Particle particle, float duration)
        {
            // Calculate the vector of the spring
            var force = particle.Position - anchor();
        
            // Calculate the magnitude of the force
            float magnitude = force.Length;
            if (magnitude < restLength) return;
        
            magnitude -= restLength;
            magnitude *= springConstant;

            // Calculate the final force and apply it
            force.Normalize();
            force *= -magnitude;

            particle.AddForce(force);
        }
    }
}
