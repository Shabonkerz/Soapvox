using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sandvox;

namespace Sandvox
{
    /// <summary>
    /// Generic Octree class. Much like a binary tree, except that instead of 2 child nodes per node, has 8 child nodes per node. Generally used for representing three-dimensional space or holding coordinates.
    /// </summary>
    /// <typeparam name="T">The type of data for each node to hold.</typeparam>
    public class Octree<T>
    {
        /// <summary>
        /// Basic building block of the Octree.
        /// </summary>
        public class OctreeNode
        {
            /// <summary>
            /// The labels for the children of a node.
            /// </summary>
            public enum OctreeOrder
            {
	            TopLeftFront,
	            TopRightFront,
	            BottomLeftFront,
	            BottomRightFront,
	            TopLeftBack,
	            TopRightBack,
	            BottomLeftBack,
	            BottomRightBack
            }
            /// <summary>
            /// The children of the node.
            /// </summary>
            public OctreeNode TopLeftBack,
                               TopLeftFront,
                               TopRightBack,
                               TopRightFront,
                               BottomLeftBack,
                               BottomRightBack,
                               BottomLeftFront,
                               BottomRightFront;
            /// <summary>
            /// The data held by the node.
            /// </summary>
            public T data;

            /// <summary>
            /// Constructs a new OctreeNode containing some initial data.
            /// </summary>
            /// <param name="data">The data to hold.</param>
            public OctreeNode( T data )
            {
                this.data = data;
            }

            /// <summary>
            /// Does nothing.
            /// </summary>

            public void Delete(OctreeNode.OctreeOrder node)
            {
                switch (node)
                {
                    case OctreeOrder.BottomLeftBack:
                        this.BottomLeftBack = null;
                        break;
                    case OctreeOrder.BottomLeftFront:
                        this.BottomLeftFront = null;
                        break;
                    case OctreeOrder.BottomRightBack:
                        this.BottomRightBack = null;
                        break;
                    case OctreeOrder.BottomRightFront:
                        this.BottomRightFront = null;
                        break;
                    case OctreeOrder.TopLeftBack:
                        this.TopLeftBack = null;
                        break;
                    case OctreeOrder.TopLeftFront:
                        this.TopLeftFront = null;
                        break;
                    case OctreeOrder.TopRightBack:
                        this.TopRightBack = null;
                        break;
                    case OctreeOrder.TopRightFront:
                        this.TopRightFront = null;
                        break;
                }

            }
            public void RemoveAll(RemoveAllHandler removeAllHandlerCallback)
            {
                if (this.BottomLeftBack != null)
                {
                    removeAllHandlerCallback(ref this.BottomLeftBack);
                    BottomLeftBack.RemoveAll(removeAllHandlerCallback);
                    this.BottomLeftBack = null;
                }

                if (this.BottomLeftFront != null)
                {
                    removeAllHandlerCallback(ref this.BottomLeftFront);
                    BottomLeftFront.RemoveAll(removeAllHandlerCallback);
                    this.BottomLeftFront = null;
                }

                if (this.BottomRightBack != null)
                {
                    removeAllHandlerCallback(ref this.BottomRightBack);
                    BottomRightBack.RemoveAll(removeAllHandlerCallback);
                    this.BottomRightBack = null;
                }

                if (this.BottomRightFront != null)
                {
                    removeAllHandlerCallback(ref this.BottomRightFront);
                    BottomRightFront.RemoveAll(removeAllHandlerCallback);
                    this.BottomRightFront = null;
                }

                if (this.TopLeftBack != null)
                {
                    removeAllHandlerCallback(ref this.TopLeftBack);
                    TopLeftBack.RemoveAll(removeAllHandlerCallback);
                    this.TopLeftBack = null;
                }


                if (this.TopLeftFront != null)
                {
                    removeAllHandlerCallback(ref this.TopLeftFront);
                    TopLeftFront.RemoveAll(removeAllHandlerCallback);
                    this.TopLeftFront = null;
                }

                if (this.TopRightBack != null)
                {
                    removeAllHandlerCallback(ref this.TopRightBack);
                    TopRightBack.RemoveAll(removeAllHandlerCallback);
                    this.TopRightBack = null;
                }


                if (this.TopRightFront != null)
                {
                    removeAllHandlerCallback(ref this.TopRightFront);
                    TopRightFront.RemoveAll(removeAllHandlerCallback);
                    this.TopRightFront = null;
                }

            }

