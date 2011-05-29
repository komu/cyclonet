using System;
using System.Collections.Generic;
using CycloNet.Physics.Particles;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Tao.FreeGlut;

namespace CycloNet.Physics.Demos
{
    public class MassAggregateApplication : DemoApplication
    {
        protected readonly ParticleWorld World;
        protected readonly GroundContacts GroundContactGenerator;

        protected List<Particle> Particles
        {
            get { return World.Particles; }
        }

        public MassAggregateApplication(int particleCount)
        {
            World = new ParticleWorld(particleCount * 10);

            for (int i = 0; i < particleCount; i++)
                World.Particles.Add(new Particle());

            GroundContactGenerator = new GroundContacts(World.Particles);
            World.ContactGenerators.Add(GroundContactGenerator);
        }

        protected override void DoRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var camera = Matrix4.LookAt(0.0f, 3.5f, 8.0f,  0.0f, 3.5f, 0.0f,  0.0f, 1.0f, 0.0f);
            GL.LoadMatrix(ref camera);

            GL.Color4(Color4.Black);

            foreach (var particle in World.Particles)
            {
                GL.PushMatrix();
                GL.Translate(particle.Position);
                Glut.glutSolidSphere(0.1f, 20, 10);
                GL.PopMatrix();
            }
        }

        protected override void DoUpdate(float elapsed)
        {
            World.StartFrame();
            World.RunPhysics(elapsed);
        }
    }
}

