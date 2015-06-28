using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sandvox;

namespace Sandvox
{
    /// <summary>
    /// Describes a three-dimensional volume in space.
    /// </summary>
    public class Volume
    {
        /// <summary>
        /// Describes the size of the volume in three dimensions.
        /// </summary>
        protected Vector3 m_size;

        /// <summary>
        /// Contains the halfway coordinate. Used in determining overflow and underflow for adding volumes.
        /// </summary>
        public Vector3 Half;

        /// <summary>
        /// THe location of the volume.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The color of the volume.
        /// </summary>
        public Color color;

        /// <summary>
        /// Flag for whether the volume is collapsed or not.
        /// </summary>
        public bool Visible = false;

        /// <summary>
        /// The middle of the volume corresponding to the space that it is in.
        /// </summary>
        public Vector3 Middle;

        /// <summary>
        /// Setter/Getter for m_size
        /// </summary>
        public Vector3 Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Volume()
        {
        }
        public bool Equals(Volume a)
        {
            return this == a;
        }
        /// <summary>
        /// Constructs a new volume.
        /// </summary>
        /// <param name="position">The coordinates of the volume.</param>
        /// <param name="size">The size of the volume.</param>
        /// <param name="color">The color of hte volume.</param>
        public Volume(Vector3 position, Vector3 size, ref Color color)
        {
            this.Size = size;
            this.Position = position;
            this.Half = size / 2;
            this.Half = new Vector3((int)this.Half.X, (int)this.Half.Y, (int)this.Half.Z);
            this.color = color;
        }

        /// <summary>
        /// Constructs a new volume.
        /// </summary>
        /// <param name="position">The coordinates of the volume.</param>
        /// <param name="size">The size of the volume.</param>
        /// <param name="color">The color of hte volume.</param>
        public Volume(Vector3 position, Vector3 size, Color color)
        {
            this.Size = size;
            this.Position = position;
            this.Half = size / 2;
            this.Half = new Vector3((int)this.Half.X, (int)this.Half.Y, (int)this.Half.Z);
            this.color = color;
        }

        /// <summary>
        /// Creates a shallow copy of the volume.
        /// </summary>
        /// <param name="input">The volume to copy.</param>
        public void MemberwiseClone(Volume input)
        {
            // Copy references to members.
            this.Size = input.Size;
            this.Position = input.Position;
            this.color = input.color;

            // Calculate derivative variables.
            this.Half = Size / 2;
            this.Half = new Vector3((int)this.Half.X, (int)this.Half.Y, (int)this.Half.Z);
            this.Middle.X = this.Half.X + this.Position.X;
            this.Middle.Y = this.Half.Y + this.Position.Y;
            this.Middle.Z = this.Half.Z + this.Position.Z;
            this.Middle = new Vector3((int)this.Middle.X, (int)this.Middle.Y, (int)this.Middle.Z);

        }


        /// <summary>
        /// Spits out size and color info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Size(" + m_size.X + "," + m_size.Y + "," + m_size.Z + "), Color(" + color.R + "," + color.G + "," + color.B + ")";
        }

        /// <summary>
        /// Makes volume visible upon collapsing.
        /// </summary>
        public void Collapse()
        {
            this.Visible = true;
        }

        /// <summary>
        /// Makes volume invisible, and the children visible.
        /// </summary>
        public void Expand()
        {
            this.Visible = false;

        }

        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public Vector3 Translate(Vector3 translation)
        {
            return Position + translation;
        }

