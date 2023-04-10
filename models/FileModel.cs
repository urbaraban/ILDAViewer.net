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
                _selectedIndex = Math.Min(this.Count - 1, Math.Max(0, value));
                OnPropertyChanged(nameof(SelectedIndex));
                OnPropertyChanged(nameof(SelectedFrame));
            }
        }
        private int _selectedIndex = 0;

        public IldaFile File { get; }

        public int Count => ((ICollection<IldaFrame>)File).Count;

        public bool IsReadOnly => ((ICollection<IldaFrame>)File).IsReadOnly;

        private int displayList { get; set; }

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
            int size = (int)Math.Min(width, height);
            
            GL.Viewport(0, 0, size, size);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1, 1, -1, 1, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void DrawFrame(IldaFrame frame)
        {
            GL.DeleteLists(this.displayList, 1);
            this.displayList = GL.GenLists(1);
            GL.NewList(this.displayList, ListMode.Compile);

            GL.Begin(PrimitiveType.Lines);
            GL.LineWidth(Properties.Settings.Default.line_width);

            for (int i = 1; i < frame.Count; i += 1)
            {
                ildPointSet(frame[i - 1], frame.IldaVersion);

                ildPointSet(frame[i], frame.IldaVersion);
            }

            GL.End();

            if (Properties.Settings.Default.point_show == true)
            {
                GL.Begin(PrimitiveType.Points);
                GL.PointSize(Properties.Settings.Default.point_size);
                for (int i = 1; i < frame.Count; i += 1)
                {
                    ildPointSet(frame[i], frame.IldaVersion);
                }
                GL.End();
            }


            GL.Ortho(-1, 1, -1, 1, -1, 1);

            GL.EndList();

            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(Color.FromArgb(200, Color.Black));
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            //this.angle += 1f;
            //GL.Rotate(this.angle, Vector3.UnitZ);
            //GL.Rotate(this.angle, Vector3.UnitY);
            //GL.Rotate(this.angle, Vector3.UnitX);
            //GL.Translate(0.5f, 0, 0);

            GL.CallList(this.displayList);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void ildPointSet(IldaPoint ildaPoint, int ildversion)
        {
            IldaColor color = GetColor(ildaPoint, ildversion);
            GL.Color3(color.R, color.G, color.B);
            Vector3 position = GetVector(ildaPoint);
            GL.Vertex3(position);
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
                point.X / (float)short.MaxValue,
                point.Y / (float)short.MaxValue,
                point.Z / (float)short.MaxValue);
        }

        public IldaColor GetColor(IldaPoint ildaPoint, int v)
        {
            IldaColor? ildaColor = null;

            if (ildaPoint.IsBlanked == false && this.File.Palette != null)
            {
                if (v < 2 && ildaPoint.PalIndex < this.File.Palette.Count)
                {
                    ildaColor = this.File.Palette[ildaPoint.PalIndex];
                }
                else if (ildaPoint.Color != null)
                {
                    ildaColor = ildaPoint.Color;
                }
            } else
            {
                ildaColor = new IldaColor(50, 50, 50);
            }

            if (ildaColor == null)
            {
                ildaColor = new IldaColor(255, 255, 255);
            }

            return ildaColor;
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
    }
}
