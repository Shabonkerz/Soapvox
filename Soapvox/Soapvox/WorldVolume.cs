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
    public class WorldVolume : Volume
    {
        /// <summary>
        /// Describes the size of the volume in three dimensions.
        /// </summary>
        //private Vector3 m_size;

        int[] facebufferpositions;

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
        /// Constructs a new volume.
        /// </summary>
        /// <param name="position">The coordinates of the volume.</param>
        /// <param name="size">The size of the volume.</param>
        /// <param name="color">The color of hte volume.</param>
        public WorldVolume(Vector3 position, Vector3 size, ref Color color)
            : base(position, size, ref color)
        {
            this.Size = size;
            this.Position = position;
            this.Half = size / 2;
            this.Half = new Vector3((int)this.Half.X, (int)this.Half.Y, (int)this.Half.Z);
            this.color = color;
            if (size == Vector3.One)
            {
                facebufferpositions = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    facebufferpositions[i] = -1;
                }
            }
        }
        /// <summary>
        /// Constructs a new volume.
        /// </summary>
        /// <param name="position">The coordinates of the volume.</param>
        /// <param name="size">The size of the volume.</param>
        /// <param name="color">The color of hte volume.</param>
        public WorldVolume(Vector3 position, Vector3 size, Color color)
            : base(position, size, ref color)
        {
            this.Size = size;
            this.Position = position;
            this.Half = size / 2;
            this.Half = new Vector3((int)this.Half.X, (int)this.Half.Y, (int)this.Half.Z);
            this.color = color;
            if (size == Vector3.One)
            {
                facebufferpositions = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    facebufferpositions[i] = -1;
                }
            }
        }
        public WorldVolume(Volume volume) : base(volume.Position, volume.Size, volume.color)
        {
            this.Size = volume.Size;
            this.Position = volume.Position;
            this.Half = volume.Size / 2;
            this.Half = new Vector3((int)this.Half.X, (int)this.Half.Y, (int)this.Half.Z);
            this.color = color;
            if (this.Size == Vector3.One)
            {
                facebufferpositions = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    facebufferpositions[i] = -1;
                }
            }
        }
        public override string ToString()
        {
            return "Size(" + m_size.X + "," + m_size.Y + "," + m_size.Z + "), Color(" + color.R + "," + color.G + "," + color.B + ")";
        }
        /// <summary>
        /// Creates a shallow copy of the volume.
        /// </summary>
        /// <param name="input">The volume to copy.</param>
        public void MemberwiseClone(WorldVolume input)
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
                BlockGame.world.faceBatch.RemoveFace(facebufferpositions[(int)facing]);

            facebufferpositions[(int)facing] = BlockGame.world.faceBatch.AddFace(facing, ref Position, m_size, ref color);

        }
        public WorldVolume Translate(int x, int y, int z)
        {
            return Translate(new Vector3(x, y, z));
        }
        public WorldVolume Translate(float x, float y, float z)
        {
            return Translate(new Vector3(x, y, z));
        }
        public WorldVolume Translate(Vector3 offset)
        {
            return new WorldVolume(this.Position + offset, this.Size, this.color);
        }

        public WorldVolume Scale(int x, int y, int z)
        {
            return Scale(new Vector3(x, y, z));
        }
        public WorldVolume Scale(float x, float y, float z)
        {
            return Scale(new Vector3(x, y, z));
        }
        public WorldVolume Scale(Vector3 scale)
        {
            return new WorldVolume(this.Position, this.Size * scale, this.color);
        }

        public void Add()
        {
            if (facebufferpositions == null)
            {
                facebufferpositions = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    facebufferpositions[i] = -1;
                }
            }


            this.AddFace(Face.Facing.Backward);
            this.AddFace(Face.Facing.Down);
            this.AddFace(Face.Facing.Forward);
            this.AddFace(Face.Facing.Left);
            this.AddFace(Face.Facing.Right);
            this.AddFace(Face.Facing.Up);
        }
        public static void TryCollapse(ref Octree<WorldVolume>.OctreeNode node)
        {
            // Make sure we haven't already collapsed, and determine whether all the child nodes are present. If not, dip out.
            if (node.data.Visible == false && 
                 node.BottomLeftBack != null &&
                 node.BottomLeftFront != null &&
                 node.BottomRightBack != null &&
                 node.BottomRightFront != null &&
                 node.TopLeftBack != null &&
                 node.TopLeftFront != null &&
                 node.TopRightBack != null &&
                 node.TopRightFront != null)
            {
                // Only collapse if all the colors are the same!
                if (node.BottomLeftBack.data.color == node.BottomLeftFront.data.color &&
                    node.BottomRightBack.data.color == node.BottomLeftFront.data.color &&
                    node.BottomRightFront.data.color == node.BottomLeftFront.data.color &&
                    node.TopLeftBack.data.color == node.BottomLeftFront.data.color &&
                    node.TopLeftFront.data.color == node.BottomLeftFront.data.color &&
                    node.TopRightBack.data.color == node.BottomLeftFront.data.color &&
                    node.TopRightFront.data.color == node.BottomLeftFront.data.color)

                    // Only collapse if all the children are collapsed. By default, every unit sized volume is collapsed.
                    if (node.TopRightFront.data.Visible &&
                        node.TopRightBack.data.Visible &&
                        node.TopLeftFront.data.Visible &&
                        node.TopLeftBack.data.Visible &&
                        node.BottomRightFront.data.Visible &&
                        node.BottomRightBack.data.Visible &&
                        node.BottomLeftFront.data.Visible &&
                        node.BottomLeftBack.data.Visible)
                        Collapse(ref node);
            }
        }
        public static void Collapse(ref Octree<WorldVolume>.OctreeNode node)
        {
            // Remove the vertices from the batch.
            node.BottomLeftBack.data.Remove();
            node.BottomLeftFront.data.Remove();
            node.BottomRightBack.data.Remove();
            node.BottomRightFront.data.Remove();
            node.TopLeftBack.data.Remove();
            node.TopLeftFront.data.Remove();
            node.TopRightBack.data.Remove();
            node.TopRightFront.data.Remove();

            // Make the volumes invisible.
            node.BottomLeftBack.data.Visible = false;
            node.BottomLeftFront.data.Visible = false;
            node.BottomRightBack.data.Visible = false;
            node.BottomRightFront.data.Visible = false;
            node.TopLeftBack.data.Visible = false;
            node.TopLeftFront.data.Visible = false;
            node.TopRightBack.data.Visible = false;
            node.TopRightFront.data.Visible = false;

            // Initialize all the facebatch indices to -1
            node.data.facebufferpositions = new int[6];
            for (int i = 0; i < 6; i++)
            {
                node.data.facebufferpositions[i] = -1;
            }

            node.data.Add();
            node.data.Visible = true;

        }
        public static void Expand(ref Octree<WorldVolume>.OctreeNode node)
        {
            if (node.BottomLeftBack != null)
            {
                node.BottomLeftBack.data.Add();
                node.BottomLeftBack.data.Visible = true;
            }
            if (node.BottomLeftFront != null)
            {
                node.BottomLeftFront.data.Add();
                node.BottomLeftFront.data.Visible = true;
            }
            if (node.BottomRightBack != null)
            {
                node.BottomRightBack.data.Add();
                node.BottomRightBack.data.Visible = true;
            }
            if (node.BottomRightFront != null)
            {
                node.BottomRightFront.data.Add();
                node.BottomRightFront.data.Visible = true;
            }
            if (node.TopLeftBack != null)
            {
                node.TopLeftBack.data.Add();
                node.TopLeftBack.data.Visible = true;
            }
            if (node.TopLeftFront != null)
            {
                node.TopLeftFront.data.Add();
                node.TopLeftFront.data.Visible = true;
            }
            if (node.TopRightBack != null)
            {
                node.TopRightBack.data.Add();
                node.TopRightBack.data.Visible = true;
            }
            if (node.TopRightFront != null)
            {
                node.TopRightFront.data.Add();
                node.TopRightFront.data.Visible = true;
            }


            node.data.Visible = false;

            node.data.Remove();

            node.data.facebufferpositions = null;

        }
        /// <summary>
        /// Removes a particular face fo a WorldVolume from the global FaceBatch.
        /// </summary>
        /// <param name="facing">The face to remove.</param>
        public void RemoveFace(Face.Facing facing)
        {
            BlockGame.world.faceBatch.RemoveFace(facebufferpositions[(int)facing]);
            facebufferpositions[(int)facing] = -1;
        }
        /// <summary>
        /// Removes the WorldVolume's faces from the global FaceBatch.
        /// </summary>
        public void Remove()
        {
            if (facebufferpositions == null) return;
            for (int i = 0; i < 6; i++)
            {
                if (facebufferpositions[i] >= 0) BlockGame.world.faceBatch.RemoveFace(facebufferpositions[i]);
                facebufferpositions[i] = -1;
            }
        }

        #region // Static methods


        /// <summary>
        /// Handles the data of the node prior to removal. In this case, the vertex data is removed prior to the node's removal.
        /// </summary>
        /// <param name="node">The node that is about to be removed.</param>
        public static void RemoveAllHandler(ref Octree<WorldVolume>.OctreeNode node)
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
        /// <summary>
        /// Handles the removal of a node from an Octree.
        /// </summary>
        /// <param name="node">The Octree node to start the search from.</param>
        /// <param name="position">The location of the WorldVolume to remove.</param>
        //public static void RemoveHandler(ref Octree<WorldVolume>.OctreeNode node, WorldVolume volume)
        //{
        //    if (volume.Position.X < node.data.Middle.X)
        //    {
        //        if (volume.Position.Y < node.data.Middle.Y)
        //        {
        //            if (volume.Position.Z < node.data.Middle.Z)
        //            {
        //                if (node.BottomLeftBack != null)
        //                {
        //                    if (node.data.Size == volume.Size * 2)
        //                    {
        //                        Expand(ref node);
        //                        node.BottomLeftBack.data.Remove();
        //                        node.BottomLeftBack = null;
        //                        UpdateNeighbors(volume);
        //                    }
        //                    else
        //                        RemoveHandler(ref node.BottomLeftBack, volume);
        //                }
        //            }
        //            else
        //            {
        //                if (node.BottomLeftFront != null)
        //                {
        //                    if (node.data.Size == volume.Size * 2)
        //                    {
        //                        Expand(ref node);
        //                        node.BottomLeftFront.data.Remove();
        //                        node.BottomLeftFront = null;
        //                        UpdateNeighbors(volume);
        //                    }
        //                    else
        //                        RemoveHandler(ref node.BottomLeftFront, volume);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (volume.Position.Z < node.data.Middle.Z)
        //            {
        //                if (node.TopLeftBack != null)
        //                {
        //                    if (node.data.Size == volume.Size * 2)
        //                    {
        //                        Expand(ref node);
        //                        node.TopLeftBack.data.Remove();
        //                        node.TopLeftBack = null;
        //                        UpdateNeighbors(volume);
        //                    }
        //                    else
        //                        RemoveHandler(ref node.TopLeftBack, volume);
        //                }
        //            }
        //            else
        //            {
        //                if (node.TopLeftFront != null)
        //                {
        //                    if (node.data.Size == volume.Size * 2)
        //                    {
        //                        Expand(ref node);
        //                        node.TopLeftFront.data.Remove();
        //                        node.TopLeftFront = null;
        //                        UpdateNeighbors(volume);
        //                    }
        //                    else
        //                        RemoveHandler(ref node.TopLeftFront, volume);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (volume.Position.Y < node.data.Middle.Y)
        //        {
        //            if (volume.Position.Z < node.data.Middle.Z)
        //            {
        //                if (node.BottomRightBack != null)
        //                {
        //                    if (node.data.Size == volume.Size * 2)
        //                    {
        //                        Expand(ref node);
        //                        node.BottomRightBack.data.Remove();
        //                        node.BottomRightBack = null;
        //                        UpdateNeighbors(volume);
        //                    }
        //                    else
        //                        RemoveHandler(ref node.BottomRightBack, volume);
        //                }
        //            }
        //            else
        //            {
        //                if (node.BottomRightFront != null)
        //                {
        //                    if (node.data.Size == volume.Size * 2)
        //                    {
        //                        Expand(ref node);
        //                        node.BottomRightFront.data.Remove();
        //                        node.BottomRightFront = null;
        //                        UpdateNeighbors(volume);
        //                    }
        //                    else
        //                        RemoveHandler(ref node.BottomRightFront, volume);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (volume.Position.Z < node.data.Middle.Z)
        //            {
        //                if (node.TopRightBack != null)
        //                {
        //                    if (node.data.Size == volume.Size * 2)
        //                    {
        //                        Expand(ref node);
        //                        node.TopRightBack.data.Remove();
        //                        node.TopRightBack = null;
        //                        UpdateNeighbors(volume);
        //                    }
        //                    else
        //                        RemoveHandler(ref node.TopRightBack, volume);
        //                }
        //            }
        //            else
        //            {
        //                if (node.TopRightFront != null)
        //                {
        //                    if (node.data.Size == volume.Size * 2)
        //                    {
        //                        Expand(ref node);
        //                        node.TopRightFront.data.Remove();
        //                        node.TopRightFront = null;
        //                        UpdateNeighbors(volume);
        //                    }
        //                    else
        //                        RemoveHandler(ref node.TopRightFront, volume);
        //                }
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// Handles the instantiation of the root node of an Octree.
        /// </summary>
        /// <param name="root">The root node to instantiate.</param>
        /// <param name="size">The maximum size of the Octree, which the root node will hold.</param>
        public static void SetRootHandler(ref Octree<WorldVolume>.OctreeNode root, WorldVolume WorldVolume)
        {
            Console.WriteLine("WorldVolume: " + WorldVolume.Size);
            // Set root node!
            root = new Octree<WorldVolume>.OctreeNode(new WorldVolume(Vector3.Zero, WorldVolume.Size, ref WorldVolume.color));
        }

        /// <summary>
        /// Octree search based on WorldVolume. To be used with an instance of Octree.
        /// </summary>
        /// <param name="node">The OctreeNode to search from.</param>
        /// <param name="position">The location of the WorldVolume being searched for.</param>
        /// <returns>A WorldVolume at location provided.</returns>
        public static WorldVolume SearchHandler(ref Octree<WorldVolume>.OctreeNode node, WorldVolume volume)
        {
            // If we're searching inside a smaller volume, it's obviously not here.
            if (volume.Size.X > node.data.Size.X 
                && volume.Size.Y > node.data.Size.Y 
                && volume.Size.Z > node.data.Size.Z) return null;

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
        /// Simple helper function to restore faces that were previously deleted when a WorldVolume at the location was added, which is now removed.
        /// </summary>
        /// <param name="position">The location of the WorldVolume that was previously removed.</param>
        public static void UpdateNeighbors(WorldVolume volume)
        {
            WorldVolume tmp = BlockGame.world.Search( volume.Translate( Vector3.Left ) );
            if (tmp != null) tmp.AddFace(Face.Facing.Left);

            tmp = BlockGame.world.Search(volume.Translate(Vector3.Right));
            if (tmp != null) tmp.AddFace(Face.Facing.Right);

            tmp = BlockGame.world.Search(volume.Translate(Vector3.Up));
            if (tmp != null) tmp.AddFace(Face.Facing.Down);

            tmp = BlockGame.world.Search(volume.Translate(Vector3.Down));
            if (tmp != null) tmp.AddFace(Face.Facing.Up);

            tmp = BlockGame.world.Search(volume.Translate(Vector3.Forward));
            if (tmp != null) tmp.AddFace(Face.Facing.Backward);

            tmp = BlockGame.world.Search(volume.Translate(Vector3.Backward));
            if (tmp != null) tmp.AddFace(Face.Facing.Forward);

        }
        /// <summary>
        /// Inserts a WorldVolume of size one, or greater, into an Octree.
        /// </summary>
        /// <param name="node">The Octree to insert into.</param>
        /// <param name="child">The WorldVolume to add to the Octree.</param>
        public static void AddHandler(ref Octree<WorldVolume>.OctreeNode node, WorldVolume child)
        {


            if (node == null)
            {
                return;
            }

            WorldVolume parent = node.data;
            //Console.WriteLine("Adding volume: " + child.ToString() + " inside of " + parent.ToString());
            parent.Middle.X = (int)(parent.Half.X + parent.Position.X);
            parent.Middle.Y = (int)(parent.Half.Y + parent.Position.Y);
            parent.Middle.Z = (int)(parent.Half.Z + parent.Position.Z);

            #region // Stop condition

            if (parent.Size.X < 2 && parent.Size.Y < 2 && parent.Size.Z < 2)
            {
                //if( parent.color != child.color ) 
                    parent.Remove();

                parent.color = child.color;
                
                WorldVolume tmp = BlockGame.world.Search(child.Translate(Vector3.Forward));

                 //Add faces if the neighbors do not exist, otherwise delete the appropriate neighboring cube's faces.
                if (tmp == null)
                    parent.AddFace(Face.Facing.Backward);
                else
                    tmp.RemoveFace(Face.Facing.Forward);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Backward));

                if (tmp == null)
                    parent.AddFace(Face.Facing.Forward);
                else
                    tmp.RemoveFace(Face.Facing.Backward);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Left));

                if (tmp == null)
                    parent.AddFace(Face.Facing.Left);
                else
                    tmp.RemoveFace(Face.Facing.Right);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Right));

                if (tmp == null)
                    parent.AddFace(Face.Facing.Right);
                else
                    tmp.RemoveFace(Face.Facing.Left);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Up));

                if (tmp == null)
                    parent.AddFace(Face.Facing.Up);
                else
                    tmp.RemoveFace(Face.Facing.Down);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Down));

                if (tmp == null)
                    parent.AddFace(Face.Facing.Down);
                else
                    tmp.RemoveFace(Face.Facing.Up);


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
                            node.TopRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(parent.Middle, parent.Half, ref child.color));
                        }
                        AddHandler(ref node.TopRightFront, new WorldVolume(child.Position, child.Size, ref child.color));
                    }
                    else
                    {
                        // We know that the top, right, back octant has something in it.
                        if (node.TopRightBack == null)
                        {
                            node.TopRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                        }

                        // Test for overflow into the z direction.
                        if (Overflow.Z > 0)
                        {
                            AddHandler(ref node.TopRightBack, new WorldVolume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(parent.Middle, parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.TopRightBack, new WorldVolume(child.Position, child.Size, ref child.color));
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
                            node.BottomRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                        }

                        // Test for overflow in the Y direction.
                        if (Overflow.Y > 0)
                        {
                            AddHandler(ref node.BottomRightFront, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(parent.Middle, parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.BottomRightFront, new WorldVolume(child.Position, child.Size, ref child.color));
                        }

                    }
                    else
                    {
                        // We know that the bottom, right, back octant has something in it...
                        if (node.BottomRightBack == null)
                        {
                            node.BottomRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                        }

                        // Test for both cases.

                        if (Overflow.Y > 0 && Overflow.Z > 0)
                        {
                            AddHandler(ref node.BottomRightBack, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, Underflow.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, Underflow.Y, Overflow.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(parent.Middle, parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(child.Size.X, Overflow.Y, Overflow.Z), ref child.color));
                        }
                        else if (Overflow.Y > 0)
                        {
                            AddHandler(ref node.BottomRightBack, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopRightBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Z > 0)
                        {
                            AddHandler(ref node.BottomRightBack, new WorldVolume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.BottomRightBack, new WorldVolume(child.Position, child.Size, ref child.color));
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
                            node.TopLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                        }


                        // Test for overflow into the X direction.
                        if (Overflow.X > 0)
                        {
                            AddHandler(ref node.TopLeftFront, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(parent.Middle, parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.TopLeftFront, new WorldVolume(child.Position, child.Size, ref child.color));
                        }
                    }

                    else
                    {

                        // Octant #8 has no possible overflow.
                        if (node.TopLeftBack == null)
                        {
                            node.TopLeftBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                        }

                        // Test for three cases.

                        if (Overflow.X > 0 && Overflow.Z > 0)
                        {
                            AddHandler(ref node.TopLeftBack, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, child.Size.Y, Overflow.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(parent.Middle, parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else if (Overflow.X > 0)
                        {
                            AddHandler(ref node.TopLeftBack, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Z > 0)
                        {
                            AddHandler(ref node.TopLeftBack, new WorldVolume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));

                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.TopLeftBack, new WorldVolume(child.Position, child.Size, ref child.color));

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
                            node.BottomLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                        }

                        // Test two cases.
                        if (Overflow.X > 0 && Overflow.Y > 0)
                        {
                            AddHandler(ref node.BottomLeftFront, new WorldVolume(child.Position, new Vector3(Underflow.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(parent.Middle, parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightFront, new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.X > 0)
                        {
                            AddHandler(ref node.BottomLeftFront, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Y > 0)
                        {
                            AddHandler(ref node.BottomLeftFront, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));

                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.BottomLeftFront, new WorldVolume(child.Position, child.Size, ref child.color));
                        }
                    }
                    else
                    {
                        // We know that the bottom, right, back octant has something in it...
                        if (node.BottomLeftBack == null)
                        {
                            node.BottomLeftBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(parent.Position, parent.Half, ref child.color));
                        }

                        // Test for both cases.
                        if (Overflow.X > 0 && Overflow.Y > 0 && Overflow.Z > 0)
                        {
                            AddHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, Underflow, ref child.color));
                            // Primaries
                            if (node.BottomRightBack == null)
                            {
                                node.BottomRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, Underflow.Y, Underflow.Z), ref child.color));

                            if (node.TopLeftBack == null)
                            {
                                node.TopLeftBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, Underflow.Z), ref child.color));

                            if (node.BottomLeftFront == null)
                            {
                                node.BottomLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, Underflow.Y, Overflow.Z), ref child.color));

                            // Secondaries
                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(Underflow.X, Overflow.Y, Overflow.Z), ref child.color));

                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightBack, new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, Underflow.Z), ref child.color));

                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, Underflow.Y, Overflow.Z), ref child.color));

                            // Tertiary
                            if (node.TopRightFront == null)
                            {
                                node.TopRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(parent.Middle, parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightFront, new WorldVolume(parent.Middle, Overflow, ref child.color));

                        }
                        else if (Overflow.X > 0 && Overflow.Y > 0)
                        {

                            AddHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(Underflow.X, Underflow.Y, child.Size.Z), ref child.color));

                            // Primaries
                            if (node.BottomRightBack == null)
                            {
                                node.BottomRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.TopLeftBack == null)
                            {
                                node.TopLeftBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));

                            // Secondaries
                            if (node.TopRightBack == null)
                            {
                                node.TopRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopRightBack, new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Y > 0 && Overflow.Z > 0)
                        {

                            AddHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, Underflow.Z), ref child.color));
                            // Primaries
                            if (node.TopLeftBack == null)
                            {
                                node.TopLeftBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));

                            if (node.BottomLeftFront == null)
                            {
                                node.BottomLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, Underflow.Y, Overflow.Z), ref child.color));

                            // Secondaries
                            if (node.TopLeftFront == null)
                            {
                                node.TopLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(child.Size.X, Overflow.Y, Overflow.Z), ref child.color));

                        }
                        else if (Overflow.X > 0 && Overflow.Z > 0)
                        {
                            AddHandler(ref node.BottomLeftBack, new WorldVolume( child.Position, new Vector3(Underflow.X, child.Size.Y, Underflow.Z), ref child.color));

                            // Primaries

                            if (node.BottomLeftFront == null)
                            {
                                node.BottomLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }

                            AddHandler(ref node.BottomLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, child.Size.Y, Overflow.Z), ref child.color));

                            if (node.BottomRightBack == null)
                            {
                                node.BottomRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, Underflow.Z), ref child.color));

                            // Secondaries
                            if (node.BottomRightFront == null)
                            {
                                node.BottomRightFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, child.Size.Y, Overflow.Z), ref child.color));

                        }
                        else if (Overflow.X > 0)
                        {
                            AddHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));

                            if (node.BottomRightBack == null)
                            {
                                node.BottomRightBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Middle.X, parent.Position.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Y > 0)
                        {

                            AddHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));
                            if (node.TopLeftBack == null)
                            {
                                node.TopLeftBack = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Middle.Y, parent.Position.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.TopLeftBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                        }
                        else if (Overflow.Z > 0)
                        {

                            AddHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                            if (node.BottomLeftFront == null)
                            {
                                node.BottomLeftFront = new Octree<WorldVolume>.OctreeNode(new WorldVolume(new Vector3(parent.Position.X, parent.Position.Y, parent.Middle.Z), parent.Half, ref child.color));
                            }
                            AddHandler(ref node.BottomLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                        }
                        else
                        {
                            AddHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, child.Size, ref child.color));
                        }

                    }
                }
            }
            #endregion


            TryCollapse(ref node);
            
        }
        public static void RemoveHandler(ref Octree<WorldVolume>.OctreeNode node, WorldVolume child)
        {


            if (node == null)
                return;

            WorldVolume parent = node.data;
            //Console.WriteLine("Adding volume: " + child.ToString() + " inside of " + parent.ToString());
            parent.Middle.X = (int)(parent.Half.X + parent.Position.X);
            parent.Middle.Y = (int)(parent.Half.Y + parent.Position.Y);
            parent.Middle.Z = (int)(parent.Half.Z + parent.Position.Z);

            #region // Stop condition

            if (parent.Size.X <= child.Size.X && parent.Size.Y <= child.Size.Y && parent.Size.Z <= child.Size.Z && parent.color == child.color)
            {
                //parent.RemoveFace(Face.Facing.Backward);
                //parent.RemoveFace(Face.Facing.Down);
                //parent.RemoveFace(Face.Facing.Forward);
                //parent.RemoveFace(Face.Facing.Left);
                //parent.RemoveFace(Face.Facing.Right);
                //parent.RemoveFace(Face.Facing.Up);


                WorldVolume tmp = BlockGame.world.Search(child.Translate(Vector3.Forward));

                //Add faces if the neighbors do not exist, otherwise delete the appropriate neighboring cube's faces.
                if (tmp != null)
                    tmp.AddFace(Face.Facing.Forward);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Backward));

                if (tmp != null)
                    tmp.AddFace(Face.Facing.Backward);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Left));

                if (tmp != null)
                    tmp.AddFace(Face.Facing.Right);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Right));

                if (tmp != null)
                    tmp.AddFace(Face.Facing.Left);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Up));

                if (tmp != null)
                    tmp.AddFace(Face.Facing.Down);


                tmp = BlockGame.world.Search(child.Translate(Vector3.Down));

                if (tmp != null)
                    tmp.AddFace(Face.Facing.Up);



                parent.Remove();

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
                            RemoveHandler(ref node.TopRightFront, new WorldVolume(child.Position, child.Size, ref child.color));
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
                                RemoveHandler(ref node.TopRightBack, new WorldVolume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.TopRightBack = null;
                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));

                                node.TopRightFront = null;
                            }
                        }
                        else
                        {
                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new WorldVolume(child.Position, child.Size, ref child.color));

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
                                RemoveHandler(ref node.BottomRightFront, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));

                                node.BottomRightFront = null;
                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else
                        {
                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new WorldVolume(child.Position, child.Size, ref child.color));
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
                                RemoveHandler(ref node.BottomRightBack, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, Underflow.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));
                                node.TopRightBack = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, Underflow.Y, Overflow.Z), ref child.color));
                                node.BottomRightFront = null;
                                
                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(child.Size.X, Overflow.Y, Overflow.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else if (Overflow.Y > 0)
                        {
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopRightBack = null;
                            }
                        }
                        else if (Overflow.Z > 0)
                        {
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new WorldVolume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.BottomRightFront = null;
                            }
                        }
                        else
                        {
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new WorldVolume(child.Position, child.Size, ref child.color));
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
                                RemoveHandler(ref node.TopLeftFront, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                            if (node.TopRightFront == null)
                            {
                                RemoveHandler(ref node.TopRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else
                        {
                            if (node.TopLeftBack != null) 
                            {
                                RemoveHandler(ref node.TopLeftFront, new WorldVolume(child.Position, child.Size, ref child.color));
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
                                RemoveHandler(ref node.TopLeftBack, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.TopRightBack = null;
                            }

                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else if (Overflow.X > 0)
                        {
                            if (node.TopLeftBack != null) 
                            {
                                RemoveHandler(ref node.TopLeftBack, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.TopRightBack = null;
                            }
                        }
                        else if (Overflow.Z > 0)
                        {
                            if (node.TopLeftBack != null) 
                            {
                                RemoveHandler(ref node.TopLeftBack, new WorldVolume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.TopLeftFront = null;
                            }
                        }
                        else
                        {
                            if (node.TopLeftBack != null) 
                            {
                                RemoveHandler(ref node.TopLeftBack, new WorldVolume(child.Position, child.Size, ref child.color));
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
                                RemoveHandler(ref node.BottomLeftFront, new WorldVolume(child.Position, new Vector3(Underflow.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomRightFront = null;
                            }

                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopRightFront = null;
                            }
                        }
                        else if (Overflow.X > 0)
                        {
                            if (node.BottomLeftFront != null) 
                            {
                                RemoveHandler(ref node.BottomLeftFront, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomRightFront = null;
                            }
                        }
                        else if (Overflow.Y > 0)
                        {
                            if (node.BottomLeftFront != null) 
                            {
                                RemoveHandler(ref node.BottomLeftFront, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopLeftFront = null;
                            }
                        }
                        else
                        {
                            if (node.BottomLeftFront != null) 
                            {
                                RemoveHandler(ref node.BottomLeftFront, new WorldVolume(child.Position, child.Size, ref child.color));
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
                                RemoveHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, Underflow, ref child.color));
                                node.BottomLeftBack = null;
                            }
                            // Primaries
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, Underflow.Y, Underflow.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, Underflow.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, Underflow.Y, Overflow.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            // Secondaries
                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(Underflow.X, Overflow.Y, Overflow.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, Underflow.Z), ref child.color));
                                node.TopRightBack = null;
                            }

                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, Underflow.Y, Overflow.Z), ref child.color));
                                node.BottomRightFront = null;
                            }

                            // Tertiary
                            if (node.TopRightFront != null)
                            {
                                RemoveHandler(ref node.TopRightFront, new WorldVolume(parent.Middle, Overflow, ref child.color));
                                node.TopRightFront = null;
                            }

                        }
                        else if (Overflow.X > 0 && Overflow.Y > 0)
                        {

                            if (node.BottomLeftBack != null) 
                            {
                                RemoveHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(Underflow.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }

                            // Primaries
                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(Underflow.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            // Secondaries
                            if (node.TopRightBack != null)
                            {
                                RemoveHandler(ref node.TopRightBack, new WorldVolume(new Vector3(parent.Middle.X, parent.Middle.Y, child.Position.Z), new Vector3(Overflow.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopRightBack = null;
                            }
                        }
                        else if (Overflow.Y > 0 && Overflow.Z > 0)
                        {

                            if (node.BottomLeftBack != null) 
                            {
                                RemoveHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, Underflow.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }
                            // Primaries
                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, Underflow.Z), ref child.color));
                                node.TopLeftBack = null;
                            }

                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, Underflow.Y, Overflow.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            // Secondaries
                            if (node.TopLeftFront != null)
                            {
                                RemoveHandler(ref node.TopLeftFront, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, parent.Middle.Z), new Vector3(child.Size.X, Overflow.Y, Overflow.Z), ref child.color));
                                node.TopLeftFront = null;
                            }

                        }
                        else if (Overflow.X > 0 && Overflow.Z > 0)
                        {
                            if (node.BottomLeftBack != null) 
                            {
                                RemoveHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }

                            // Primaries

                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(Underflow.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }

                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.BottomRightBack = null;
                            }

                            // Secondaries
                            if (node.BottomRightFront != null)
                            {
                                RemoveHandler(ref node.BottomRightFront, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, parent.Middle.Z), new Vector3(Overflow.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.BottomRightFront = null;
                            }

                        }
                        else if (Overflow.X > 0)
                        {
                            if (node.BottomLeftBack != null) 
                            {
                                RemoveHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(Underflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }

                            if (node.BottomRightBack != null)
                            {
                                RemoveHandler(ref node.BottomRightBack, new WorldVolume(new Vector3(parent.Middle.X, child.Position.Y, child.Position.Z), new Vector3(Overflow.X, child.Size.Y, child.Size.Z), ref child.color));
                                node.BottomRightBack = null;
                            }
                        }
                        else if (Overflow.Y > 0)
                        {

                            if (node.BottomLeftBack != null) 
                            {
                                RemoveHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(child.Size.X, Underflow.Y, child.Size.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }
                            if (node.TopLeftBack != null)
                            {
                                RemoveHandler(ref node.TopLeftBack, new WorldVolume(new Vector3(child.Position.X, parent.Middle.Y, child.Position.Z), new Vector3(child.Size.X, Overflow.Y, child.Size.Z), ref child.color));
                                node.TopLeftBack = null;
                            }
                        }
                        else if (Overflow.Z > 0)
                        {

                            if (node.BottomLeftBack != null) 
                            {
                                RemoveHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, new Vector3(child.Size.X, child.Size.Y, Underflow.Z), ref child.color));
                                node.BottomLeftBack = null;
                            }
                            if (node.BottomLeftFront != null)
                            {
                                RemoveHandler(ref node.BottomLeftFront, new WorldVolume(new Vector3(child.Position.X, child.Position.Y, parent.Middle.Z), new Vector3(child.Size.X, child.Size.Y, Overflow.Z), ref child.color));
                                node.BottomLeftFront = null;
                            }
                        }
                        else
                        {
                            if (node.BottomLeftBack != null) 
                            {
                                RemoveHandler(ref node.BottomLeftBack, new WorldVolume(child.Position, child.Size, ref child.color));
                                node.BottomLeftBack = null;
                            }
                        }

                    }
                }
            }
            #endregion

            
            //TryCollapse(ref node);

        }


        #endregion
    }
}
