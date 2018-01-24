//using AikidoWebsite.Data.ValueObjects;
//using Raven.Imports.Newtonsoft.Json;
//using Raven.Imports.Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace AikidoWebsite.Data.Converter {

//    public class GroupSetConverter : JsonConverter {

//        public override bool CanConvert(Type objectType) {
//            return typeof(ISet<Gruppe>).IsAssignableFrom(objectType);
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
//            JArray array = JArray.Load(reader);
//            HashSet<Gruppe> groups = new HashSet<Gruppe>();

//            foreach (var element in array) {
//                var group = (Gruppe)Enum.Parse(typeof(Gruppe), element.ToString());
//                groups.Add(group);
//            }

//            return groups;
//        }

//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
//            var set = value as IEnumerable<Gruppe>;

//            writer.WriteStartArray();
//            foreach (var entry in set) {
//                writer.WriteValue(entry.ToString());
//            }
//            writer.WriteEndArray();
//        }
//    }
//}
