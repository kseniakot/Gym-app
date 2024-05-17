using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
//using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Gym.Model
{
    public class Amount
    {
        

     //   [JsonProperty("value")]
        public string Value { get; set; }

      //  [JsonProperty("currency")]
        public string Currency { get; set; }

        //[JsonIgnore]
        //public int OrderId { get; set; }
        //[JsonIgnore]
        //public Order Order { get; set; }

        //[JsonIgnore]
        //public string PaymentId { get; set; }
        //[JsonIgnore]
        //public Payment Payment { get; set; }

    }

    public class Confirmation
    {
       
      //  [JsonProperty("type")]
        public string Type { get; set; }

      //  [JsonProperty("confirmation_url")]
        public string Confirmation_url { get; set; }

        //[JsonIgnore]
        //public string? PaymentId { get; set; }
        //[JsonIgnore]
        //public Payment? Payment { get; set; }
    }
   
    public class Redirection
    {
       
        //  [JsonProperty("type")]
        public string Type { get; set; }

        //  [JsonProperty("confirmation_url")]
        public string Return_url { get; set; }
        //[JsonIgnore]
        //public int OrderId { get; set; }
        //[JsonIgnore]
        //public Order Order { get; set; }


    }


    public class Order
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }  

      //  [JsonProperty("amount")]
        public Amount Amount { get; set; }

      //  [JsonProperty("capture")]
        public bool Capture { get; set; }

      //  [JsonProperty("confirmation")]
        public Redirection Confirmation { get; set; }

        //  [JsonProperty("description")]

        //public string Description { get; set; }

        [JsonIgnore]
        public Payment? Payment { get; set; }
        [JsonIgnore]
        public int? MembershipId { get; set; }
        [JsonIgnore]
        public Membership? Membership { get; set; }
    }



   
}