        /// <summary>
        /// Handles the data of the node prior to removal. In this case, the vertex data is removed prior to the node's removal.
        /// </summary>
        /// <param name="node">The node that is about to be removed.</param>
        public static void RemoveAllHandler(ref Octree<Volume>.OctreeNode node)
        {
            if (node.BottomLeftBack != null)
                RemoveAllHandler(ref node.BottomLeftBack);
            if (node.BottomRightBack != null)
                RemoveAllHandler(ref node.BottomRightBack);
            if (node.BottomLeftFront != null)
                RemoveAllHandler(ref node.BottomLeftFront);
            if (node.BottomRightFront != null)
                RemoveAllHandler(ref node.BottomRightFront);
            if (node.TopLeftBack != null)
                RemoveAllHandler(ref node.TopLeftBack);
            if (node.TopLeftFront != null)
                RemoveAllHandler(ref node.TopLeftFront);
            if (node.TopRightBack != null)
                RemoveAllHandler(ref node.TopRightBack);
            if (node.TopRightFront != null)
                RemoveAllHandler(ref node.TopRightFront);

            node.data.Remove();
        }
        public void Remove()
        {
            this.Visible = false;
        }
        /// <summary>
        /// Handles the removal of a node from an Octree.
        /// </summary>
        /// <param name="node">The Octree node to start the search from.</param>
        /// <param name="position">The location of the volume to remove.</param>
        public static void RemoveHandler(ref Octree<Volume>.OctreeNode node, Volume child)
        {


            if (node == null)
                return;

            Volume parent = node.data;
            //Console.WriteLine("Adding volume: " + child.ToString() + " inside of " + parent.ToString());
            parent.Middle.X = (int)(parent.Half.X + parent.Position.X);
            parent.Middle.Y = (int)(parent.Half.Y + parent.Position.Y);
            parent.Middle.Z = (int)(parent.Half.Z + parent.Position.Z);

            #region // Stop condition

            if (parent.Size.X < 2 && parent.Size.Y < 2 && parent.Size.Z < 2 && parent.color == child.color)
            {

                //parent.Remove();

                parent.Visible = false;

                return;
            }
            #endregion

            #region // Overflow calculation.

            Vector3 Local = Vector3.Zero;
            Vector3 Overflow = Vector3.Zero;
            Vector3 Underflow = Vector3.Zero;

            // Determine the relative position from the parent. Needed for underflow/overflow calculations.
            Local.X = child.Position.X - parent.Position.X;
            Local.Y = child.Position.Y - parent.Position.Y;
            Local.Z = child.Position.Z - parent.Position.Z;

            // Calculate overflow and underflow along the x-axis.
            if ((child.Size.X + Local.X) - parent.Half.X > 0)
            {
                Underflow.X = (parent.Half.X - Local.X);
                Overflow.X = child.Size.X - Underflow.X;
            }

            // Calculate overflow and underflow along the y-axis.
            if ((child.Size.Y + Local.Y) - parent.Half.Y > 0)
            {
                Underflow.Y = (parent.Half.Y - Local.Y);
                Overflow.Y = child.Size.Y - Underflow.Y;
            }

            // Calculate overflow and underflow along the z-axis.
            if ((child.Size.Z + Local.Z) - parent.Half.Z > 0)
            {
                Underflow.Z = (parent.Half.Z - Local.Z);
                Overflow.Z = child.Size.Z - Underflow.Z;
            }
            #endregion

            #region Recursive overflow logic
            if (Local.X >= parent.Half.X)
            {
                if (Local.Y >= parent.Half.Y)
                {
                    if (Local.Z >= parent.Half.Z)
                    {
                        // Octant #8 has no possible overflow.
                        if (node.TopRightFront != null)
                        {
                            RemoveHandler(ref node.TopRightFront, new Volume(child.Position, child.Size, ref child.color));
                            node.TopRightFront = null;
                        }
                    }
                    else
                    {
                        // Test for overflow into the z direction.
                        if (Overflow.Z > 0)
                        {
                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new Volume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.TopRightBack = null;
                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));

                                node.TopRightFront = null;
                            }
                        }
                        else
                        {
                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new Volume(child.Position, child.Size, ref child.color));

                                node.TopRightBack = null;
                            }
                        }
                    }
                }
                else
                {
                    if (Local.Z >= parent.Half.Z)
                    {
                        // Test for overflow in the Y direction.
                        if (Overflow.Y > 0)
                        {
                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));

                                node.BottomRightFront = null;
                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else
                        {
                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new Volume(child.Position, child.Size, ref child.color));
                                node.BottomRightFront = null;
                            }
                        }

                    }
                    else
                    {
                        // Test for both cases.

                        if (Overflow.Y > 0 && Overflow.Z > 0)
                        {
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, Underflow.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));
                                node.TopRightBack = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, Underflow.Y, Overflow.Z), ref child.color));
                                node.BottomRightFront = null;

                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(child.Size.X, Overflow.Y, Overflow.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else if (Overflow.Y > 0)
                        {
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopRightBack = null;
                            }
                        }
                        else if (Overflow.Z > 0)
                        {
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new Volume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.BottomRightFront = null;
                            }
                        }
                        else
                        {
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new Volume(child.Position, child.Size, ref child.color));
                                node.BottomRightBack = null;
                            }
                        }
                    }
                }
            }
            else
            {
                if (Local.Y >= parent.Half.Y)
                {
                    if (Local.Z >= parent.Half.Z)
                    {

                        // Test for overflow into the X direction.
                        if (Overflow.X > 0)
                        {
                            if (node.TopLeftFront == null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                            if (node.TopRightFront == null)
                            {
                                RemoveHandler(ref node.TopRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else
                        {
                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new Volume(child.Position, child.Size, ref child.color));
                                node.TopLeftBack = null;
                            }
                        }
                    }

                    else
                    {
                        // Test for three cases.

                        if (Overflow.X > 0 && Overflow.Z > 0)
                        {
                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.TopRightBack = null;
                            }

                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else if (Overflow.X > 0)
                        {
                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.TopRightBack = null;
                            }
                        }
                        else if (Overflow.Z > 0)
                        {
                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new Volume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.TopLeftFront = null;
                            }
                        }
                        else
                        {
                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new Volume(child.Position, child.Size, ref child.color));
                                node.TopLeftBack = null;
                            }

                        }
                    }
                }
                else
                {
                    if (Local.Z >= parent.Half.Z)
                    {
                        // Test two cases.
                        if (Overflow.X > 0 && Overflow.Y > 0)
                        {
                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new Volume(child.Position, new Vector3(Underflow.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomRightFront = null;
                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else if (Overflow.X > 0)
                        {
                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomRightFront = null;
                            }
                        }
                        else if (Overflow.Y > 0)
                        {
                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopLeftFront = null;
                            }
                        }
                        else
                        {
                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new Volume(child.Position, child.Size, ref child.color));
                                node.BottomLeftFront = null;
                            }
                        }
                    }
                    else
                    {
                        // Test for both cases.
                        if (Overflow.X > 0 && Overflow.Y > 0 && Overflow.Z > 0)
                        {
                            if (node.BottomLeftBack != null)
                            {
                                RemoveHandler(ref node.BottomLeftBack, new Volume(child.Position, Underflow, ref child.color));
                                node.BottomLeftBack = null;
                            }
                            // Primaries
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, Underflow.Y, Underflow.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, Underflow.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, Underflow.Y, Overflow.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            // Secondaries
                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(Underflow.X, Overflow.Y, Overflow.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, Underflow.Z), ref child.color));
                                node.TopRightBack = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, Underflow.Y, Overflow.Z), ref child.color));
                                node.BottomRightFront = null;
                            }

                            // Tertiary
                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new Volume(parent.Middle, Overflow, ref child.color));
                                node.TopRightFront = null;
                            }

                        }
                        else if (Overflow.X > 0 && Overflow.Y > 0)
                        {

                            if (node.BottomLeftBack != null)
                            {
                                RemoveHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(Underflow.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }

                            // Primaries
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            // Secondaries
                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopRightBack = null;
                            }
                        }
                        else if (Overflow.Y > 0 && Overflow.Z > 0)
                        {

                            if (node.BottomLeftBack != null)
                            {
                                RemoveHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, Underflow.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }
                            // Primaries
                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, Underflow.Y, Overflow.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            // Secondaries
                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(child.Size.X, Overflow.Y, Overflow.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                        }
                        else if (Overflow.X > 0 && Overflow.Z > 0)
                        {
                            if (node.BottomLeftBack != null)
                            {
                                RemoveHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }

                            // Primaries

                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            // Secondaries
                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.BottomRightFront = null;
                            }

                        }
                        else if (Overflow.X > 0)
                        {
                            if (node.BottomLeftBack != null)
                            {
                                RemoveHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }

                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomRightBack = null;
                            }
                        }
                        else if (Overflow.Y > 0)
                        {

                            if (node.BottomLeftBack != null)
                            {
                                RemoveHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }
                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopLeftBack = null;
                            }
                        }
                        else if (Overflow.Z > 0)
                        {

                            if (node.BottomLeftBack != null)
                            {
                                RemoveHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }
                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }
                        }
                        else
                        {
                            if (node.BottomLeftBack != null)
                            {
                                RemoveHandler(ref node.BottomLeftBack, new Volume(child.Position, child.Size, ref child.color));
                                node.BottomLeftBack = null;
                            }
                        }

                    }
                }
            }
            #endregion


            //TryCollapse(ref node);

        }

        /// <summary>
        /// Handles the instantiation of the root node of an Octree.
        /// </summary>
        /// <param name="root">The root node to instantiate.</param>
        /// <param name="size">The maximum size of the Octree, which the root node will hold.</param>
        public static void SetRootHandler(ref Octree<Volume>.OctreeNode root, Volume volume)
        {
            Console.WriteLine("Volume: " + volume.Size);
            // Set root node!
            root = new Octree<Volume>.OctreeNode(new Volume(Vector3.Zero, volume.Size, ref volume.color));
        }

        /// <summary>
        /// Octree search based on volume. To be used with an instance of Octree.
        /// </summary>
        /// <param name="node">The OctreeNode to search from.</param>
        /// <param name="position">The location of the Volume being searched for.</param>
        /// <returns>A Volume at location provided.</returns>
        public static Volume SearchHandler(ref Octree<Volume>.OctreeNode node, Volume volume)
        {
            if (node.data.Size == volume.Size &&
                node.data.Position.X == volume.Position.X &&
                node.data.Position.Y == volume.Position.Y &&
                node.data.Position.Z == volume.Position.Z)
                return node.data;

            if (volume.Position.X < node.data.Middle.X)
            {
                if (volume.Position.Y < node.data.Middle.Y)
                {
                    if (volume.Position.Z < node.data.Middle.Z)
                    {
                        if (node.BottomLeftBack == null) return null;
                        return SearchHandler(ref node.BottomLeftBack, volume);
                    }
                    else
                    {
                        if (node.BottomLeftFront == null) return null;
                        return SearchHandler(ref node.BottomLeftFront, volume);
                    }
                }
                else
                {
                    if (volume.Position.Z < node.data.Middle.Z)
                    {
                        if (node.TopLeftBack == null) return null;
                        return SearchHandler(ref node.TopLeftBack, volume);
                    }
                    else
                    {
                        if (node.TopLeftFront == null) return null;
                        return SearchHandler(ref node.TopLeftFront, volume);
                    }
                }
            }
            else
            {
                if (volume.Position.Y < node.data.Middle.Y)
                {
                    if (volume.Position.Z < node.data.Middle.Z)
                    {
                        if (node.BottomRightBack == null) return null;
                        return SearchHandler(ref node.BottomRightBack, volume);
                    }
                    else
                    {
                        if (node.BottomRightFront == null) return null;
                        return SearchHandler(ref node.BottomRightFront, volume);
                    }
                }
                else
                {
                    if (volume.Position.Z < node.data.Middle.Z)
                    {
                        if (node.TopRightBack == null) return null;
                        return SearchHandler(ref node.TopRightBack, volume);
                    }
                    else
                    {
                        if (node.TopRightFront == null) return null;
                        return SearchHandler(ref node.TopRightFront, volume);
                    }
                }
            }
        }


        /// <summary>
        /// Inserts a WorldVolume of size one, or greater, into an Octree.
        /// </summary>
        /// <param name="node">The Octree to insert into.</param>
        /// <param name="child">The WorldVolume to add to the Octree.</param>
        public static void AddHandler(ref Octree<Volume>.OctreeNode node, Volume child)
        {


            if (node == null)
            {
                return;
            }

            Volume parent = node.data;

            parent.Middle.X = (int)(parent.Half.X + parent.Position.X);
            parent.Middle.Y = (int)(parent.Half.Y + parent.Position.Y);
            parent.Middle.Z = (int)(parent.Half.Z + parent.Position.Z);

            #region // Stop condition

            if (parent.Size.X < 2 && parent.Size.Y < 2 && parent.Size.Z < 2)
            {
                parent.Visible = true;

                return;
            }
            #endregion

            #region // Overflow calculation.

            Vector3 Local = Vector3.Zero;
            Vector3 Overflow = Vector3.Zero;
            Vector3 Underflow = Vector3.Zero;

            // Determine the relative position from the parent. Needed for underflow/overflow calculations.
            Local.X = child.Position.X - parent.Position.X;
            Local.Y = child.Position.Y - parent.Position.Y;
            Local.Z = child.Position.Z - parent.Position.Z;

            // Calculate overflow and underflow along the x-axis.
            if ((child.Size.X + Local.X) - parent.Half.X > 0)
            {
                Underflow.X = (parent.Half.X - Local.X);
                Overflow.X = child.Size.X - Underflow.X;
            }

            // Calculate overflow and underflow along the y-axis.
            if ((child.Size.Y + Local.Y) - parent.Half.Y > 0)
            {
                Underflow.Y = (parent.Half.Y - Local.Y);
                Overflow.Y = child.Size.Y - Underflow.Y;
            }

            // Calculate overflow and underflow along the z-axis.
            if ((child.Size.Z + Local.Z) - parent.Half.Z > 0)
            {
                Underflow.Z = (parent.Half.Z - Local.Z);
                Overflow.Z = child.Size.Z - Underflow.Z;
            }
            #endregion

            #region Recursive overflow logic
            if (Local.X >= parent.Half.X)
            {
                if (Local.Y >= parent.Half.Y)
                {
                    if (Local.Z >= parent.Half.Z)
                    {
                        // Octant #8 has no possible overflow.
                        if (node.TopRightFront == null)
                        {
                            node.TopRightFront = new Octree<Volume>.OctreeNode(new Volume(parent.Middle, parent.Half, ref child.color));
                        }
                        AddHandler(ref node.TopRightFront, new Volume(child.Position, child.Size, ref child.color));
                    }
                    else
                    {
                        // We know that the top, right, back octant has something in it.
                        if (node.TopRightBack == null)
                        {
                            node.TopRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                        }

                        // Test for overflow into the z direction.
                        if (Overflow.Z > 0)
                        {
                            AddHandler(ref node.TopRightBack, new Volume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<Volume>.OctreeNode(new Volume(parent.Middle, parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.TopRightBack, new Volume(child.Position, child.Size, ref child.color));
                        }
                    }
                }
                else
                {
                    if (Local.Z >= parent.Half.Z)
                    {
                        // We know that the bottom, right, front octant has something in it...
                        if (node.BottomRightFront == null)
                        {
                            node.BottomRightFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                        }

                        // Test for overflow in the Y direction.
                        if (Overflow.Y > 0)
                        {
                            AddHandler(ref node.BottomRightFront, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<Volume>.OctreeNode(new Volume(parent.Middle, parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.BottomRightFront, new Volume(child.Position, child.Size, ref child.color));
                        }

                    }
                    else
                    {
                        // We know that the bottom, right, back octant has something in it...
                        if (node.BottomRightBack == null)
                        {
                            node.BottomRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                        }

                        // Test for both cases.

                        if (Overflow.Y > 0 && Overflow.Z > 0)
                        {
                            AddHandler(ref node.BottomRightBack, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, Underflow.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, Underflow.Y, Overflow.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<Volume>.OctreeNode(new Volume(parent.Middle, parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(child.Size.X, Overflow.Y, Overflow.Z), ref child.color));
                        }
                        else if (Overflow.Y > 0)
                        {
                            AddHandler(ref node.BottomRightBack, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Z > 0)
                        {
                            AddHandler(ref node.BottomRightBack, new Volume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.BottomRightBack, new Volume(child.Position, child.Size, ref child.color));
                        }
                    }
                }
            }
            else
            {
                if (Local.Y >= parent.Half.Y)
                {
                    if (Local.Z >= parent.Half.Z)
                    {
                        if (node.TopLeftFront == null)
                        {
                            node.TopLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                        }


                        // Test for overflow into the X direction.
                        if (Overflow.X > 0)
                        {
                            AddHandler(ref node.TopLeftFront, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<Volume>.OctreeNode(new Volume(parent.Middle, parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.TopLeftFront, new Volume(child.Position, child.Size, ref child.color));
                        }
                    }

                    else
                    {

                        // Octant #8 has no possible overflow.
                        if (node.TopLeftBack == null)
                        {
                            node.TopLeftBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                        }

                        // Test for three cases.

                        if (Overflow.X > 0 && Overflow.Z > 0)
                        {
                            AddHandler(ref node.TopLeftBack, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, child.Size.Y, Overflow.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<Volume>.OctreeNode(new Volume(parent.Middle, parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else if (Overflow.X > 0)
                        {
                            AddHandler(ref node.TopLeftBack, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Z > 0)
                        {
                            AddHandler(ref node.TopLeftBack, new Volume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.TopLeftBack, new Volume(child.Position, child.Size, ref child.color));

                        }
                    }
                }
                else
                {
                    if (Local.Z >= parent.Half.Z)
                    {
                        // We know that the bottom, right, front octant has something in it...
                        if (node.BottomLeftFront == null)
                        {
                            node.BottomLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                        }

                        // Test two cases.
                        if (Overflow.X > 0 && Overflow.Y > 0)
                        {
                            AddHandler(ref node.BottomLeftFront, new Volume(child.Position, new Vector3(Underflow.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<Volume>.OctreeNode(new Volume(parent.Middle, parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightFront, new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.X > 0)
                        {
                            AddHandler(ref node.BottomLeftFront, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Y > 0)
                        {
                            AddHandler(ref node.BottomLeftFront, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.BottomLeftFront, new Volume(child.Position, child.Size, ref child.color));
                        }
                    }
                    else
                    {
                        // We know that the bottom, right, back octant has something in it...
                        if (node.BottomLeftBack == null)
                        {
                            node.BottomLeftBack = new Octree<Volume>.OctreeNode(new Volume(parent.Position, parent.Half, ref child.color));
                        }

                        // Test for both cases.
                        if (Overflow.X > 0 && Overflow.Y > 0 && Overflow.Z > 0)
                        {
                            AddHandler(ref node.BottomLeftBack, new Volume(child.Position, Underflow, ref child.color));
                            // Primaries
                            if (node.BottomRightBack == null)
                            {
                                node.BottomRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, Underflow.Y, Underflow.Z), ref child.color));

                            if (node.TopLeftBack == null)
                            {
                                node.TopLeftBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, Underflow.Z), ref child.color));

                            if (node.BottomLeftFront == null)
                            {
                                node.BottomLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, Underflow.Y, Overflow.Z), ref child.color));

                            // Secondaries
                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(Underflow.X, Overflow.Y, Overflow.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, Underflow.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, Underflow.Y, Overflow.Z), ref child.color));

                            // Tertiary
                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<Volume>.OctreeNode(new Volume(parent.Middle, parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightFront, new Volume(parent.Middle, Overflow, ref child.color));

                        }
                        else if (Overflow.X > 0 && Overflow.Y > 0)
                        {

                            AddHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(Underflow.X, Underflow.Y, child.Size.Z), ref child.color));

                            // Primaries
                            if (node.BottomRightBack == null)
                            {
                                node.BottomRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.TopLeftBack == null)
                            {
                                node.TopLeftBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));

                            // Secondaries
                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Y > 0 && Overflow.Z > 0)
                        {

                            AddHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, Underflow.Z), ref child.color));
                            // Primaries
                            if (node.TopLeftBack == null)
                            {
                                node.TopLeftBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));

                            if (node.BottomLeftFront == null)
                            {
                                node.BottomLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, Underflow.Y, Overflow.Z), ref child.color));

                            // Secondaries
                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(child.Size.X, Overflow.Y, Overflow.Z), ref child.color));

                        }
                        else if (Overflow.X > 0 && Overflow.Z > 0)
                        {
                            AddHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, Underflow.Z), ref child.color));

                            // Primaries

                            if (node.BottomLeftFront == null)
                            {
                                node.BottomLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, child.Size.Y, Overflow.Z), ref child.color));

                            if (node.BottomRightBack == null)
                            {
                                node.BottomRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, Underflow.Z), ref child.color));

                            // Secondaries
                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, child.Size.Y, Overflow.Z), ref child.color));

                        }
                        else if (Overflow.X > 0)
                        {
                            AddHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.BottomRightBack == null)
                            {
                                node.BottomRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightBack, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Y > 0)
                        {

                            AddHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));
                            if (node.TopLeftBack == null)
                            {
                                node.TopLeftBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Z > 0)
                        {

                            AddHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                            if (node.BottomLeftFront == null)
                            {
                                node.BottomLeftFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomLeftFront, new Volume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.BottomLeftBack, new Volume(child.Position, child.Size, ref child.color));
                        }

                    }
                }
            }
            #endregion


            #region // Collapsal logic.
            //TryCollapse(ref node);
            /*
            if (!parent.Collapsed && node.BottomLeftBack != null &&
                 node.BottomLeftFront != null &&
                 node.BottomRightBack != null &&
                 node.BottomRightFront != null &&
                 node.TopLeftBack != null &&
                 node.TopLeftFront != null &&
                 node.TopRightBack != null &&
                 node.TopRightFront != null)
            {
                if (node.BottomLeftBack.data.color == node.BottomLeftFront.data.color &&
                    node.BottomRightBack.data.color == node.BottomLeftFront.data.color &&
                    node.BottomRightFront.data.color == node.BottomLeftFront.data.color &&
                    node.TopLeftBack.data.color == node.BottomLeftFront.data.color &&
                    node.TopLeftFront.data.color == node.BottomLeftFront.data.color &&
                    node.TopRightBack.data.color == node.BottomLeftFront.data.color &&
                    node.TopRightFront.data.color == node.BottomLeftFront.data.color)

                    if( node.TopRightFront.data.Collapsed &&
                        node.TopRightBack.data.Collapsed &&
                        node.TopLeftFront.data.Collapsed &&
                        node.TopLeftBack.data.Collapsed &&
                        node.BottomRightFront.data.Collapsed &&
                        node.BottomRightBack.data.Collapsed &&
                        node.BottomLeftFront.data.Collapsed &&
                        node.BottomLeftBack.data.Collapsed )
                            node.data.Collapse(ref node);
            }
            if (parent.Collapsed && parent.Size != Vector3.One &&
                (node.BottomRightFront == null ||
                node.BottomRightBack == null ||
                node.BottomLeftFront == null ||
                node.BottomLeftBack == null ||
                node.TopRightFront == null ||
                node.TopRightBack == null ||
                node.TopLeftFront == null ||
                node.TopLeftBack == null)
                )
                node.data.Expand( ref node );
           
             * */
            #endregion
        }

    }
}
