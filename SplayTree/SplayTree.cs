using System;
using System.Collections.Generic;

namespace SplayTree
{
    public class SplayTree<T, TKey> where TKey : IComparable<TKey>
    {
        public class TreeNode
        {
            public TKey key;
            public T value;
            public TreeNode leftSon;
            public TreeNode rightSon;
            public TreeNode parent;

            /// <summary>
            /// Shows whenever this node is leaf or not
            /// </summary>
            public bool Leaf => leftSon == null && rightSon == null;

            /// <summary>
            /// Shows whenever this node is root or not
            /// </summary>
            public bool Root => parent == null;

            public TreeNode(TKey k, T val)
            {
                key = k;
                value = val;
            }

            public void RemoveConnections()
            {
                leftSon = null;
                rightSon = null;
                parent = null;
            }
        }

        public TreeNode root { get; private set; }

        public int Size { get; private set; }

        /// <summary>
        /// Insert new node to Tree
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        public void Insert(T value, TKey key)
        {
            if (this.Size == 0)
            {
                this.root = new TreeNode(key, value);
                Size++;
                return;
            }

            TreeNode current = root;
            TreeNode previous = null;
            int comparedResult = 0;
            while (current != null)
            {
                previous = current;
                comparedResult = current.key.CompareTo(key);
                if (comparedResult == 0) throw new ArgumentException("Key already in Splay Tree");
                current = comparedResult < 0 ? current.rightSon : current.leftSon;
            }

            current = new TreeNode(key, value) {parent = previous};
            if (comparedResult < 0) previous.rightSon = current;
            else previous.leftSon = current;

            Splay(current);
            Size++;
        }

        /// <summary>
        /// Use rotations to correctly order Tree nodes to keep most popular ones at the top of the tree for faster Find
        /// </summary>
        /// <param name="node"></param>
        private void Splay(TreeNode node)
        {
            if (node == null || node.Root) return;

            while (!node.Root)
            {
                if (node.parent.parent == null && node == node.parent.leftSon) RightRotation(node.parent);
                else if (node.parent.parent == null && node == node.parent.rightSon) LeftRotation(node.parent);
                else if (node == node.parent.leftSon && node.parent == node.parent.parent.leftSon)
                {
                    RightRotation(node.parent.parent);
                    RightRotation(node.parent);
                }
                else if (node == node.parent.rightSon && node.parent == node.parent.parent.rightSon)
                {
                    LeftRotation(node.parent.parent);
                    LeftRotation(node.parent);
                }
                else if (node == node.parent.leftSon && node.parent == node.parent.parent.rightSon)
                {
                    RightRotation(node.parent);
                    LeftRotation(node.parent);
                }
                else
                {
                    LeftRotation(node.parent);
                    RightRotation(node.parent);
                }
            }

        }

        /// <summary>
        /// Find item by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="foundedNode"></param>
        /// <returns></returns>
        public bool Find(TKey key, out TreeNode foundedNode)
        {
            foundedNode = default;
            TreeNode current = root;
            TreeNode previous = null;
            while (current != null)
            {
                previous = current;
                int compareResult = key.CompareTo(current.key);
                if (compareResult > 0) current = current.rightSon;
                else if (compareResult < 0) current = current.leftSon;
                else
                {
                    Splay(current);
                    foundedNode = current;
                    return true;
                }
            }

            Splay(previous);
            return false;
        }

        /// <summary>
        /// Find item by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="foundedNode"></param>
        /// <returns></returns>
        public bool Find(TKey key, out T foundedNode)
        {
            foundedNode = default;
            TreeNode current = root;
            TreeNode previous = null;
            while (current != null)
            {
                previous = current;
                int compareResult = key.CompareTo(current.key);
                if (compareResult > 0) current = current.rightSon;
                else if (compareResult < 0) current = current.leftSon;
                else
                {
                    Splay(current);
                    foundedNode = current.value;
                    return true;
                }
            }

            Splay(previous);
            return false;
        }

