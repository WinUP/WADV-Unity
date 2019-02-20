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
        public GraphicEffect Effect { get; }
        
        public string EffectType { get; }


        public EffectValue(GraphicEffect effect) {
            Effect = effect;
        }
        
        protected EffectValue(SerializationInfo info, StreamingContext context) {
            EffectType = info.GetString("name");
            Effect = EffectPlugin.Create(
                EffectType,
                info.GetSingle("duration"),
                (EasingType) info.GetInt32("easing"),
                (Dictionary<string, SerializableValue>) info.GetValue("parameters", typeof(Dictionary<string, SerializableValue>))
            );
        }
        
        public override SerializableValue Duplicate() {
            return new EffectValue(EffectType, _parameters);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            var (duration, easing, parameters) = Effect.GetParameters();
            info.AddValue("name", EffectType);
            info.AddValue("parameters", parameters);
            info.AddValue("easing", easing);
            info.AddValue("duration", duration);
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