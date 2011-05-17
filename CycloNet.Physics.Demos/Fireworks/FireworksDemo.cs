using System;
using System.Collections.Generic;
using CycloNet.Physics.Particles;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CycloNet.Physics.Demos.Fireworks
{
    public class FireworksDemo : DemoApplication
    {
        const int MaxFireworks = 1024;

        readonly List<Firework> fireworks = new List<Firework>();
        readonly List<FireworkRule> rules = new List<FireworkRule>();

        public FireworksDemo()
        {
            Title = "CycloNet > FireWorks";

            InitFireworkRules();
        }

        private void InitFireworkRules()
        {
            rules.Add(new FireworkRule
            {
                Type = 1,
                MinAge = 0.5f,
                MaxAge = 1.4f,
                MinVelocity = new Vector3(-5, 25, -5),
                MaxVelocity = new Vector3(5, 28, 5),
                Damping = 0.1f,
                Payloads = new [] { new FireworkRule.Payload(3, 5), new FireworkRule.Payload(5, 5) }
            });

            rules.Add(new FireworkRule
            {
                Type = 2,
                MinAge = 0.5f,
                MaxAge = 1.0f,
                MinVelocity = new Vector3(-5, 10, -5),
                MaxVelocity = new Vector3(5, 20, 5),
                Damping = 0.8f,
                Payloads = new [] { new FireworkRule.Payload(4, 2) }
            });

            rules.Add(new FireworkRule
            {
                Type = 3,
                MinAge = 0.5f,
                MaxAge = 1.5f,
                MinVelocity = new Vector3(-5, -5, -5),
                MaxVelocity = new Vector3(5, 5, 5),
                Damping = 0.1f
            });

            rules.Add(new FireworkRule
            {
                Type = 4,
                MinAge = 0.25f,
                MaxAge = 0.5f,
                MinVelocity = new Vector3(-20, 5, -5),
                MaxVelocity = new Vector3(20, 5, 5),
                Damping = 0.2f
            });

            rules.Add(new FireworkRule
            {
                Type = 5,
                MinAge = 0.5f,
                MaxAge = 1.0f,
                MinVelocity = new Vector3(-20, 2, -5),
                MaxVelocity = new Vector3(20, 18, 5),
                Damping = 0.01f,
                Payloads = new [] { new FireworkRule.Payload(3, 5) }
            });

            rules.Add(new FireworkRule
            {
                Type = 6,
                MinAge = 3,
                MaxAge = 5,
                MinVelocity = new Vector3(-5, 5, -5),
                MaxVelocity = new Vector3(5, 10, 5),
                Damping = 0.95f
            });

            rules.Add(new FireworkRule
            {
                Type = 7,
                MinAge = 4,
                MaxAge = 5,
                MinVelocity = new Vector3(-5, 50, -5),
                MaxVelocity = new Vector3(5, 60, 5),
                Damping = 0.01f,
                Payloads = new [] { new FireworkRule.Payload(8, 10) }
            });

            rules.Add(new FireworkRule
            {
                Type = 8,
                MinAge = 0.25f,
                MaxAge = 0.5f,
                MinVelocity = new Vector3(-1, -1, -1),
                MaxVelocity = new Vector3(1, 1, 1),
                Damping = 0.01f
            });

            rules.Add(new FireworkRule
            {
                Type = 9,
                MinAge = 3,
                MaxAge = 5,
                MinVelocity = new Vector3(-15, 10, -5),
                MaxVelocity = new Vector3(15, 15, 5),
                Damping = 0.95f
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0, 0, 0.1f, 1.0f);
        }

        void Create(int type, int count, Firework parent)
        {
            FireworkRule rule = rules[type - 1];

            for (int i = 0; i < count; i++)
                fireworks.Add(rule.Create(parent));
        }

        protected override void DoUpdate(float duration)
        {
            ProcessInput();

            foreach (var firework in fireworks.ToArray())
            {
                if (firework.Update(duration))
                {
                    var rule = rules[firework.Type-1];
                    firework.Type = 0;

                    foreach (var payload in rule.Payloads)
                        Create(payload.Type, payload.Count, firework);
                }
            }

            fireworks.RemoveAll(f => f.Type == 0);
        }

        protected override void DoRender()
        {
            float size = 0.1f;

            // Clear the viewport and set the camera direction
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            var camera = Matrix4.LookAt(0, 4, 10,  0, 4, 0,  0, 1, 0);
            GL.LoadMatrix(ref camera);

            // Render each firework in turn
            GL.Begin(BeginMode.Quads);
            foreach (var firework in fireworks)
            {
                switch (firework.Type)
                {
                case 1: GL.Color3(1f,0f,0f); break;
                case 2: GL.Color3(1f,0.5f,0f); break;
                case 3: GL.Color3(1f,1f,0f); break;
                case 4: GL.Color3(0f,1f,0f); break;
                case 5: GL.Color3(0f,1f,1f); break;
                case 6: GL.Color3(0.4f,0.4f,1f); break;
                case 7: GL.Color3(1f,0f,1f); break;
                case 8: GL.Color3(1f,1f,1f); break;
                case 9: GL.Color3(1f,0.5f,0.5f); break;
                };

                var pos = firework.Position;
                GL.Vertex3(pos.X-size, pos.Y-size, pos.Z);
                GL.Vertex3(pos.X+size, pos.Y-size, pos.Z);
                GL.Vertex3(pos.X+size, pos.Y+size, pos.Z);
                GL.Vertex3(pos.X-size, pos.Y+size, pos.Z);

                // Render the firework's reflection
                GL.Vertex3(pos.X-size, -pos.Y-size, pos.Z);
                GL.Vertex3(pos.X+size, -pos.Y-size, pos.Z);
                GL.Vertex3(pos.X+size, -pos.Y+size, pos.Z);
                GL.Vertex3(pos.X-size, -pos.Y+size, pos.Z);
            }

            GL.End();
        }

        char? key;

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            key = e.KeyChar;
        }

        void ProcessInput()
        {
            if (key.HasValue)
            switch (key.Value)
            {
            case '1': Create(1, 1, null); break;
            case '2': Create(2, 1, null); break;
            case '3': Create(3, 1, null); break;
            case '4': Create(5, 1, null); break;
            case '6': Create(6, 1, null); break;
            case '7': Create(7, 1, null); break;
            case '8': Create(8, 1, null); break;
            case '9': Create(9, 1, null); break;
            }
            key = null;
        }
    }

    /// <summary>
    /// Fireworks are particles, with additional data for rendering and evolution.
    /// </summary>
    class Firework : Particle
    {
        /// <summary>
        /// Fireworks have an integer type, used for firework rules.
        /// </summary>
        public int Type;

        /// <summary>
        /// The age of a firework determines when it detonates. Age gradually
        /// decreases, when it passes zero the firework delivers its payload.
        /// Think of age as fuse-left.
        /// </summary>
        public float Age;

        /// <summary>
        /// Updates the firework by the given duration of time. Returns true
        /// if the firework has reached the end of its life and needs to be
        /// removed.
        /// </summary>
        public bool Update(float duration)
        {
            // Update our physical state
            Integrate(duration);

            // We work backwards from our age to zero.
            Age -= duration;
            return (Age < 0) || (Position.Y < 0);
        }
    }

    /// <summary>
    /// Firework rules control the length of a firework's fuse and the
    /// particles it should evolve into.
    /// </summary>
    public class FireworkRule
    {
        static readonly Random random = new Random();

        public int Type;
        public float MinAge;
        public float MaxAge;
        public Vector3 MinVelocity;
        public Vector3 MaxVelocity;
        public float Damping;
        public Payload[] Payloads = { };

        /// <summary>
        /// The payload is the new firework type to create when this
        /// firework's fuse is over.
        /// </summary>
        public class Payload
        {
            /// <summary>
            /// The type of the new particle to create.
            /// </summary>
            public int Type;

            /// <summary>
            /// The number of particles in this payload.
            /// </summary>
            public int Count;

            public Payload(int type, int count)
            {
                Type = type;
                Count = count;
            }
        }

        /// <summary>
        //  Creates a new firework of this type. The optional parent firework is used to base
        /// and velocity on.
        /// </summary>
        internal Firework Create(Firework parent)
        {
            var firework = new Firework
            {
                Type = Type,
                Age = random.NextFloat(MinAge, MaxAge),
                Mass = 1,
                Damping = Damping,
                Acceleration = new Vector3(0, -9.81f, 0),
                Velocity = random.RandomVector3(MinVelocity, MaxVelocity)
            };

            if (parent != null) {
                // The position and velocity are based on the parent.
                firework.Position = parent.Position;
                firework.Velocity += parent.Velocity;
            }
            else
            {
                int x = random.Next(3) - 1;
                firework.Position = new Vector3(5.0f * x, 0, 0);
            }

            firework.ClearAccumulator();
            return firework;
        }
    }
}
