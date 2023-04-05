using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using ILDA.net;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ILDAViewer.net.models
{
    internal class FileModel : NotifyModel, ICollection<IldaFrame>
    {
        public IldaFrame? SelectedFrame { get; set; }

        public IldaFile File { get; }

        public int Count => ((ICollection<IldaFrame>)File).Count;

        public bool IsReadOnly => ((ICollection<IldaFrame>)File).IsReadOnly;

        public FileModel(IldaFile file)
        {
            File = file;
            if (file.Count > 0)
            {
                SelectedFrame = file[0];
            }

        }

        /// <summary>
        /// re-initialize size related staffs
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(double width, double height)
        {
            this.size = new Size((int)width, (int)height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            float halfWidth = (float)(this.size.Width / 2);
            float halfHeight = (float)(this.size.Height / 2);
            GL.Ortho(-halfWidth, halfWidth, -halfHeight, halfHeight, 1000, -1000);
            GL.Viewport(0, 0, (int)this.size.Width, (int)this.size.Height);
        }

        /// <summary>
        /// render a scene , same as OpenGLviaFramebuffer
        /// it's better to make animation depending on the time, not the interval nor assuming frame rate
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Render(TimeSpan time)
        {
            if (this.displayList <= 0)
            {
                this.displayList = GL.GenLists(1);
                GL.NewList(this.displayList, ListMode.Compile);

                GL.Begin(PrimitiveType.Points);

                float factor = 0.2f;
                Random rnd = new Random();
                for (int i = 0; i < 100; i++)
                {
                    GL.Color3((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));

                    Vector3 position = new Vector3(
                        rnd.Next(-1000, 1000) * factor,
                        rnd.Next(-1000, 1000) * factor,
                        rnd.Next(-1000, 1000) * factor);
                    GL.Vertex3(position);

                    position.Normalize();
                    GL.Normal3(position);
                }
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                Vector3 v1 = new Vector3(rnd.Next(-1000, 1000),
                        rnd.Next(-1000, 1000) * factor,
                        rnd.Next(-1000, 1000) * factor);
                Vector3 v2 = new Vector3(rnd.Next(-1000, 1000),
                        rnd.Next(-1000, 1000) * factor,
                        rnd.Next(-1000, 1000) * factor);
                GL.Vertex3(v1);
                GL.Vertex3(v2);

                GL.End();

                GL.EndList();
            }

            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(Color.FromArgb(200, Color.DarkGray));
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            float halfWidth = this.size.Width * 0.5f;
            this.angle += 1f;
            GL.Rotate(this.angle, Vector3.UnitZ);
            GL.Rotate(this.angle, Vector3.UnitY);
            GL.Rotate(this.angle, Vector3.UnitX);
            GL.Translate(0.5f, 0, 0);

            GL.CallList(this.displayList);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            return true; // continue animation
        }

        public void DrawFrame(IldaFrame frame)
        {
            GL.DeleteLists(this.displayList, 1);
            this.displayList = GL.GenLists(1);
            GL.NewList(this.displayList, ListMode.Compile);

            GL.Begin(PrimitiveType.Points);

            for (int i = 0; i < frame.Count; i += 1)
            {
                IldaColor color;
                if(frame.IldaVersion < 2)
                {
                    color = File.Palette[frame[i].PalIndex];
                }
                else
                {
                    color = frame[i].Color;
                }
                GL.Color3(color.R, color.G, color.B);

                float halfWidth = this.size.Width * 0.5f;

                Vector3 position = new Vector3(
                    frame[i].X / (float)short.MaxValue * this.size.Width,
                    frame[i].Y / (float)short.MaxValue * this.size.Height,
                    frame[i].Z / (float)short.MaxValue);
                GL.Vertex3(position);

                position.Normalize();
                GL.Normal3(position);
            }

            GL.End();
            GL.EndList();

            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(Color.FromArgb(200, Color.DarkGray));
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            this.angle += 1f;
            GL.Rotate(this.angle, Vector3.UnitZ);
            GL.Rotate(this.angle, Vector3.UnitY);
            GL.Rotate(this.angle, Vector3.UnitX);
            GL.Translate(0.5f, 0, 0);

            GL.CallList(this.displayList);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        public void DrawLines(IldaFrame frame, int frameindex)
        {
            for (int i = 0; i < frame.Count; i += 1)
            {
                IldaColor color;
                if (frame.IldaVersion < 2)
                {
                    color = File.Palette[frame[i].PalIndex];
                }
                else
                {
                    color = frame[i].Color;
                }
                GL.Color3(color.R, color.G, color.B);

                Vector3 position = new Vector3(
                    frame[i].X / (float)short.MaxValue * this.size.Width,
                    frame[i].Y / (float)short.MaxValue * this.size.Height,
                    frame[i].Z / (float)short.MaxValue);
                GL.Vertex3(position);

                position.Normalize();
                GL.Normal3(position);
            }
        }



        public void Add(IldaFrame item)
        {
            ((ICollection<IldaFrame>)File).Add(item);
        }

        public void Clear()
        {
            ((ICollection<IldaFrame>)File).Clear();
        }

        public bool Contains(IldaFrame item)
        {
            return ((ICollection<IldaFrame>)File).Contains(item);
        }

        public void CopyTo(IldaFrame[] array, int arrayIndex)
        {
            ((ICollection<IldaFrame>)File).CopyTo(array, arrayIndex);
        }

        public bool Remove(IldaFrame item)
        {
            return ((ICollection<IldaFrame>)File).Remove(item);
        }

        public IEnumerator<IldaFrame> GetEnumerator()
        {
            return ((IEnumerable<IldaFrame>)File).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)File).GetEnumerator();
        }

        #region Fields

        private float angle;

        private int displayList;

        private Size size;
        #endregion
    }
}
