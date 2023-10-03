using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Windows.Media;

namespace MemoMinder
{
    [Serializable]
    public class DataMemo : ICloneable
    {
        public Brush BackgroundWindow { get; set; }
        public Brush BackgroundTextBox { get; set; }
        public Brush TextBoxForeground { get; set; }
        public Brush CaptionForeground { get; set; }
        public FontFamily? TextBoxfontFamily { get; set; }
        public FontFamily? CaptionFontFamily { get; set; }
        public string BackgroundWindowColorPath { get; set; }
        public string BackgroundTextBoxPath { get; set; }
        public string CaptionText { get; set; }
        public string MemoText { get; set; }
        public double TextBoxMargin { get; set; }
        public double TextBoxFontSize { get; set; }
        public bool VerticalScrollBarVisibility { get; set; }
        public bool IsToggleWindow { get; set; }
        public bool IsCaptionActive { get; set; }
        public double CaptionFontSize { get; set; }
        public bool IsUnderlineCaption { get; set; }
        public double HeightWindow { get; set; }
        public double WidthWindow { get; set; }
        public DataMemo() { }

        public object Clone() =>
             MemberwiseClone();
        
    }
    [Serializable]
    class DataWindow
    {
        public string? LastOpenedFile { get; set; }
    }
    public class BrushConverter : JsonConverter<Brush>
    {
        public override Brush Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string colorString = reader.GetString();
                if (ColorConverter.ConvertFromString(colorString) is Color color)
                {
                    return new SolidColorBrush(color);
                }
            }

            throw new JsonException("Unable to deserialize Brush.");
        }

        public override void Write(Utf8JsonWriter writer, Brush value, JsonSerializerOptions options)
        {
            if (value is SolidColorBrush solidColorBrush)
            {
                writer.WriteStringValue(solidColorBrush.Color.ToString());
            }
            else
            {
                throw new NotSupportedException("Only SolidColorBrush serialization is supported.");
            }
        }

        public override bool CanConvert(Type typeToConvert) => typeof(Brush).IsAssignableFrom(typeToConvert);
    }

    public class FontFamilyConverter : JsonConverter<FontFamily>
    {
        public override FontFamily Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string fontFamilyName = reader.GetString();
                return new FontFamily(fontFamilyName);
            }

            throw new JsonException("Unable to deserialize FontFamily.");
        }

        public override void Write(Utf8JsonWriter writer, FontFamily value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Source);
        }
        public override bool CanConvert(Type typeToConvert) => typeof(FontFamily).IsAssignableFrom(typeToConvert);
    }

}
