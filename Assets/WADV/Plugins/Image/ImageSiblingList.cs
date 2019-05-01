using System;
using System.Collections.Generic;
using UnityEngine;

namespace WADV.Plugins.Image {
    public class ImageSiblingList {
        private readonly Dictionary<string, GameObject> _imageIndex = new Dictionary<string, GameObject>();
        private readonly List<(string Name, int Sibling)> _images = new List<(string, int)>();

        public int Detect(int layer) {
            var index = _images.FindIndex(e => e.Sibling > layer);
            return index < 0 ? _images.Count : index;
        }

        public int Add(string name, GameObject target, int layer) {
            if (_imageIndex.ContainsKey(name))
                throw new ArgumentException($"Cannot store sibling: name {name} already existed", nameof(name));
            _imageIndex.Add(name, target);
            var result = (name, layer);
            if (_images.Count == 0) {
                _images.Add(result);
                return 0;
            }
            var index = Detect(layer);
            if (index == _images.Count) {
                _images.Add(result);
                return index;
            }
            _images.Insert(index, result);
            return index;
        }
        
        public GameObject Find(string name) {
            return _imageIndex.TryGetValue(name, out var target) ? target : null;
        }

        public bool Contains(string name) {
            return _imageIndex.ContainsKey(name);
        }

        public GameObject Remove(string name) {
            _images.RemoveAt(_images.FindIndex(e => e.Name == name));
            var result = _imageIndex[name];
            _imageIndex.Remove(name);
            return result;
        }
    }
}