using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.TechChallenge.LambdaProduto.Domain.Entities.Enum
{
    public static class EnumExtensions
    {
        public static string GetDescription(this System.Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            if (field != null)
            {
                DescriptionAttribute attribute =
                    (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

                if (attribute != null)
                {
                    return attribute.Description;
                }
            }

            return value.ToString();
        }
    }
}
