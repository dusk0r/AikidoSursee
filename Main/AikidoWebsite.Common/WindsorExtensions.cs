using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AikidoWebsite.Common {
    
    public static class WindsorExtensions {

        public static void Inject(this IWindsorContainer container, object obj) {
            foreach (var pi in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => Attribute.IsDefined(pi, typeof(InjectAttribute)))) {
                var value = pi.GetValue(obj, null);
                
                if (value == null) {
                    var resolved = container.Resolve(pi.PropertyType);

                    if (resolved == null) {
                        throw new ArgumentException(String.Format("Konnte {0} nicht auflösen", pi.PropertyType.Name));
                    } else {
                        pi.SetValue(obj, resolved, null);
                    }
                }
            }

        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class InjectAttribute : Attribute {

    }
}
