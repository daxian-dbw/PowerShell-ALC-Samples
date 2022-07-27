using System;
using System.Management.Automation;
using Newtonsoft.Json;

namespace assembly.conflict
{
    [JsonConverter(typeof(DummyConverter))]
    public enum TestEnum
    {
        Abc,
        Def
    }

    public class DummyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }

    [Cmdlet("Test", "DummyCommand")]
    public class DummyCommand : PSCmdlet
    {
        protected override void EndProcessing()
        {
            string typeName = typeof(JsonConvert).FullName;
            Console.WriteLine($"Using '{typeName}' from '{GetAssemblyName()}'");
        }

        private string GetAssemblyName()
        {
            return typeof(JsonConvert).Assembly.FullName;
        }

        public static void PrintConverterName()
        {
            Console.WriteLine(typeof(DummyConverter).FullName);
        }
    }
}