            private bool InBounds(ref Vector3 position, ref Vector3 size, ref Vector3 point)
            {
                return (point.X >= position.X
                    && point.Y >= position.Y
                    && point.Z >= position.Z
                    && point.X < position.X + size.X
                    && point.Y < position.Y + size.Y
                    && point.Z < position.Z + size.Z);
            }
            /// <summary>
            /// Retrieves the specified child node.
            /// </summary>
            /// <param name="node">The child node to retrieve.</param>
            /// <returns>The specified child node.</returns>
            public OctreeNode getNode(OctreeNode.OctreeOrder node)
            {
                switch (node)
                {
                    case OctreeOrder.TopLeftBack:
                        return this.TopLeftBack;
                    case OctreeOrder.TopLeftFront:
                        return this.TopLeftFront;
                    case OctreeOrder.TopRightBack:
                        return this.TopRightBack;
                    case OctreeOrder.TopRightFront:
                        return this.TopRightFront;
                    case OctreeOrder.BottomLeftFront:
                        return this.BottomLeftFront;
                    case OctreeOrder.BottomLeftBack:
                        return this.BottomLeftBack;
                    case OctreeOrder.BottomRightFront:
                        return this.BottomRightFront;
                    case OctreeOrder.BottomRightBack:
                        return this.BottomRightBack;
                    default:
                        return null;
                }
            }

            /// <summary>
            /// Traverses the entire Octree and performs a function on each node.
            /// </summary>
            /// <param name="traversaloperation">The operation to perform on each node.</param>
            public void Traverse(TraversalOperation traversaloperation)
            {

                if (this.TopLeftBack != null)
                {
                    this.TopLeftBack.Traverse(traversaloperation);
                    traversaloperation(ref this.TopLeftBack);
                }

                if (this.TopLeftFront != null)
                {
                    this.TopLeftFront.Traverse(traversaloperation);
                    traversaloperation(ref this.TopLeftFront);
                }

                if (this.TopRightBack != null)
                {
                    this.TopRightBack.Traverse(traversaloperation);
                    traversaloperation(ref this.TopRightBack);
                }

                if (this.TopRightFront != null)
                {
                    this.TopRightFront.Traverse(traversaloperation);
                    traversaloperation(ref this.TopRightFront);
                }

                if (this.BottomLeftFront != null)
                {
                    this.BottomLeftFront.Traverse(traversaloperation);
                    traversaloperation(ref this.BottomLeftFront);
                }

                if (this.BottomLeftBack != null)
                {
                    this.BottomLeftBack.Traverse(traversaloperation);
                    traversaloperation(ref this.BottomLeftBack);
                }

                if (this.BottomRightFront != null)
                {
                    this.BottomRightFront.Traverse(traversaloperation);
                    traversaloperation(ref this.BottomRightFront);
                }

                if (this.BottomRightBack != null)
                {
                    this.BottomRightBack.Traverse(traversaloperation);
                    traversaloperation(ref this.BottomRightBack);
                }

            }
        }

        /// <summary>
        /// Handles the OnAdd event.
        /// </summary>
        /// <param name="sender">The node that was added.</param>
        /// <param name="e">The arguments executed upon the event.</param>
        public delegate void OnAddHandler(object sender, EventArgs e);


        /// <summary>
        /// Handles the OnRemove event.
        /// </summary>
        /// <param name="sender">The node that was removed.</param>
        /// <param name="e">The arguments executed upon the event.</param>
        public delegate void OnRemoveHandler(object sender, EventArgs e);

        /// <summary>
        /// The delegate for the traversal operation.
        /// </summary>
        /// <param name="node">The node to operate on.</param>
        public delegate void TraversalOperation(ref OctreeNode node);

        /// <summary>
        /// Add function delegate.
        /// </summary>
        /// <param name="node">The node or Octree to insert into.</param>
        /// <param name="data">The data to insert.</param>
        public delegate void AddHandler(ref OctreeNode node, T data);

        /// <summary>
        /// Remove function delegate.
        /// </summary>
        /// <param name="node">The node or Octree to remove from.</param>
        /// <param name="Position">The location of the object to remove.</param>
        public delegate void RemoveHandler(ref OctreeNode node, T data);

