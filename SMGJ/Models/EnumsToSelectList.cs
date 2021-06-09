using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SMGJ.Models
{
    public class EnumsToSelectList<T>
    {
        public static SelectList GetSelectList()
        {

            var list = new List<KeyValuePair<Enum, string>>();
            foreach (Enum value in Enum.GetValues(typeof(T)))
            {
                string description = value.ToString();
                FieldInfo fieldInfo = value.GetType().GetField(description);
                var attribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).First();
                if (attribute != null)
                {
                    description = (attribute as DescriptionAttribute).Description;
                }
                list.Add(new KeyValuePair<Enum, string>(value, description));
            }

            var values = from e in list
                         select new { ID = Convert.ToInt16(e.Key), Name = e.Value };

            return new SelectList(values, "ID", "Name");
        }
    }
}