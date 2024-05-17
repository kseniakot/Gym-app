﻿//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gym.Model
{

    public class  Notification
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Event { get; set; }

        public Payment Object { get; set; }
    }



    public class Payment
    {
        //[JsonProperty("id")]
        public string Id { get; set; }

       // [JsonProperty("status")]
        public string Status { get; set; }
        public Amount Amount { get; set; }

       // [JsonProperty("paid")]
        public bool Paid { get; set; }

     //   [JsonProperty("confirmation")]
        public Confirmation Confirmation { get; set; }

       // [JsonProperty("created_at")]
       
       ///public DateTimeOffset CreatedAt { get; set; }
       

       [JsonIgnore]
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order? Order { get; set; }


    }
}
