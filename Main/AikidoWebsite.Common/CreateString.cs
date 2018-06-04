using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AikidoWebsite.Common
{

    public class CreateString
    {
        private Object Instance { get; set; }

        private List<string> strings;

        public static CreateString From(Object obj)
        {
            return new CreateString(obj);
        }

        private CreateString(Object instance)
        {
            this.Instance = instance;
            this.strings = new List<string>();
        }

        public CreateString Add(object obj)
        {
            strings.Add((obj == null) ? "null" : obj.ToString());
            return this;
        }

        public CreateString Add(string name, object obj)
        {
            var str = String.Format("{0}: {1}", name, (obj == null) ? "null" : obj.ToString());
            strings.Add(str);
            return this;
        }

        public CreateString UseProperties()
        {
            UsePropertiesWhere(pi => true);

            return this;
        }

        public CreateString UsePropertiesWhere(Predicate<PropertyInfo> pred)
        {
            foreach (var pi in Instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pred(pi))
                {
                    Add(pi.Name, pi.GetValue(Instance, null));
                }
            }

            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Instance.GetType().Name);
            sb.Append(" [ ");
            sb.Append(String.Join(", ", strings));
            sb.Append(" ]");

            return sb.ToString();
        }

    }
}
