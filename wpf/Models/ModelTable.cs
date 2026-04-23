using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

//  {
//   "HandID": 123456789,
//   "TableName": "Berlin #01",
//   "Players": ["Jack", "Henry", "Daniel"],
//   "Winners": ["Jack"],
//   "WinAmount": "1 000,00$"
//  }

namespace wpf.Models
{
    [Serializable]
    public class ModelTable
    {
        [JsonPropertyName("HandID")]
        public string HandID;
        [JsonPropertyName("Players")]
        private List<string> Players;
        [JsonPropertyName("Winners")]
        private List<string> Winners;
        [JsonPropertyName("WinAmount")]
        public string WinAmount;
        //[JsonIgnore]

        public ModelTable(){}
    }
    
}