using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Tao.FreeGlut;
using CycloNet.Physics.Particles;

namespace CycloNet.Physics.Demos.Ballistic
{
    enum ShotType
    {
        Pistol, Artillery, Fireball, Laser
    };

    /// <summary>
    /// Holds a single ammunition round record.
    /// </summary>
    class AmmoRound
    {
        public readonly Particle Particle = new Particle();
        public readonly ShotType type;
        public DateTime StartTime;

        public AmmoRound(ShotType type)
        {
            this.type = type;

            switch (type)
            {
            case ShotType.Pistol:
                Particle.Mass = 2.0f; // 2.0kg
                Particle.Velocity = new Vector3(0.0f, 0.0f, 35.0f); // 35m/s
                Particle.Acceleration = new Vector3(0, -1, 0);
                Particle.Damping = 0.99f;
                break;

            case ShotType.Artillery:
                Particle.Mass = 200.0f; // 200.0kg
                Particle.Velocity = new Vector3(0.0f, 30.0f, 40.0f); // 50m/s
                Particle.Acceleration = new Vector3(0, -20, 0);
                Particle.Damping = 0.99f;
                break;

            case ShotType.Fireball:
                Particle.Mass = 1.0f; // 1.0kg - mostly blast damage
                Particle.Velocity = new Vector3(0.0f, 0.0f, 10.0f); // 5m/s
                Particle.Acceleration = new Vector3(0, 0.6f, 0); // floats up
                Particle.Damping = 0.9f;
                break;

            case ShotType.Laser:
                // Note that this is the kind of laser bolt seen in films,
                // not a realistic laser beam!
                Particle.Mass = 0.1f;  // 0.1kg - almost no weight
                Particle.Velocity = new Vector3(0.0f, 0.0f, 100.0f); // 100m/s
                Particle.Acceleration = new Vector3(0.0f, 0.0f, 0.0f); // No gravity
                Particle.Damping = 0.99f;
                break;
            }

            // Set the data common to all particle types
            Particle.Position = new Vector3(0.0f, 1.5f, 0.0f);
            StartTime = TimingData.LastFrameTimestamp;

            // Clear the force accumulators
            Particle.ClearAccumulator();
        }

        public void Render()
        {
            var position = Particle.Position;

            GL.Color4(Color4.Black);
            GL.PushMatrix();
            GL.Translate(position);

            Glut.glutSolidSphere(0.3f, 5, 4);

            GL.PopMatrix();

            GL.Color4(new Color4(0.75f, 0.75f, 0.75f, 1.0f));
            GL.PushMatrix();

            GL.Translate(position.X, 0, position.Z);
            GL.Scale(1, 0.1f, 1);

            Glut.glutSolidSphere(0.6f, 5, 4);

            GL.PopMatrix();
        }
    };

    public class BallisticDemo : DemoApplication
    {
        List<AmmoRound> ammo = new List<AmmoRound>();

        public BallisticDemo()
        {
            Title = "CycloNet > Ballistic Demo";
        }

        void Fire(ShotType shotType)
        {
            if (ammo.Count < 64)
                ammo.Add(new AmmoRound(shotType));
        }

        protected override void DoUpdate(float duration)
        {
            // Update the physics of each particle in turn
            foreach (var shot in ammo)
            {
                // Run the physics
                shot.Particle.Integrate(duration);
            }

            // Check if the particle is now invalid
            ammo.RemoveAll(shot =>
                    shot.Particle.Position.Y < 0.0f ||
                    shot.Particle.Position.Z > 200f ||
                    false //shot.StartTime+5000 < TimingData::get().lastFrameTimestamp);
                               );
        }

        protected override void DoRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            var camera = Matrix4.LookAt(-25.0f, 8.0f, 5.0f,
                                        0.0f, 5.0f, 22.0f,
                                        0.0f, 1.0f, 0.0f);
            GL.LoadMatrix(ref camera);

            // Draw a sphere at the firing point, and add a shadow projected
            // onto the ground plane.
            GL.Color4(Color4.Black);
            GL.PushMatrix();

            GL.Translate(0.0f, 1.5f, 0.0f);
            Glut.glutSolidSphere(0.1f, 5, 5);
            GL.Translate(0.0f, -1.5f, 0.0f);
            GL.Color3(0.75f, 0.75f, 0.75f);
            GL.Scale(1.0f, 0.1f, 1.0f);
            Glut.glutSolidSphere(0.1f, 5, 5);
            GL.PopMatrix();


            // Draw some scale lines
            GL.Color3(0.75f, 0.75f, 0.75f);
            GL.Begin(BeginMode.Lines);
            for (int i = 0; i < 200; i += 10)
            {
                GL.Vertex3(-5.0f, 0.0f, i);
                GL.Vertex3(5.0f, 0.0f, i);
            }
            GL.End();

            // Render each particle in turn
            foreach (var shot in ammo)
                shot.Render();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
            case '1':
                Fire(ShotType.Pistol);
                break;
            case '2':
                Fire(ShotType.Artillery);
                break;
            case '3':
                Fire(ShotType.Fireball);
                break;
            case '4':
                Fire(ShotType.Laser);
                break;
            }
        }
    }
}
