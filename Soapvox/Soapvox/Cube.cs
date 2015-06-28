using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sandvox
{
    /// <summary>
    /// Describes a three-dimensional volume in space.
    /// </summary>
    public class Cube : Volume
    {

        /// <summary>
        /// Flag for whether the volume is collapsed or not.
        /// </summary>
        public bool Collapsed;

        int[] facebufferpositions;
        Vector3 Middle;
        Vector3 m_size;

        /// <summary>
        /// Constructs a new volume.
        /// </summary>
        /// <param name="position">The coordinates of the volume.</param>
        /// <param name="size">The size of the volume.</param>
        /// <param name="color">The color of the volume.</param>
        public Cube(Vector3 position, Vector3 size, ref Color color) : base(position, size, ref color)
        {
            
        }
        /// <summary>
        /// Constructs a new volume.
        /// </summary>
        /// <param name="position">The coordinates of the volume.</param>
        /// <param name="size">The size of the volume.</param>
        /// <param name="color">The color of hte volume.</param>
        public Cube(Vector3 position, Vector3 size, Color color) : base(position, size, color)
        {
        }
        /// <summary>
        /// Creates a shallow copy of the volume.
        /// </summary>
        /// <param name="input">The volume to copy.</param>
        public void MemberwiseClone(Cube input)
        {
            // Copy references to members.
            this.Size = input.Size;
            this.Position = input.Position;
            this.facebufferpositions = input.facebufferpositions;
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
        /// Adds a particular face to the global FaceBatch.
        /// </summary>
        /// <param name="facing">The face to add.</param>
        public void AddFace(Face.Facing facing)
        {
            if (facebufferpositions[(int)facing] > -1)
                Game.world.faceBatch.RemoveFace(facebufferpositions[(int)facing]);

            facebufferpositions[(int)facing] = Game.world.faceBatch.AddFace(facing, ref Position, m_size, ref color);

        }
        public void Add()
        {
            this.AddFace(Face.Facing.Backward);
            this.AddFace(Face.Facing.Down);
            this.AddFace(Face.Facing.Forward);
            this.AddFace(Face.Facing.Left);
            this.AddFace(Face.Facing.Right);
            this.AddFace(Face.Facing.Up);
        }
        public void Collapse(ref Octree<Volume>.OctreeNode node)
        {
            this.Collapsed = true;
            node.BottomLeftBack.data.Remove();
            node.BottomLeftFront.data.Remove();
            node.BottomRightBack.data.Remove();
            node.BottomRightFront.data.Remove();
            node.TopLeftBack.data.Remove();
            node.TopLeftFront.data.Remove();
            node.TopRightBack.data.Remove();
            node.TopRightFront.data.Remove();

            node.data.Add();

        }
        public void Expand(ref Octree<Volume>.OctreeNode node)
        {
            this.Collapsed = false;

            this.Remove();


            if( node.BottomLeftBack != null )
                node.BottomLeftBack.data.Add();
            if (node.BottomLeftFront != null)
                node.BottomLeftFront.data.Add();
            if (node.BottomRightBack != null)
                node.BottomRightBack.data.Add();
            if (node.BottomRightFront != null)
                node.BottomRightFront.data.Add();
            if (node.TopLeftBack != null)
                node.TopLeftBack.data.Add();
            if (node.TopLeftFront != null)
                node.TopLeftFront.data.Add();
            if (node.TopRightBack != null)
                node.TopRightBack.data.Add();
            if (node.TopRightFront != null) 
                node.TopRightFront.data.Add();

        }
        /// <summary>
        /// Removes a particular face fo a volume from the global FaceBatch.
        /// </summary>
        /// <param name="facing">The face to remove.</param>
        public void RemoveFace(Face.Facing facing)
        {
            Game.world.faceBatch.RemoveFace(facebufferpositions[(int)facing]);
            facebufferpositions[(int)facing] = -1;
        }
        /// <summary>
        /// Removes the volume's faces from the global FaceBatch.
        /// </summary>
        public void Remove()
        {
            for (int i = 0; i < 6; i++)
            {
                if (facebufferpositions[i] >= 0) Game.world.faceBatch.RemoveFace(facebufferpositions[i]);
                facebufferpositions[i] = -1;
            }
        }
        /// <summary>
        /// Handles the removal of a node from an Octree.
        /// </summary>
        /// <param name="node">The Octree node to start the search from.</param>
        /// <param name="position">The location of the volume to remove.</param>
        public static void RemoveHandler(ref Octree<Volume>.OctreeNode node, Vector3 position)
        {
            if (node.data.Position.X < node.data.Middle.X)
            {
                if (node.data.Position.Y < node.data.Middle.Y)
                {
                    if (node.data.Position.Z < node.data.Middle.Z)
                    {
                        if (node.BottomLeftBack != null)
                        {
                            if (node.data.Size == Vector3.One * 2)
                            {
                                node.BottomLeftBack.data.Remove();
                                node.BottomLeftBack = null;
                                UpdateNeighbors(position);
                            }
                            else
                                RemoveHandler(ref node.BottomLeftBack, position);
                        }
                    }
                    else
                    {
                        if (node.BottomLeftFront != null)
                        {
                            if (node.data.Size == Vector3.One * 2)
                            {
                                node.BottomLeftFront.data.Remove();
                                node.BottomLeftFront = null;
                                UpdateNeighbors(position);
                            }
                            else
                                RemoveHandler(ref node.BottomLeftFront, position);
                        }
                    }
                }
                else
                {
                    if (node.data.Position.Z < node.data.Middle.Z)
                    {
                        if (node.TopLeftBack != null)
                        {
                            if (node.data.Size == Vector3.One * 2)
                            {
                                node.TopLeftBack.data.Remove();
                                node.TopLeftBack = null;
                                UpdateNeighbors(position);
                            }
                            else
                                RemoveHandler(ref node.TopLeftBack, position);
                        }
                    }
                    else
                    {
                        if (node.TopLeftFront != null)
                        {
                            if (node.data.Size == Vector3.One * 2)
                            {
                                node.TopLeftFront.data.Remove();
                                node.TopLeftFront = null;
                                UpdateNeighbors(position);
                            }
                            else
                                RemoveHandler(ref node.TopLeftFront, position);
                        }
                    }
                }
            }
            else
            {
                if (node.data.Position.Y < node.data.Middle.Y)
                {
                    if (node.data.Position.Z < node.data.Middle.Z)
                    {
                        if (node.BottomRightBack != null)
                        {
                            if (node.data.Size == Vector3.One * 2)
                            {
                                node.BottomRightBack.data.Remove();
                                node.BottomRightBack = null;
                                UpdateNeighbors(position);
                            }
                            else
                                RemoveHandler(ref node.BottomRightBack, position);
                        }
                    }
                    else
                    {
                        if (node.BottomRightFront != null)
                        {
                            if (node.data.Size == Vector3.One * 2)
                            {
                                node.BottomRightFront.data.Remove();
                                node.BottomRightFront = null;
                                UpdateNeighbors(position);
                            }
                            else
                                RemoveHandler(ref node.BottomRightFront, position);
                        }
                    }
                }
                else
                {
                    if (node.data.Position.Z < node.data.Middle.Z)
                    {
                        if (node.TopRightBack != null)
                        {
                            if (node.data.Size == Vector3.One * 2)
                            {
                                node.TopRightBack.data.Remove();
                                node.TopRightBack = null;
                                UpdateNeighbors(position);
                            }
                            else
                                RemoveHandler(ref node.TopRightBack, position);
                        }
                    }
                    else
                    {
                        if (node.TopRightFront != null)
                        {
                            if (node.data.Size == Vector3.One * 2)
                            {
                                node.TopRightFront.data.Remove();
                                node.TopRightFront = null;
                                UpdateNeighbors(position);
                            }
                            else
                                RemoveHandler(ref node.TopRightFront, position);
                        }
                    }
                }
            }
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
        public static Volume SearchHandler(ref Octree<Volume>.OctreeNode node, Vector3 position)
        {
            if (node.data.Size == Vector3.One &&
                node.data.Position.X == position.X &&
                node.data.Position.Y == position.Y &&
                node.data.Position.Z == position.Z)
                return node.data;

            if (position.X < node.data.Middle.X)
            {
                if (position.Y < node.data.Middle.Y)
                {
                    if (position.Z < node.data.Middle.Z)
                    {
                        if (node.BottomLeftBack == null) return null;
                        return SearchHandler(ref node.BottomLeftBack, position);
                    }
                    else
                    {
                        if (node.BottomLeftFront == null) return null;
                        return SearchHandler(ref node.BottomLeftFront, position);
                    }
                }
                else
                {
                    if (position.Z < node.data.Middle.Z)
                    {
                        if (node.TopLeftBack == null) return null;
                        return SearchHandler(ref node.TopLeftBack, position);
                    }
                    else
                    {
                        if (node.TopLeftFront == null) return null;
                        return SearchHandler(ref node.TopLeftFront, position);
                    }
                }
            }
            else
            {
                if (position.Y < node.data.Middle.Y)
                {
                    if (position.Z < node.data.Middle.Z)
                    {
                        if (node.BottomRightBack == null) return null;
                        return SearchHandler(ref node.BottomRightBack, position);
                    }
                    else
                    {
                        if (node.BottomRightFront == null) return null;
                        return SearchHandler(ref node.BottomRightFront, position);
                    }
                }
                else
                {
                    if (position.Z < node.data.Middle.Z)
                    {
                        if (node.TopRightBack == null) return null;
                        return SearchHandler(ref node.TopRightBack, position);
                    }
                    else
                    {
                        if (node.TopRightFront == null) return null;
                        return SearchHandler(ref node.TopRightFront, position);
                    }
                }
            }
        }

        /// <summary>
        /// Simple helper function to restore faces that were previously deleted when a volume at the location was added, which is now removed.
        /// </summary>
        /// <param name="position">The location of the volume that was previously removed.</param>
        public static void UpdateNeighbors(Vector3 position)
        {
            Volume tmp = Game.world.Search((int)position.X + 1, (int)position.Y, (int)position.Z);
            if (tmp != null) tmp.AddFace(Face.Facing.Left);

            tmp = Game.world.Search((int)position.X - 1, (int)position.Y, (int)position.Z);
            if (tmp != null) tmp.AddFace(Face.Facing.Right);

            tmp = Game.world.Search((int)position.X, (int)position.Y + 1, (int)position.Z);
            if (tmp != null) tmp.AddFace(Face.Facing.Down);

            tmp = Game.world.Search((int)position.X, (int)position.Y - 1, (int)position.Z);
            if (tmp != null) tmp.AddFace(Face.Facing.Up);

            tmp = Game.world.Search((int)position.X, (int)position.Y, (int)position.Z + 1);
            if (tmp != null) tmp.AddFace(Face.Facing.Backward);

            tmp = Game.world.Search((int)position.X, (int)position.Y, (int)position.Z - 1);
            if (tmp != null) tmp.AddFace(Face.Facing.Forward);

        }
        /// <summary>
        /// Inserts a volume of size one, or greater, into an Octree.
        /// </summary>
        /// <param name="node">The Octree to insert into.</param>
        /// <param name="child">The volume to add to the Octree.</param>
        public static void AddHandler(ref Octree<Volume>.OctreeNode node, Volume child)
        {


            if (node == null)
            {
                return;
            }

            Volume parent = node.data;

            #region // Stop condition

            if (parent.Size.X < 2 && parent.Size.Y < 2 && parent.Size.Z < 2)
            {
                parent.Remove();

                Volume tmp = Game.world.Search((int)child.Position.X, (int)child.Position.Y, (int)child.Position.Z - 1);

                // Add faces if the neighbors do not exist, otherwise delete the appropriate neighboring cube's faces.
                if (tmp == null)
                    parent.AddFace(Face.Facing.Backward);
                else
                    tmp.RemoveFace(Face.Facing.Forward);


                tmp = Game.world.Search((int)child.Position.X, (int)child.Position.Y, (int)child.Position.Z + 1);

                if (tmp == null)
                    parent.AddFace(Face.Facing.Forward);
                else
                    tmp.RemoveFace(Face.Facing.Backward);


                tmp = Game.world.Search((int)child.Position.X - 1, (int)child.Position.Y, (int)child.Position.Z);

                if (tmp == null)
                    parent.AddFace(Face.Facing.Left);
                else
                    tmp.RemoveFace(Face.Facing.Right);


                tmp = Game.world.Search((int)child.Position.X + 1, (int)child.Position.Y, (int)child.Position.Z);

                if (tmp == null)
                    parent.AddFace(Face.Facing.Right);
                else
                    tmp.RemoveFace(Face.Facing.Left);


                tmp = Game.world.Search((int)child.Position.X, (int)child.Position.Y + 1, (int)child.Position.Z);

                if (tmp == null)
                    parent.AddFace(Face.Facing.Up);
                else
                    tmp.RemoveFace(Face.Facing.Down);


                tmp = Game.world.Search((int)child.Position.X, (int)child.Position.Y - 1, (int)child.Position.Z);

                if (tmp == null)
                    parent.AddFace(Face.Facing.Down);
                else
                    tmp.RemoveFace(Face.Facing.Up);




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
                Underflow.Y = (parent.Half.Z - Local.Z);
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
                            AddHandler(ref node.TopRightFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
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
                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));

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
                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
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

                            AddHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightFront, new Volume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<Volume>.OctreeNode(new Volume(parent.Middle, parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightFront, new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
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

                            AddHandler(ref node.TopLeftFront, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
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

                            AddHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Position.Z), new Vector3(Underflow.X, Overflow.Y, Underflow.Z), ref child.color));

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

                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), new Vector3(Overflow.X, Overflow.Y, Underflow.Z), ref child.color));

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

                            AddHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));

                            // Secondaries
                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightBack, new Volume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Y > 0 && Overflow.Z > 0)
                        {

                            AddHandler(ref node.BottomLeftBack, new Volume(child.Position, new Vector3(child.Size.X, Underflow.Y, Underflow.Z), ref child.color));
                            // Primaries
                            if (node.TopLeftBack == null)
                            {
                                node.TopLeftBack = new Octree<Volume>.OctreeNode(new Volume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));

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
                            AddHandler(ref node.BottomLeftBack, new Volume(new Vector3(child.Position.X, child.Position.Y, child.Position.Z), new Vector3(Underflow.X, child.Size.Y, Underflow.Z), ref child.color));

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
                            AddHandler(ref node.TopLeftBack, new Volume(new Vector3(child.Position.X, parent.Middle.Y, parent.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
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

            if ( !parent.Collapsed && node.BottomLeftBack != null &&
                 node.BottomLeftFront != null &&
                 node.BottomRightBack != null &&
                 node.BottomRightFront != null &&
                 node.TopLeftBack != null &&
                 node.TopLeftFront != null &&
                 node.TopRightBack != null &&
                 node.TopRightFront != null)
                node.data.Collapse( ref node );

            if (parent.Collapsed &&
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
            #endregion
        }
    }
}
