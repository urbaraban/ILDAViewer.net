using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ILDA.net;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ILDAViewer.net.models
{
    internal class FileModel : NotifyModel, ICollection<IldaFrame>
    {
        public IldaFrame? SelectedFrame
        {
            get
            {
                if (this.File.Count > 0)
                {
                    return this.File[SelectedIndex];
                }
                return null;
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
                OnPropertyChanged(nameof(SelectedFrame));
            }
        }
        private int _selectedIndex = 0;

        public IldaFile File { get; }

        public int Count => ((ICollection<IldaFrame>)File).Count;

        public bool IsReadOnly => ((ICollection<IldaFrame>)File).IsReadOnly;

        public FileModel(IldaFile file)
        {
            File = file;
            SelectedIndex = 0;
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
            float halfWidth = 0.5f;
            float halfHeight = 0.5f;
            GL.Ortho(-halfWidth, halfWidth, -halfHeight, halfHeight, short.MaxValue, -short.MaxValue);
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
            GL.Viewport(0, 0, 500, 500);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 500, 0, 500, -1, 1);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.ClearColor(Color.FromArgb(200, Color.DarkGray));
            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(100.0f, 100.0f, 0.0f); // origin of the line
            GL.Vertex3(200.0f, 140.0f, 5.0f); // ending point of the line
            GL.End();

            GL.Flush();
        }

        public Vector3[] GetVectors(IldaFrame frame)
        {
            Vector3[] ret = new Vector3[frame.Count];
            for (int i = 0; i < frame.Count; i += 1)
            {
                ret[i] = new Vector3(frame[i].X, frame[i].Y, frame[i].Z);
            }
            return ret;
        }

        public Vector3 GetVector(IldaPoint point)
        {
            return new Vector3(
                point.X / 32767.0f,
                point.Y / 32767.0f,
                point.Z / 32767.0f);
        }

        public IldaColor GetColor(IldaPoint ildaPoint, int v)
        {
            if (ildaPoint.IsBlanked == false)
            {
                if (v < 2 && this.File.Palette != null &&
                    ildaPoint.PalIndex > 0 && ildaPoint.PalIndex < this.File.Palette.Count)
                {
                    return this.File.Palette[ildaPoint.PalIndex];
                }
                else if (ildaPoint.Color != null)
                {
                    return ildaPoint.Color;
                }
            }

            return new IldaColor(50, 50, 50);
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
