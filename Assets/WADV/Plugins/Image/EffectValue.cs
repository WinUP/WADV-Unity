using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using JetBrains.Annotations;
using WADV.Plugins.Image.Effects;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Translation;

namespace WADV.Plugins.Image {
    [Serializable]
    public class EffectValue : SerializableValue, ISerializable, IEqualOperator, IStringConverter {
        [CanBeNull] public IGraphicEffect Effect { get; }
        
        public string EffectType { get; }

        private readonly Dictionary<string, SerializableValue> _parameters;

        public EffectValue(string effectType, Dictionary<string, SerializableValue> parameters) {
            EffectType = effectType;
            _parameters = parameters;
            Effect = EffectPlugin.Create(EffectType);
            Effect?.SetEffect(parameters);
        }
        
        protected EffectValue(SerializationInfo info, StreamingContext context) {
            EffectType = info.GetString("name");
            _parameters = (Dictionary<string, SerializableValue>) info.GetValue("parameters", typeof(Dictionary<string, SerializableValue>));
            Effect = EffectPlugin.Create(EffectType);
            Effect?.SetEffect(_parameters);
        }
        
        public override SerializableValue Duplicate() {
            return new EffectValue(EffectType, _parameters);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("parameters", _parameters);
            info.AddValue("name", EffectType);
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is EffectValue characterValue
                   && characterValue.EffectType == EffectType
                   && characterValue._parameters.Equals(_parameters);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return EffectType;
        }
    }
}