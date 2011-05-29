using System;
using System.Linq;
using System.Collections.Generic;
using CycloNet.Physics.Particles;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Tao.FreeGlut;

namespace CycloNet.Physics.Demos.Bridge
{
    public class BridgeDemo : MassAggregateApplication
    {
        const int RodCount = 6;
        const int CableCount = 10;
        const int SupportCount = 12;
        const float BaseMass = 1;
        const float ExtraMass = 10;

        List<ParticleCableConstraint> supports = new List<ParticleCableConstraint>(SupportCount);
        List<ParticleCable> cables = new List<ParticleCable>(CableCount);
        List<ParticleRod> rods = new List<ParticleRod>(RodCount);

        Vector3 MassPos = new Vector3(0, 0, 0.5f);
        Vector3 MassDisplayPos;

        public BridgeDemo():
            base(12)
        {
            Title = "CycloNet - Bridge Demo";

            // Create masses
            for (int i = 0; i < Particles.Count; i++)
            {
                var p = Particles[i];

                p.Position = new Vector3((i/2)*2.0f-5.0f, 4, (i%2)*2.0f-1.0f);
                p.Velocity = Vector3.Zero;
                p.Damping = 0.9f;
                p.Acceleration = Gravity;
                p.ClearAccumulator();
            }

            // Add the links
            for (int i = 0; i < CableCount; i++)
            {
                cables.Add(new ParticleCable(Particles[i], Particles[i+2])
                {
                    MaxLength = 1.9f,
                    Restitution = 0.3f
                });
            }

            for (int i = 0; i < SupportCount; i++)
            {
                var anchor = new Vector3((i/2)*2.2f-5.5f, 6, (i%2)*1.6f-0.8f);
                supports.Add(new ParticleCableConstraint(Particles[i], anchor)
                {
                    MaxLength = (i <  6) ? ((i/2)*0.5f + 3.0f) : (5.5f - (i/2)*0.5f),
                    Restitution = 0.5f
                });
            }

            for (int i = 0; i < RodCount; i++)
            {
                rods.Add(new ParticleRod(Particles[i*2], Particles[i*2+1])
                {
                    Length = 2
                });
            }

            World.ContactGenerators.AddRange(cables);
            World.ContactGenerators.AddRange(supports);
            World.ContactGenerators.AddRange(rods);

            UpdateAdditionalMass();
        }

        private void UpdateAdditionalMass()
        {
            foreach (var p in Particles)
                p.Mass = BaseMass;

            // Find the coordinates of the mass as an index and proportion
            var x = (int) MassPos.X;
            var xp = MassPos.X % 1.0f;
            if (x < 0)
            {
                x = 0;
                xp = 0;
            }
            if (x >= 5)
            {
                x = 5;
                xp = 0;
            }

            var z = (int) MassPos.Z;
            var zp = MassPos.Z % 1.0f;
            if (z < 0)
            {
                z = 0;
                zp = 0;
            }
            if (z >= 1)
            {
                z = 1;
                zp = 0;
            }

            // Calculate where to draw the mass
            MassDisplayPos = Vector3.Zero;

            // Add the proportion to the correct masses
            Particles[x*2+z].Mass = BaseMass + ExtraMass*(1-xp)*(1-zp);

            MassDisplayPos += Particles[x*2+z].Position * ((1-xp)*(1-zp));

            if (xp > 0)
            {
                Particles[x*2+z+2].Mass = BaseMass + ExtraMass*xp*(1-zp);
                MassDisplayPos +=
                    Particles[x*2+z+2].Position * (xp*(1-zp));

                if (zp > 0)
                {
                    Particles[x*2+z+3].Mass = BaseMass + ExtraMass*xp*zp;
                    MassDisplayPos += Particles[x*2+z+3].Position * (xp*zp);
                }
            }
            if (zp > 0)
            {
                Particles[x*2+z+1].Mass = BaseMass + ExtraMass*(1-xp)*zp;
                MassDisplayPos += Particles[x*2+z+1].Position * ((1-xp)*zp);
            }
        }

        protected override void DoRender()
        {
            base.DoRender();

            GL.Begin(BeginMode.Lines);

            GL.Color3(0, 0, 1.0f);
            foreach (var rod in rods)
            {
                GL.Vertex3(rod.Particle0.Position);
                GL.Vertex3(rod.Particle1.Position);
            }

            GL.Color3(0, 1.0f, 0);
            foreach (var cable in cables)
            {
                GL.Vertex3(cable.Particle0.Position);
                GL.Vertex3(cable.Particle1.Position);
            }

            GL.Color3(0.7f, 0.7f, 0.7f);
            foreach (var support in supports)
            {
                GL.Vertex3(support.Particle.Position);
                GL.Vertex3(support.Anchor);
            }

            GL.End();

            GL.Color3(1.0f, 0, 0);
            GL.PushMatrix();
            GL.Translate(MassDisplayPos);

            Glut.glutSolidSphere(0.25f, 20, 10);
            GL.PopMatrix();
        }

        protected override void DoUpdate(float elapsed)
        {
            base.DoUpdate(elapsed);

            UpdateAdditionalMass();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
            case 's': case 'S':
                MassPos.Z += 0.1f;
                if (MassPos.Z > 1.0f) MassPos.Z = 1.0f;
                break;
            case 'w': case 'W':
                MassPos.Z -= 0.1f;
                if (MassPos.Z < 0.0f) MassPos.Z = 0.0f;
                break;
            case 'a': case 'A':
                MassPos.X -= 0.1f;
                if (MassPos.X < 0.0f) MassPos.X = 0.0f;
                break;
            case 'd': case 'D':
                MassPos.X += 0.1f;
                if (MassPos.X > 5.0f) MassPos.X = 5.0f;
                break;
            }
        }
    }
}

