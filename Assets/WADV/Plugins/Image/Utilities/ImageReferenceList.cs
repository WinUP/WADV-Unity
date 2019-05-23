using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Plugins.Effect;

namespace WADV.Plugins.Image.Utilities {
    public class ImageReference {
        public GameObject RenderTarget;
        public int Sibling;
        [CanBeNull] public GraphicEffect Effect;
    }
    
    public class ImageReferenceList {
        private readonly List<(string Name, ImageReference Reference)> _references = new List<(string Name, ImageReference Reference)>();
        private ImageReference[] _cache;

        public ImageReferenceList() {
            UpdateCache();
        }
        
        public int Detect(int layer) {
            var index = Array.FindIndex(_cache, e => e.Sibling > layer);
            return index < 0 ? _cache.Length : index;
        }

        public int Add(string name, GameObject target, int layer) {
            if (Find(name) != null)
                throw new ArgumentException($"Cannot store image reference: name {name} already existed", nameof(name));
            var reference = new ImageReference {
                RenderTarget = target,
                Sibling = Detect(layer)
            };
            if (reference.Sibling == _cache.Length) {
                _references.Add((name, reference));
            } else {
                _references.Insert(reference.Sibling, (name, reference));
            }
            UpdateCache();
            return reference.Sibling;
        }
        
        public ImageReference Find(string name) {
            var index = _references.FindIndex(e => e.Name == name);
            return index < 0 ? null : _references[index].Reference;
        }

        public bool Contains(string name) {
            return _references.Any(e => e.Name == name);
        }

        [CanBeNull]
        public ImageReference Remove(string name) {
            var index = _references.FindIndex(e => e.Name == name);
            if (index < 0) return null;
            var result = _references[index];
            _references.RemoveAt(index);
            UpdateCache();
            return result.Reference;
        }

        private void UpdateCache() {
            _cache = _references.Select(e => e.Reference).ToArray();
        }
    }
}