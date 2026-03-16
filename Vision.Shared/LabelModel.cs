using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Vision.Shared
{
    public class ColorJsonConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteValue(ColorTranslator.ToHtml(value));
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return existingValue;

            string s = reader.Value.ToString();
            if (string.IsNullOrWhiteSpace(s))
                return existingValue;

            return ColorTranslator.FromHtml(s);
        }
    }

    public enum ShowType { AlwaysShow, NG_Only, AlwaysHide }
    public class LabelModel
    {
        [Category("General")]
        public string Tag { get; set; } = "Label";
        
        [Category("General")]
        public ShowType showType { get; set; } = ShowType.AlwaysShow;
        
        [Category("Text")]
        public string Text { get; set; } = "Label";

        [Category("Position")]
        public float RelativeX { get; set; }

        [Category("Position")]
        public float RelativeY { get; set; }

        [Category("Size")]
        public int Width { get; set; }

        [Category("Size")]
        public int Height { get; set; }

        [Category("Appearance")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color BackColor { get; set; } = Color.White;

        [Category("Appearance")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color ForeColor { get; set; } = Color.Black;

        [Category("Appearance")]
        public string FontName { get; set; }

        [Category("Appearance")]
        public float FontSize { get; set; }

        [Category("Tools")]
        [DisplayName("Tool List")]
        public List<Tool> ToolList { get; set; } = new List<Tool>();

        [JsonIgnore]
        [Browsable(false)]
        public int Result { get; set; }
    }

    public class Tool
    {
        [Category("Tool Info")]
        [DisplayName("Process Order")]
        public int ProgramNo { get; set; }

        [Category("Tool Info")]
        public int ToolNo { get; set; }

        [Category("Tool Info")]
        public int CameraNo { get; set; }

        [Category("Tool Info")]
        public bool PosAdjust { get; set; } = true;

        [Browsable(false)]
        public int Result { get; set; } // 0: NA, 1: OK, 2: NG
        public override string ToString()
        {
            return "VisionTool";
        }

    }
}