        /// <summary>
        /// Find used only inside SplayTree -- without splay method at the end
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private TreeNode Find(TKey key)
        {
            TreeNode current = root;
            while (current != null)
            {
                int compareResult = key.CompareTo(current.key);
                if (compareResult > 0) current = current.rightSon;
                else if (compareResult < 0) current = current.leftSon;
                else
                {
                    var foundedNode = current;
                    return foundedNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Delete node from Tree u can use already found node or insert key to find node to be deleted
        /// </summary>
        /// <param name="key"></param>
        /// <param name="nodeToDelete"></param>
        public void Delete(TKey key, TreeNode nodeToDelete = null)
        {
            if (nodeToDelete == null)
            {
                //if (!Find(key, out nodeToDelete)) return; // Find with splay method so deleted node will be root
                nodeToDelete = Find(key);
                if (nodeToDelete == null) return;
            }

            // 1.Case - node is a leaf (have no children)
            if (nodeToDelete.Leaf)
            {
                if (nodeToDelete.Root)
                {
                    root = null;
                    Size--;
                    return;
                }
                if (nodeToDelete == nodeToDelete.parent.leftSon) nodeToDelete.parent.leftSon = null;
                else nodeToDelete.parent.rightSon = null;
            }

            // 2.Case - node have one children
            else if (nodeToDelete.leftSon != null && nodeToDelete.rightSon == null)
            {
                nodeToDelete.leftSon.parent = nodeToDelete.parent;
                if (nodeToDelete == root)
                {
                    root = nodeToDelete.leftSon;
                }
                else
                {
                    if (nodeToDelete == nodeToDelete.parent.leftSon)
                        nodeToDelete.parent.leftSon = nodeToDelete.leftSon;
                    else nodeToDelete.parent.rightSon = nodeToDelete.leftSon;
                }
            }
            else if (nodeToDelete.rightSon != null && nodeToDelete.leftSon == null)
            {
                nodeToDelete.rightSon.parent = nodeToDelete.parent;
                if (nodeToDelete == root)
                {
                    root = nodeToDelete.rightSon;
                }
                else
                {
                    if (nodeToDelete == nodeToDelete.parent.leftSon)
                        nodeToDelete.parent.leftSon = nodeToDelete.rightSon;
                    else nodeToDelete.parent.rightSon = nodeToDelete.rightSon;
                }
            }

            // 3.Case - node has both children
            else
            {
                TreeNode successor = InOrderSuccessor(nodeToDelete);
                if (nodeToDelete == nodeToDelete.parent?.rightSon) nodeToDelete.parent.rightSon = successor;
                else if (nodeToDelete == nodeToDelete.parent?.leftSon) nodeToDelete.parent.leftSon = successor;

                successor.leftSon = nodeToDelete.leftSon;
                if (successor.leftSon != null) successor.leftSon.parent = successor;

                if (successor != nodeToDelete.rightSon)
                {
                    successor.parent.leftSon = successor.rightSon;
                    if(successor.rightSon != null) successor.rightSon.parent = successor.parent;
                    successor.rightSon = nodeToDelete.rightSon;
                }
                if (successor.rightSon != null) successor.rightSon.parent = successor;

                successor.parent = nodeToDelete.parent;
                if (nodeToDelete.Root) root = successor;
            }

            if (!nodeToDelete.Root) Splay(nodeToDelete.parent);
            nodeToDelete.RemoveConnections();
            Size--;
        }

        /// <summary>
        /// Find In Order successor of input param node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public TreeNode InOrderSuccessor(TreeNode node)
        {
            if (node == null) return null;

            if (node.rightSon != null)
            {
                node = node.rightSon;
                while (node.leftSon != null)
                    node = node.leftSon;
                return node;
            }

            TreeNode p = node.parent;
            while (p != null && p.rightSon == node)
            {
                node = p;
                p = node.parent;
            }

            return p;
        }

        /// <summary>
        /// Left rotation of right son of node x
        /// </summary>
        /// <param name="x">Parent of the node we are going to rotate left</param>
        private void LeftRotation(TreeNode x)
        {
            TreeNode y = x.rightSon;
            if (y == null) return;

            x.rightSon = y.leftSon;
            if (y.leftSon != null)
            {
                y.leftSon.parent = x;
            }

            y.parent = x.parent;
            if (x.Root) root = y;
            else if (x == x.parent.leftSon) x.parent.leftSon = y;
            else x.parent.rightSon = y;

            y.leftSon = x;
            x.parent = y;
        }

        /// <summary>
        /// Right rotation of left son of node x
        /// </summary>
        /// <param name="x">Parent of the node we are going to rotate right</param>
        private void RightRotation(TreeNode x)
        {
            TreeNode y = x.leftSon;
            if (y == null) return;

            x.leftSon = y.rightSon;
            if (y.rightSon != null)
            {
                y.rightSon.parent = x;
            }

            y.parent = x.parent;
            if (x.Root) root = y;
            else if (x == x.parent.leftSon) x.parent.leftSon = y;
            else x.parent.rightSon = y;

            y.rightSon = x;
            x.parent = y;
        }

        public void LevelOrder()
        {
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                TreeNode actual = queue.Dequeue();
                Console.WriteLine(actual.key);
                if (actual.leftSon != null) queue.Enqueue(actual.leftSon);
                if (actual.rightSon != null) queue.Enqueue(actual.rightSon);
            }
        }

        /// <summary>
        /// Returns List of InOrder sorted values
        /// </summary>
        /// <returns></returns>
        public List<T> inorderIteration()
        {
            Stack<TreeNode> s = new Stack<TreeNode>();
            List<T> list = new List<T>();
            TreeNode node = root;
            while (true)
            {
                // Go to the left extreme insert all the elements to stack
                while (node != null)
                {
                    s.Push(node);
                    node = node.leftSon;
                }

                // check if Stack is empty, if yes, exit from everywhere
                if (s.Count == 0)
                {
                    return list;
                }

                // pop the element from the stack , print it and add the nodes at
                // the right to the Stack
                node = s.Pop();
                list.Add(node.value);
                node = node.rightSon;
            }
        }

        /// <summary>
        /// Returns List of LevelOrder sorted values
        /// </summary>
        /// <returns></returns>
        public List<T> levelOrderIteration()
        {
            List<T> list = new List<T>();
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                TreeNode actual = queue.Dequeue();
                list.Add(actual.value);
                if (actual.leftSon != null) queue.Enqueue(actual.leftSon);
                if (actual.rightSon != null) queue.Enqueue(actual.rightSon);
            }

            return list;
        }
    }
}
