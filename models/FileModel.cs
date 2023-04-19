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

        public IldaPoint? SelectedPoint
        {
            get => _selectedpoint;
            set
            {
                _selectedpoint = value;
                OnPropertyChanged(nameof(SelectedPoint));
            }
        }
        private IldaPoint? _selectedpoint;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                this.SelectedPoint = null;
                _selectedIndex = Math.Min(this.Count - 1, Math.Max(0, value));
                OnPropertyChanged(nameof(SelectedIndex));
                OnPropertyChanged(nameof(SelectedFrame));
            }
        }
        private int _selectedIndex = 0;

        public string Location
        {
            get => File.Location;
        }

        public int Count => ((ICollection<IldaFrame>)File).Count;

        public bool IsReadOnly => ((ICollection<IldaFrame>)File).IsReadOnly;

        private int displayList { get; set; }

        public FileModel()
        {
            this.File = new IldaFile();
            this.SelectedIndex = 0;
        }
        public FileModel(IldaFile file)
        {
            File = file;
            SelectedIndex = 0;
        }

        private IldaFile File { get; }

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
                IldaPoint p1 = frame[i - 1];
                IldaPoint p2 = frame[i];
                if ((p1.IsBlanked != true && p2.IsBlanked != true)
                    || Properties.Settings.Default.show_blanked == true)
                {
                    IldPointSet(p1, frame.IldaVersion);
                    IldPointSet(p2, frame.IldaVersion);
                }
            }

            GL.End();

            if (Properties.Settings.Default.point_show == true)
            {
                GL.PointSize(Properties.Settings.Default.point_size);
                GL.Begin(PrimitiveType.Points);
                
                for (int i = 1; i < frame.Count; i += 1)
                {
                    if (frame[i].IsBlanked != true || Properties.Settings.Default.show_blanked == true)
                    IldPointSet(frame[i], frame.IldaVersion);
                }
                GL.End();
            }
            if (this.SelectedPoint != null)
            {
                GL.PointSize(Properties.Settings.Default.point_size * 2);
                GL.Begin(PrimitiveType.Points);
                IldPointSet(this.SelectedPoint, frame.IldaVersion);
                GL.End();
            }


            GL.Ortho(-1, 1, -1, 1, -1, 1);

            GL.EndList();

            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(Color.FromArgb(200, Color.Black));
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.CallList(this.displayList);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void IldPointSet(IldaPoint ildaPoint, int ildversion)
        {
            GL.Disable(EnableCap.Lighting);
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

        internal IldaColor GetPalette(byte pindex)
        {
            if (this.File != null && pindex < this.File.Palette.Count)
            {
                return this.File.Palette[pindex];
            }
            return new IldaColor(255,255,255);
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
