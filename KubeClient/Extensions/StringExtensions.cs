using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeClient.Extensions
{
    public static class StringExtensions
    {
        public static T ToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
