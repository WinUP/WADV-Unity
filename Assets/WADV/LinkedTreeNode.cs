using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace WADV {
    /// <summary>
    /// 表示一个链接树节点
    /// </summary>
    public class LinkedTreeNode : IEnumerable<LinkedTreeNode> {
        /// <summary>
        /// 获取或设置节点可用性
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 获取或设置节点的父节点
        /// </summary>
        [CanBeNull]
        public LinkedTreeNode Parent {
            get => _parent;
            set => SetParent(value);
        }

        /// <summary>
        /// 获取或设置节点的同级同父节点遍历优先级（越大越优先，同级越先加入越优先）
        /// </summary>
        public int Priority {
            get => _priority;
            set {
                _priority = value;
                SetParent(_parent);
            }
        }

        /// <summary>
        /// 获取隶属此节点的一级子节点的数目
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 获取最近的遍历优先级大于此节点的同级同父节点子节点
        /// </summary>
        [CanBeNull] public LinkedTreeNode Left { get; private set; }
        /// <summary>
        /// 获取最近的遍历优先级小于此节点的同级同父节点子节点
        /// </summary>
        [CanBeNull] public LinkedTreeNode Right { get; private set; }
        /// <summary>
        /// 获取隶属此节点的优先级最大的子节点（或之一）
        /// </summary>
        [CanBeNull] public LinkedTreeNode FirstChild { get; private set; }

        private LinkedTreeNode _parent;
        private int _priority;
        
        /// <summary>
        /// 创建一个一级子节点
        /// </summary>
        /// <param name="priority">子节点遍历优先级</param>
        /// <returns></returns>
        public LinkedTreeNode CreateChild(int priority = 0) {
            return new LinkedTreeNode() {Priority = priority, Parent = this};
        }
        
        /// <summary>
        /// 查找第一个符合条件的子节点
        /// </summary>
        /// <param name="prediction">用于确定目标子节点的函数</param>
        /// <returns></returns>
        [CanBeNull]
        public LinkedTreeNode FindChild(Func<LinkedTreeNode, bool> prediction) {
            var processingChild = FirstChild;
            while (processingChild != null) {
                if (prediction.Invoke(processingChild)) {
                    return processingChild;
                }
                processingChild = processingChild.Right;
            }
            return null;
        }
        
        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="node">目标子节点</param>
        /// <returns></returns>
        public bool RemoveChild(LinkedTreeNode node) {
            return RemoveChild(e => e == node);
        }

        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="prediction">用于确定目标子节点的函数</param>
        /// <returns></returns>
        public bool RemoveChild(Func<LinkedTreeNode, bool> prediction) {
            var processingChild = FirstChild;
            while (processingChild != null) {
                if (prediction.Invoke(processingChild)) {
                    processingChild.Destroy();
                    return true;
                }
                processingChild = processingChild.Right;
            }
            return false;
        }
        
        /// <summary>
        /// 销毁当前节点
        /// </summary>
        public void Destroy() {
            if (_parent != null) {
                if (_parent.FirstChild == this) {
                    _parent.FirstChild = Right;
                }
                --_parent.Count;
                _parent = null;
            }
            if (Left != null) {
                Left.Right = Right;
            }
            if (Right != null) {
                Right.Left = Left;
            }
        }
        
        private void SetParent(LinkedTreeNode parent) {
            if (_parent != null) {
                Destroy();
            }
            if (parent == null) return;
            _parent = parent;
            ++_parent.Count;
            if (_parent.FirstChild == null) {
                _parent.FirstChild = this;
                return;
            }
            if (_parent.FirstChild._priority < _priority) {
                _parent.FirstChild.Left = this;
                Right = _parent.FirstChild;
                _parent.FirstChild = this;
                return;
            }
            var leftNode = _parent.FirstChild;
            while (true) {
                if (leftNode?.Right == null || leftNode.Right._priority < _priority) break;
                leftNode = leftNode.Right;
            }
            if (leftNode.Right != null) {
                leftNode.Right.Left = this;
            }
            Right = leftNode.Right;
            Left = leftNode;
            leftNode.Right = this;
        }
        
        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<LinkedTreeNode> GetEnumerator() {
            return new LinkedTreeNodeEmulator(this);
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        /// <summary>
        /// 用于深度优先遍历链接树的遍历器
        /// </summary>
        private class LinkedTreeNodeEmulator : IEnumerator<LinkedTreeNode> {
            private readonly LinkedTreeNode _root;
            private Stack<LinkedTreeNode> _mapStack = new Stack<LinkedTreeNode>();

            public LinkedTreeNodeEmulator(LinkedTreeNode root) {
                _root = root;
            }

            private void MoveToNextEnabledNode() {
                if (Current == null) return;
                do {
                    if (Current.Right != null) {
                        Current = Current.Right;
                        continue;
                    }
                    while (_mapStack.Count > 0) {
                        Current = _mapStack.Pop();
                        if (Current == null) return;
                        if (Current.Right == null) continue;
                        Current = Current.Right;
                        break;
                    }
                } while (Current != null && !Current.Enabled);
                if (Current == _root) {
                    Current = null;
                }
            }
            
            public bool MoveNext() {
                if (Current == null) {
                    if (!_root.Enabled) return false;
                    Current = _root;
                    return true;
                }
                if (Current.FirstChild != null) {
                    _mapStack.Push(Current);
                    Current = Current.FirstChild;
                    if (Current.Enabled) {
                        return true;
                    }
                    MoveToNextEnabledNode();
                } else {
                    MoveToNextEnabledNode();
                }
                return Current != null;
            }

            public void Reset() {
                Current = null;
                _mapStack = new Stack<LinkedTreeNode>();
            }

            public LinkedTreeNode Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() {
                GC.SuppressFinalize(this);
            }
        }
    }

    /// <inheritdoc cref="LinkedTreeNode" />
    /// <summary>
    /// 表示一个异构链接树节点
    /// </summary>
    /// <typeparam name="T">节点内容类型</typeparam>
    public class LinkedTreeNode<T> : LinkedTreeNode, IEnumerable<(LinkedTreeNode Node, T Value)> {
        /// <summary>
        /// 获取或设置节点内容
        /// </summary>
        public T Content { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建一个链接树节点
        /// </summary>
        /// <param name="content">节点内容</param>
        public LinkedTreeNode(T content) {
            Content = content;
        }

        /// <summary>
        /// 创建一个一级子节点
        /// </summary>
        /// <param name="content">子节点内容</param>
        /// <param name="priority">子节点遍历优先级</param>
        /// <returns></returns>
        public LinkedTreeNode<T> CreateChild(T content, int priority = 0) {
            return new LinkedTreeNode<T>(content) {Priority = priority, Parent = this};
        }
        
        /// <summary>
        /// 查找第一个符合条件的子节点
        /// </summary>
        /// <param name="content">子节点内容</param>
        /// <returns></returns>
        [CanBeNull]
        public LinkedTreeNode<T> FindChild(T content) {
            return FindChild(e => e is LinkedTreeNode<T> node && node.Content.Equals(content)) as LinkedTreeNode<T>;
        }
        
        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="item">子节点内容</param>
        /// <returns></returns>
        public bool RemoveChild(T item) {
            return RemoveChild(e => e is LinkedTreeNode<T> node && node.Content.Equals(item));
        }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public new IEnumerator<(LinkedTreeNode Node, T Value)> GetEnumerator() {
            return new LinkedTreeNodeEmulator(this);
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        /// <summary>
        /// 用于深度优先遍历链接树的遍历器
        /// </summary>
        private class LinkedTreeNodeEmulator : IEnumerator<(LinkedTreeNode Node, T Value)> {
            private readonly LinkedTreeNode<T> _root;
            private Stack<LinkedTreeNode> _mapStack = new Stack<LinkedTreeNode>();

            public LinkedTreeNodeEmulator(LinkedTreeNode<T> root) {
                _root = root;
            }

            private void MoveToNextEnabledNode() {
                if (Current.Node == null) return;
                do {
                    var right = Current.Node.Right;
                    while (right != null && !(right.Enabled && right is LinkedTreeNode<T>)) {
                        right = right.Right;
                    }
                    if (right != null) {
                        UpdateValue(right as LinkedTreeNode<T>);
                        continue;
                    }
                    while (_mapStack.Count > 0) {
                        var parent = _mapStack.Pop();
                        if (parent == null) return;
                        if (parent == _root) {
                            UpdateValue(_root);
                            continue;
                        }
                        if (parent.Right == null || !(parent.Right is LinkedTreeNode<T> parentNode)) continue;
                        UpdateValue(parentNode);
                        break;
                    }
                } while (Current.Node != null && !Current.Node.Enabled);
                if (Current.Node == _root) {
                    Current = (Node: null, Value: default);
                }
            }
            
            public bool MoveNext() {
                if (Current.Node == null) {
                    if (!_root.Enabled) return false;
                    Current = (Node: _root, Value: _root.Content);
                    return true;
                }
                if (Current.Node.FirstChild != null) {
                    var child = Current.Node.FirstChild;
                    while (child != null && !(child.Enabled && child is LinkedTreeNode<T>)) {
                        child = child.Right;
                    }
                    if (child != null) {
                        _mapStack.Push(Current.Node);
                        UpdateValue(child as LinkedTreeNode<T>);
                        return true;
                    }
                    MoveToNextEnabledNode();
                } else {
                    MoveToNextEnabledNode();
                }
                return Current.Node != null;
            }

            public void Reset() {
                Current = (Node: null, Value: default);
                _mapStack = new Stack<LinkedTreeNode>();
            }

            public (LinkedTreeNode Node, T Value) Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() {
                GC.SuppressFinalize(this);
            }

            private void UpdateValue(LinkedTreeNode<T> node) {
                Current = (Node: node, Value: node.Content);
            }
        }
    }
}