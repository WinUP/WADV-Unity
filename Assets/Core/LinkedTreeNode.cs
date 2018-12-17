using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Core {
    /// <summary>
    /// 表示一个链接树节点
    /// </summary>
    /// <typeparam name="T">节点内容类型</typeparam>
    public class LinkedTreeNode<T> : IEnumerable<T> {
        /// <summary>
        /// 获取或设置节点内容
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// 获取或设置节点可用性
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 获取或设置节点的父节点
        /// </summary>
        [CanBeNull]
        public LinkedTreeNode<T> Parent {
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
        [CanBeNull] public LinkedTreeNode<T> Left { get; private set; }
        /// <summary>
        /// 获取最近的遍历优先级小于此节点的同级同父节点子节点
        /// </summary>
        [CanBeNull] public LinkedTreeNode<T> Right { get; private set; }
        /// <summary>
        /// 获取隶属此节点的优先级最大的子节点（或之一）
        /// </summary>
        [CanBeNull] public LinkedTreeNode<T> FirstChild { get; private set; }

        private LinkedTreeNode<T> _parent;
        private int _priority;
        
        /// <summary>
        /// 创建一个链接树节点
        /// </summary>
        public LinkedTreeNode() {}

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
        public LinkedTreeNode<T> FindNode(T content) {
            return FindNode(e => e.Content.Equals(content));
        }
        
        /// <summary>
        /// 查找第一个符合条件的子节点
        /// </summary>
        /// <param name="prediction">用于确定目标子节点的函数</param>
        /// <returns></returns>
        public LinkedTreeNode<T> FindNode(Func<LinkedTreeNode<T>, bool> prediction) {
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
        /// 递归查找第一个符合条件的子节点
        /// </summary>
        /// <param name="content">子节点内容</param>
        /// <returns></returns>
        public LinkedTreeNode<T> FindNodeRecursion(T content) {
            return FindNodeRecursion(e => e.Content.Equals(content));
        }
        
        /// <summary>
        /// 递归查找第一个符合条件的子节点
        /// </summary>
        /// <param name="prediction">用于确定目标子节点的函数</param>
        /// <returns></returns>
        public LinkedTreeNode<T> FindNodeRecursion(Func<LinkedTreeNode<T>, bool> prediction) {
            var result = FindNode(prediction);
            if (result != null) return result;
            var processingChild = FirstChild;
            while (processingChild != null) {
                result = processingChild.FindNodeRecursion(prediction);
                if (result != null) return result;
                processingChild = processingChild.Right;
            }
            return null;
        }
        
        /// <summary>
        /// 查找所有符合条件的子节点
        /// </summary>
        /// <param name="content">子节点内容</param>
        /// <returns></returns>
        public IEnumerable<LinkedTreeNode<T>> FindAllNodes(T content) {
            return FindAllNodes(e => e.Content.Equals(content));
        }
        
        /// <summary>
        /// 查找所有符合条件的子节点
        /// </summary>
        /// <param name="prediction">用于确定目标子节点的函数</param>
        /// <returns></returns>
        public IEnumerable<LinkedTreeNode<T>> FindAllNodes(Func<LinkedTreeNode<T>, bool> prediction) {
            var processingChild = FirstChild;
            var result = new List<LinkedTreeNode<T>>();
            while (processingChild != null) {
                if (prediction.Invoke(processingChild)) {
                    result.Add(processingChild);
                }
                processingChild = processingChild.Right;
            }
            return result;
        }
        
        /// <summary>
        /// 递归查找所有符合条件的子节点
        /// </summary>
        /// <param name="content">子节点内容</param>
        /// <returns></returns>
        public IEnumerable<LinkedTreeNode<T>> FindAllNodesRecursion(T content) {
            return FindAllNodesRecursion(e => e.Content.Equals(content));
        }
        
        /// <summary>
        /// 递归查找所有符合条件的子节点
        /// </summary>
        /// <param name="prediction">用于确定目标子节点的函数</param>
        /// <returns></returns>
        public IEnumerable<LinkedTreeNode<T>> FindAllNodesRecursion(Func<LinkedTreeNode<T>, bool> prediction) {
            var result = new List<LinkedTreeNode<T>> {FindNode(prediction)};
            var processingChild = FirstChild;
            while (processingChild != null) {
                result.AddRange(processingChild.FindAllNodesRecursion(prediction));
                processingChild = processingChild.Right;
            }
            return result;
        }
        
        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="item">子节点内容</param>
        /// <returns></returns>
        public bool Remove(T item) {
            return Remove(e => e.Content.Equals(item));
        }

        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="node">目标子节点</param>
        /// <returns></returns>
        public bool Remove(LinkedTreeNode<T> node) {
            return Remove(e => e == node);
        }

        /// <summary>
        /// 删除子节点
        /// </summary>
        /// <param name="prediction">用于确定目标子节点的函数</param>
        /// <returns></returns>
        public bool Remove(Func<LinkedTreeNode<T>, bool> prediction) {
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
        /// 递归删除子节点
        /// </summary>
        /// <param name="node">子节点内容</param>
        /// <returns></returns>
        public bool RemoveChildRecursion(T node) {
            return RemoveChildRecursion(e => e.Content.Equals(node));
        }

        /// <summary>
        /// 递归删除子节点
        /// </summary>
        /// <param name="node">目标子节点</param>
        /// <returns></returns>
        public bool RemoveChildRecursion(LinkedTreeNode<T> node) {
            return RemoveChildRecursion(e => e == node);
        }

        /// <summary>
        /// 递归删除子节点
        /// </summary>
        /// <param name="prediction">用于确定目标子节点的函数</param>
        /// <returns></returns>
        public bool RemoveChildRecursion(Func<LinkedTreeNode<T>, bool> prediction) {
            if (Remove(prediction)) return true;
            var processingChild = FirstChild;
            while (processingChild != null) {
                if (processingChild.RemoveChildRecursion(prediction)) return true;
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

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() {
            return new LinkedTreeNodeEmulator(this);
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        private void SetParent(LinkedTreeNode<T> parent) {
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
        /// 用于深度优先遍历链接树的遍历器
        /// </summary>
        private class LinkedTreeNodeEmulator : IEnumerator<T> {
            private LinkedTreeNode<T> _root;
            private LinkedTreeNode<T> _currentNode;
            private Stack<LinkedTreeNode<T>> _mapStack = new Stack<LinkedTreeNode<T>>();

            public LinkedTreeNodeEmulator(LinkedTreeNode<T> root) {
                _root = root;
            }

            private void MoveToNextEnabledNode() {
                if (_currentNode == null) return;
                do {
                    if (_currentNode.Right != null) {
                        _currentNode = _currentNode.Right;
                        continue;
                    }
                    while (_mapStack.Count > 0) {
                        _currentNode = _mapStack.Pop();
                        if (_currentNode.Right == null) continue;
                        _currentNode = _currentNode.Right;
                        break;
                    }
                } while (_currentNode != null && !_currentNode.Enabled);
                if (_currentNode == _root) {
                    _currentNode = null;
                }
            }
            
            public bool MoveNext() {
                if (_currentNode == null) {
                    if (!_root.Enabled) return false;
                    _currentNode = _root;
                    UpdateValue();
                    return true;
                }
                if (_currentNode.FirstChild != null) {
                    _mapStack.Push(_currentNode);
                    _currentNode = _currentNode.FirstChild;
                    if (_currentNode.Enabled) {
                        UpdateValue();
                        return true;
                    }
                    MoveToNextEnabledNode();
                    UpdateValue();
                } else {
                    MoveToNextEnabledNode();
                    UpdateValue();
                }
                return _currentNode != null;
            }

            public void Reset() {
                _currentNode = null;
                _mapStack = new Stack<LinkedTreeNode<T>>();
                UpdateValue();
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() {
                _root = null;
                _mapStack = null;
            }

            private void UpdateValue() {
                Current = _currentNode == null ? default(T) : _currentNode.Content;
            }
        }
    }
}