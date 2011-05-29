using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace CycloNet.Physics.Demos
{
    public abstract class DemoApplication : GameWindow
    {
        public readonly Vector3 Gravity = new Vector3(0, -9.81f, 0);
        
        public DemoApplication():
            base(800, 600)
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(new Color4(0.9f, 0.95f, 1.0f, 1.0f));
            GL.Enable(EnableCap.DepthTest);
            GL.ShadeModel(ShadingModel.Smooth);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);

            var aspect = (float) Width / (float) Height;

            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), aspect, 1.0f, 500.0f);
            GL.LoadMatrix(ref projection);

            GL.MatrixMode(MatrixMode.Modelview);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            DoRender();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[Key.Escape])
            {
                Exit();
                return;
            }

            TimingData.Update();

            DoUpdate((float) e.Time);
        }

        protected virtual void DoUpdate(float elapsed)
        {
        }

        protected virtual void DoRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.Begin(BeginMode.Lines);
            GL.Vertex2(1, 1);
            GL.Vertex2(639, 319);
            GL.End();
        }
    }
}

