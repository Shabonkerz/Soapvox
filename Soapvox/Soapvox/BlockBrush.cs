using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

namespace Sandvox
{
    class BlockBrush
    {
        private Vector3 size;
        private System.Drawing.Color color;

        public delegate void OnSizeChangeHandler(object o, EventArgs e);
        public delegate void OnColorChangeHandler(object o, EventArgs e);

        public event OnSizeChangeHandler SizeChange;
        public event OnColorChangeHandler ColorChange;

        public Vector3 Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value.X < 1) value.X = 1;
                if (value.Y < 1) value.Y = 1;
                if (value.Z < 1) value.Z = 1;
                size = value;
                OnSizeChange(new EventArgs());
            }
        }
        public System.Drawing.Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                OnColorChange(new EventArgs());
            }
        }

        public void OnSizeChange(EventArgs e)
        {
            if (SizeChange != null)
                SizeChange(new object(), e);
        }

        public void OnColorChange(EventArgs e)
        {
            if (ColorChange != null)
                ColorChange(new object(), e);
        }

        public BlockBrush( Vector3 size, System.Drawing.Color color )
        {
            this.size = size;
            this.color = color;
        }

    }
}
