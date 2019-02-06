using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using WADV.Plugins.Image.Effects;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Translation;

namespace WADV.Plugins.Image {
    [Serializable]
    public class EffectValue : SerializableValue, ISerializable, IEqualOperator, IStringConverter {
        public IGraphicEffect Effect { get; set; }

        private readonly string _name;
        private readonly Dictionary<string, SerializableValue> _parameters;

        public EffectValue(string name, Dictionary<string, SerializableValue> parameters) {
            _name = name;
            _parameters = parameters;
        }
        
        protected EffectValue(SerializationInfo info, StreamingContext context) {
            _name = info.GetString("name");
            _parameters = (Dictionary<string, SerializableValue>) info.GetValue("parameters", typeof(Dictionary<string, SerializableValue>));
            Effect = EffectPlugin.Create(_name);
            if (Effect == null)
                throw new KeyNotFoundException($"Unable to create effect: expected effect name {_name} not existed");
            Effect.CreateEffect(_parameters);
        }
        
        public override SerializableValue Duplicate() {
            return new EffectValue(_name, _parameters) {
                Effect = Effect
            };
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("parameters", _parameters);
            info.AddValue("name", _name);
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is EffectValue characterValue
                   && characterValue._name == _name
                   && characterValue._parameters.Equals(_parameters);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return _name;
        }
    }
}