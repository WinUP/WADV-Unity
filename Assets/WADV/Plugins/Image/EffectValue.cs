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


        public EffectValue([NotNull] GraphicEffect effect) {
            Effect = effect;
        }
        
        protected EffectValue(SerializationInfo info, StreamingContext context) {
            EffectType = info.GetString("name");
            Effect = EffectPlugin.Create(
                EffectType,
                (Dictionary<string, SerializableValue>) info.GetValue("parameters", typeof(Dictionary<string, SerializableValue>)),
                info.GetSingle("duration"),
                (EasingType) info.GetInt32("easing")
            );
        }
        
        public override SerializableValue Duplicate() {
            return new EffectValue(Effect);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("name", EffectType);
            info.AddValue("duration", Effect.Duration);
            info.AddValue("easing", (int) Effect.EasingType);
            info.AddValue("parameters", Effect.Parameters);
        }

        public bool EqualsWith(SerializableValue target, string language = TranslationManager.DefaultLanguage) {
            return target is EffectValue effectValue
                   && effectValue.EffectType == EffectType
                   && effectValue.Effect.Equals(Effect);
        }

        public string ConvertToString(string language = TranslationManager.DefaultLanguage) {
            return EffectType;
        }
    }
}