﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class ODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; } = new();
    }   

}
