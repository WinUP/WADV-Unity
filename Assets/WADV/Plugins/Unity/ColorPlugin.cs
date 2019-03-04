using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using WADV.Extensions;
using WADV.Reflection;
using WADV.VisualNovel.Interoperation;
using WADV.VisualNovel.Plugin;
using WADV.VisualNovel.Runtime.Utilities;

namespace WADV.Plugins.Unity {
    [StaticRegistrationInfo("Color")]
    [UsedImplicitly]
    public class ColorPlugin : IVisualNovelPlugin {
        public Task<SerializableValue> Execute(PluginExecuteContext context) {
            byte? r, g, b, a;
            foreach (var (key, value) in context.StringParameters) {
                var name = key.ConvertToString(context.Language);
                if (name.StartsWith("#")) return Task.FromResult<SerializableValue>(ParseHex(name));
                switch (name) {
                    case "Hex":
                        return Task.FromResult<SerializableValue>(ParseHex(StringValue.TryParse(value)));
                    case "R":
                        r = GetColorNumber(value, context.Language);
                        break;
                    case "G":
                        g = GetColorNumber(value, context.Language);
                        break;
                    case "B":
                        b = GetColorNumber(value, context.Language);
                        break;
                    case "A":
                    case "Alpha":
                        a = GetColorNumber(value, context.Language);
                        break;
                    case "AliceBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(240, 248, 255, 255));
                    case "AntiqueWhite":
                        return Task.FromResult<SerializableValue>(new ColorValue(250, 235, 215, 255));
                    case "Aqua":
                    case "Cyan":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 255, 255, 255));
                    case "Aquamarine":
                        return Task.FromResult<SerializableValue>(new ColorValue(127, 255, 212, 255));
                    case "Azure":
                        return Task.FromResult<SerializableValue>(new ColorValue(240, 255, 255, 255));
                    case "Beige":
                        return Task.FromResult<SerializableValue>(new ColorValue(245, 245, 220, 255));
                    case "Bisque":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 228, 196, 255));
                    case "Black":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 0, 0, 255));
                    case "BlanchedAlmond":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 235, 205, 255));
                    case "Blue":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 0, 255, 255));
                    case "BlueViolet":
                        return Task.FromResult<SerializableValue>(new ColorValue(138, 43, 226, 255));
                    case "Brown":
                        return Task.FromResult<SerializableValue>(new ColorValue(165, 42, 42, 255));
                    case "Burlywood":
                        return Task.FromResult<SerializableValue>(new ColorValue(222, 184, 135, 255));
                    case "CadetBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(95, 158, 160, 255));
                    case "Chartreuse":
                        return Task.FromResult<SerializableValue>(new ColorValue(127, 255, 0, 255));
                    case "Chocolate":
                        return Task.FromResult<SerializableValue>(new ColorValue(210, 105, 30, 255));
                    case "Coral":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 127, 80, 255));
                    case "CornflowerBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(100, 149, 237, 255));
                    case "Cornsilk":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 248, 220, 255));
                    case "Crimson":
                        return Task.FromResult<SerializableValue>(new ColorValue(220, 20, 60, 255));
                    case "DarkBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 0, 139, 255));
                    case "DarkCyan":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 139, 139, 255));
                    case "DarkGoldenrod":
                        return Task.FromResult<SerializableValue>(new ColorValue(184, 134, 11, 255));
                    case "DarkGray":
                    case "DarkGrey":
                        return Task.FromResult<SerializableValue>(new ColorValue(169, 169, 169, 255));
                    case "DarkGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 100, 0, 255));
                    case "DarkKhaki":
                        return Task.FromResult<SerializableValue>(new ColorValue(189, 183, 107, 255));
                    case "DarkMagenta":
                        return Task.FromResult<SerializableValue>(new ColorValue(139, 0, 139, 255));
                    case "DarkOliveGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(85, 107, 47, 255));
                    case "DarkOrange":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 140, 0, 255));
                    case "DarkOrchid":
                        return Task.FromResult<SerializableValue>(new ColorValue(153, 50, 204, 255));
                    case "DarkRed":
                        return Task.FromResult<SerializableValue>(new ColorValue(139, 0, 0, 255));
                    case "DarkSalmon":
                        return Task.FromResult<SerializableValue>(new ColorValue(233, 150, 122, 255));
                    case "DarkSeaGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(143, 188, 143, 255));
                    case "DarkSlateBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(72, 61, 139, 255));
                    case "DarkSlateGray":
                    case "DarkSlateGrey":
                        return Task.FromResult<SerializableValue>(new ColorValue(47, 79, 79, 255));
                    case "DarkTurquoise":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 206, 209, 255));
                    case "DarkViolet":
                        return Task.FromResult<SerializableValue>(new ColorValue(148, 0, 211, 255));
                    case "DeepPink":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 20, 147, 255));
                    case "DeepSkyBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 191, 255, 255));
                    case "DimGray":
                    case "DimGrey":
                        return Task.FromResult<SerializableValue>(new ColorValue(105, 105, 105, 255));
                    case "DodgerBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(30, 144, 255, 255));
                    case "Firebrick":
                        return Task.FromResult<SerializableValue>(new ColorValue(178, 34, 34, 255));
                    case "FloralWhite":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 250, 240, 255));
                    case "ForestGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(34, 139, 34, 255));
                    case "Fuchsia":
                    case "Magenta":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 0, 255, 255));
                    case "Gainsboro":
                        return Task.FromResult<SerializableValue>(new ColorValue(220, 220, 220, 255));
                    case "GhostWhite":
                        return Task.FromResult<SerializableValue>(new ColorValue(248, 248, 255, 255));
                    case "Gold":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 215, 0, 255));
                    case "Goldenrod":
                        return Task.FromResult<SerializableValue>(new ColorValue(218, 165, 32, 255));
                    case "Gray":
                    case "Grey":
                    case "X11Gray":
                    case "X11Grey":
                        return Task.FromResult<SerializableValue>(new ColorValue(190, 190, 190, 255));
                    case "WebGray":
                    case "WebGrey":
                        return Task.FromResult<SerializableValue>(new ColorValue(128, 128, 128, 255));
                    case "Green":
                    case "X11Green":
                    case "Lime":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 255, 0, 255));
                    case "WebGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 128, 0, 255));
                    case "GreenYellow":
                        return Task.FromResult<SerializableValue>(new ColorValue(173, 255, 47, 255));
                    case "Honeydew":
                        return Task.FromResult<SerializableValue>(new ColorValue(240, 255, 240, 255));
                    case "HotPink":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 105, 180, 255));
                    case "IndianRed":
                        return Task.FromResult<SerializableValue>(new ColorValue(205, 92, 92, 255));
                    case "Indigo":
                        return Task.FromResult<SerializableValue>(new ColorValue(75, 0, 130, 255));
                    case "Ivory":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 255, 240, 255));
                    case "Khaki":
                        return Task.FromResult<SerializableValue>(new ColorValue(240, 230, 140, 255));
                    case "Lavender":
                        return Task.FromResult<SerializableValue>(new ColorValue(230, 230, 250, 255));
                    case "LavenderBlush":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 240, 245, 255));
                    case "LawnGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(124, 252, 0, 255));
                    case "LemonChiffon":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 250, 205, 255));
                    case "LightBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(173, 216, 230, 255));
                    case "LightCoral":
                        return Task.FromResult<SerializableValue>(new ColorValue(240, 128, 128, 255));
                    case "LightCyan":
                        return Task.FromResult<SerializableValue>(new ColorValue(224, 255, 255, 255));
                    case "LightGoldenrod":
                        return Task.FromResult<SerializableValue>(new ColorValue(250, 250, 210, 255));
                    case "LightGray":
                    case "LightGrey":
                        return Task.FromResult<SerializableValue>(new ColorValue(211, 211, 211, 255));
                    case "LightGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(144, 238, 144, 255));
                    case "LightPink":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 182, 193, 255));
                    case "LightSalmon":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 160, 122, 255));
                    case "LightSeaGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(32, 178, 170, 255));
                    case "LightSkyBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(135, 206, 250, 255));
                    case "LightSlateGray":
                    case "LightSlateGrey":
                        return Task.FromResult<SerializableValue>(new ColorValue(119, 136, 153, 255));
                    case "LightSteelBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(176, 196, 222, 255));
                    case "LightYellow":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 255, 224, 255));
                    case "LimeGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(50, 205, 50, 255));
                    case "Linen":
                        return Task.FromResult<SerializableValue>(new ColorValue(250, 240, 230, 255));
                    case "Maroon":
                    case "X11Maroon":
                        return Task.FromResult<SerializableValue>(new ColorValue(176, 48, 96, 255));
                    case "WebMaroon":
                        return Task.FromResult<SerializableValue>(new ColorValue(128, 0, 0, 255));
                    case "MediumAquamarine":
                        return Task.FromResult<SerializableValue>(new ColorValue(102, 205, 170, 255));
                    case "MediumBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 0, 205, 255));
                    case "MediumOrchid":
                        return Task.FromResult<SerializableValue>(new ColorValue(186, 85, 211, 255));
                    case "MediumPurple":
                        return Task.FromResult<SerializableValue>(new ColorValue(147, 112, 219, 255));
                    case "MediumSeaGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(60, 179, 113, 255));
                    case "MediumSlateBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(123, 104, 238, 255));
                    case "MediumSpringGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 250, 154, 255));
                    case "MediumTurquoise":
                        return Task.FromResult<SerializableValue>(new ColorValue(72, 209, 204, 255));
                    case "MediumVioletRed":
                        return Task.FromResult<SerializableValue>(new ColorValue(199, 21, 133, 255));
                    case "MidnightBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(25, 25, 112, 255));
                    case "MintCream":
                        return Task.FromResult<SerializableValue>(new ColorValue(245, 255, 250, 255));
                    case "MistyRose":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 228, 225, 255));
                    case "Moccasin":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 228, 181, 255));
                    case "NavajoWhite":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 222, 173, 255));
                    case "NavyBlue":
                    case "Navy":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 0, 128, 255));
                    case "OldLace":
                        return Task.FromResult<SerializableValue>(new ColorValue(253, 245, 230, 255));
                    case "Olive":
                        return Task.FromResult<SerializableValue>(new ColorValue(128, 128, 0, 255));
                    case "OliveDrab":
                        return Task.FromResult<SerializableValue>(new ColorValue(107, 142, 35, 255));
                    case "Orange":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 165, 0, 255));
                    case "OrangeRed":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 69, 0, 255));
                    case "Orchid":
                        return Task.FromResult<SerializableValue>(new ColorValue(218, 112, 214, 255));
                    case "PaleGoldenrod":
                        return Task.FromResult<SerializableValue>(new ColorValue(238, 232, 170, 255));
                    case "PaleGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(152, 251, 152, 255));
                    case "PaleTurquoise":
                        return Task.FromResult<SerializableValue>(new ColorValue(175, 238, 238, 255));
                    case "PaleVioletRed":
                        return Task.FromResult<SerializableValue>(new ColorValue(219, 112, 147, 255));
                    case "PapayaWhip":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 239, 213, 255));
                    case "PeachPuff":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 218, 185, 255));
                    case "Peru":
                        return Task.FromResult<SerializableValue>(new ColorValue(205, 133, 63, 255));
                    case "Pink":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 192, 203, 255));
                    case "Plum":
                        return Task.FromResult<SerializableValue>(new ColorValue(221, 160, 221, 255));
                    case "PowderBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(176, 224, 230, 255));
                    case "X11Purple":
                        return Task.FromResult<SerializableValue>(new ColorValue(160, 32, 240, 255));
                    case "WebPurple":
                        return Task.FromResult<SerializableValue>(new ColorValue(128, 0, 128, 255));
                    case "RebeccaPurple":
                        return Task.FromResult<SerializableValue>(new ColorValue(102, 51, 153, 255));
                    case "Red":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 0, 0, 255));
                    case "RosyBrown":
                        return Task.FromResult<SerializableValue>(new ColorValue(188, 143, 143, 255));
                    case "RoyalBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(65, 105, 225, 255));
                    case "SaddleBrown":
                        return Task.FromResult<SerializableValue>(new ColorValue(139, 69, 19, 255));
                    case "Salmon":
                        return Task.FromResult<SerializableValue>(new ColorValue(250, 128, 114, 255));
                    case "SandyBrown":
                        return Task.FromResult<SerializableValue>(new ColorValue(244, 164, 96, 255));
                    case "SeaGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(46, 139, 87, 255));
                    case "Seashell":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 245, 238, 255));
                    case "Sienna":
                        return Task.FromResult<SerializableValue>(new ColorValue(160, 82, 45, 255));
                    case "Silver":
                        return Task.FromResult<SerializableValue>(new ColorValue(192, 192, 192, 255));
                    case "SkyBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(135, 206, 235, 255));
                    case "SlateBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(106, 90, 205, 255));
                    case "SlateGray":
                    case "SlateGrey":
                        return Task.FromResult<SerializableValue>(new ColorValue(112, 128, 144, 255));
                    case "Snow":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 250, 250, 255));
                    case "SpringGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 255, 127, 255));
                    case "SteelBlue":
                        return Task.FromResult<SerializableValue>(new ColorValue(70, 130, 180, 255));
                    case "Tan":
                        return Task.FromResult<SerializableValue>(new ColorValue(210, 180, 140, 255));
                    case "Teal":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 128, 128, 255));
                    case "Thistle":
                        return Task.FromResult<SerializableValue>(new ColorValue(216, 191, 216, 255));
                    case "Tomato":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 99, 71, 255));
                    case "Transparent":
                        return Task.FromResult<SerializableValue>(new ColorValue(0, 0, 0, 0));
                    case "Turquoise":
                        return Task.FromResult<SerializableValue>(new ColorValue(64, 224, 208, 255));
                    case "Violet":
                        return Task.FromResult<SerializableValue>(new ColorValue(238, 130, 238, 255));
                    case "Wheat":
                        return Task.FromResult<SerializableValue>(new ColorValue(245, 222, 179, 255));
                    case "White":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 255, 255, 255));
                    case "WhiteSmoke":
                        return Task.FromResult<SerializableValue>(new ColorValue(245, 245, 245, 255));
                    case "Yellow":
                        return Task.FromResult<SerializableValue>(new ColorValue(255, 255, 0, 255));
                    case "YellowGreen":
                        return Task.FromResult<SerializableValue>(new ColorValue(154, 205, 50, 255));
                }
            }
            r = r ?? 0;
            g = g ?? 0;
            b = b ?? 0;
            a = a ?? 255;
            return Task.FromResult<SerializableValue>(new ColorValue(r.Value, g.Value, b.Value, a.Value));
        }

        public void OnRegister() { }

        public void OnUnregister(bool isReplace) { }

        private static ColorValue ParseHex(string hex) {
            hex = hex.Substring(1).ToUpper();
            if (hex.Length != 3 && hex.Length != 4 && hex.Length != 6 && hex.Length !=8 || !hex.All(e => (e >= 'A' && e <= 'F') || (e >= '0' && e <= '9')))
                throw new ArgumentException($"Unable to create color: hex #{hex} format error");
            if (hex.Length == 3) {
                hex = $"FF{hex[0].ToString()}{hex[0].ToString()}{hex[1].ToString()}{hex[1].ToString()}{hex[2].ToString()}{hex[2].ToString()}";
            } else if (hex.Length == 4) {
                hex = $"{hex[0].ToString()}{hex[0].ToString()}{hex[1].ToString()}{hex[1].ToString()}{hex[2].ToString()}{hex[2].ToString()}{hex[3].ToString()}{hex[3].ToString()}";
            } else if (hex.Length == 6) {
                hex = $"FF{hex}";
            }
            return new ColorValue(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16),
                Convert.ToByte(hex.Substring(6, 2), 16)
            );
        }

        private static byte GetColorNumber(SerializableValue value, string language) {
            if (value is FloatValue floatValue) {
                if (floatValue.value < 0.0F || floatValue.value > 255.0F)
                    throw new ArgumentException($"Unable to create color: value {floatValue.value} must between 0-255");
                if (floatValue.value <= 1.0F) return ColorValue.ToByteColor(floatValue.value);
                return (byte) Mathf.Clamp(Mathf.RoundToInt(floatValue.value), 0, 255);
            }
            if (value is IntegerValue integerValue) {
                if (integerValue.value < 0 || integerValue.value > 255)
                    throw new ArgumentException($"Unable to create color: value {integerValue.value} must between 0-255");
                return (byte) integerValue.value;
            }
            if (value is IIntegerConverter integerConverter) {
                var integerNumber = integerConverter.ConvertToInteger(language);
                if (integerNumber < 0 || integerNumber > 255)
                    throw new ArgumentException($"Unable to create color: value {integerNumber} must between 0-255");
                return (byte) integerNumber;
            }
            var floatNumber = FloatValue.TryParse(value, language);
            if (floatNumber < 0.0F || floatNumber > 255.0F)
                throw new ArgumentException($"Unable to create color: value {floatNumber} must between 0-255");
            if (floatNumber <= 1.0F) return ColorValue.ToByteColor(floatNumber);
            return (byte) Mathf.Clamp(Mathf.RoundToInt(floatNumber), 0, 255);
        }
    }
}