        /// <summary>
        /// Search function delegate.
        /// </summary>
        /// <param name="node">The node or Octree to search from.</param>
        /// <param name="position">The location to retrieve the data from.</param>
        /// <returns></returns>
        public delegate T SearchHandler(ref OctreeNode node, T data);

        /// <summary>
        /// The delegate for initializing the root.
        /// </summary>
        /// <param name="node">The root node.</param>
        /// <param name="data">The data to set the root node to.</param>
        public delegate void SetRootHandler(ref OctreeNode node, T data);

        /// <summary>
        /// The delegate for removing all nodes.
        /// </summary>
        /// <param name="node">The current node containing whatever data needs to be modified or deleted upon node deletion.</param>
        public delegate void RemoveAllHandler(ref OctreeNode node);

        /// <summary>
        /// Event triggered when a node is added.
        /// </summary>
        public event OnAddHandler AddEvent;

        /// <summary>
        /// Event triggered when a node is removed.
        /// </summary>
        public event OnRemoveHandler RemoveEvent;

        /// <summary>
        /// Callback function for adding a node.
        /// </summary>
        public AddHandler AddHandlerCallback;

        /// <summary>
        /// Callback function for removing a node.
        /// </summary>
        public RemoveHandler RemoveHandlerCallback;

        /// <summary>
        /// Callback function for searching the octree.
        /// </summary>
        public SearchHandler SearchHandlerCallback;

        /// <summary>
        /// Callback function for setting or initializing the root of the octree.
        /// </summary>
        public SetRootHandler SetRootHandlerCallback;

        /// <summary>
        /// Callback function for removing all nodes.
        /// </summary>
        public RemoveAllHandler RemoveAllHandlerCallback;



        OctreeNode root = null;
        T space;

        public Octree(T space, AddHandler addHandler, RemoveHandler removeHandler, SearchHandler searchHandler, SetRootHandler setRootHandler, RemoveAllHandler removeAllHandler)
	    {
            this.space = space;
            this.AddHandlerCallback = addHandler;
            this.RemoveHandlerCallback = removeHandler;
            this.SearchHandlerCallback = searchHandler;
            this.SetRootHandlerCallback = setRootHandler;
            this.RemoveAllHandlerCallback = removeAllHandler;

	    }
        public void Traverse(TraversalOperation traversaloperation)
        {
            if (root == null) return;

            root.Traverse(traversaloperation);
            traversaloperation(ref root);
        }

        public List<T> toList()
        {
            List<T> list = new List<T>();

            Traverse((ref Octree<T>.OctreeNode node) =>
            {
                list.Add( node.data );
            });

            return list;
        }
        public T Search(T data)
        {
            //if (Position.X >= Size.X || Position.Y >= Size.Y || Position.Z >= Size.Z || Position.X < 0 || Position.Y < 0 || Position.Z < 0) return default(T);
            if (root == null) return default(T);
            return SearchHandlerCallback( ref root, data );

        }

        /// <summary>
        /// Remove event invoker.
        /// </summary>
        /// <param name="sender">The octree.</param>
        /// <param name="e">The event args.</param>
        public void OnRemove(object sender, EventArgs e)
        {
            if (RemoveEvent != null)
                RemoveEvent(sender, e);
        }

        /// <summary>
        /// Add event invoker.
        /// </summary>
        /// <param name="sender">The octree.</param>
        /// <param name="e">The event args.</param>
        public void OnAdd(object sender, EventArgs e)
        {
            if (AddEvent != null)
                AddEvent(sender, e);
        }
        public void Add(T data)
        {
            if (root == null)
            {
                SetRootHandlerCallback(ref root, this.space);
            }
            AddHandlerCallback(ref root, data);

            OnAdd(this, new EventArgs());
        }
        public void Remove(T data)
        {
            if (root == null) return;
            RemoveHandlerCallback( ref root, data);

            OnRemove(this, new EventArgs());
        }
        public void RemoveAll()
        {
            if (root == null) return;

            RemoveAllHandlerCallback(ref root);

            root.RemoveAll(this.RemoveAllHandlerCallback);

            OnRemove(this, new EventArgs());

        }
        public int Log2( int i )
        {
	        if( i % 2 > 0) return 0;

	        int j = 0;
	        while( i != 1 )
	        {
		        i /= 2;
		        j++;
	        }
	        return i;
        }
    }

}
