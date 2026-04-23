using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace wpf.Models
{
    public class ModelHand : ModelJson
    {
        public Dictionary<string,Dictionary<string,Dictionary<string,string>>> dictTable = new 
                Dictionary<string, Dictionary<string, Dictionary<string,string>>>();

        public Dictionary<string,JObject> dictJson = new Dictionary<string, JObject>();
        
        public ModelHand(){}
        public string ReadJson(string pathFolder)
        {
            ScanFolder(pathFolder);

            dictTable.Clear();
            dictJson.Clear();
            foreach (var pair in dictFolder)
            {
                try
                {
                    ReadJson(pair.Key,pair.Value);
                }
                catch (Exception ex)
                {
                    //return $"Ошибка чтения {pair.Key}: {ex.Message}";
                }
            }
            return "";
        }

        private void ReadJson(string nameFile,string rawJson)
        {
            JArray jsonArray = JArray.Parse(rawJson);
            if (jsonArray == null || jsonArray.Count == 0) return;

            foreach (var item in jsonArray)
            {
                JObject json = (JObject)item;
                if(json == null){continue;}

                SerializeJsonDict(json);
            }
        }

        private void SerializeJsonDict(JObject json)
        {
            //JObject json = JObject.Parse(rawJson);
            //if(json == null){return;}

            string tableName = json["TableName"]?.ToString();
            string handId = json["HandID"]?.ToString();

            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(handId))
            {
                //Console.WriteLine("TableName или HandID отсутствуют");
                return;
            }

            if (!dictTable.ContainsKey(tableName))
            {
                dictTable[tableName] = new Dictionary<string, Dictionary<string, string>>();
            }

            if (!dictTable[tableName].ContainsKey(handId))
            {
                dictTable[tableName][handId] = new Dictionary<string, string>();
            }
            else
            {
                dictTable[tableName].Remove(handId);
            }

            //string players = json["Players"]?.ToString()?.TrimStart('[').TrimEnd(']');
            //string players = json["Players"]?.ToString()?.TrimStart('[').TrimEnd(']').Replace("\"", "");
            string players = string.Join(", ", json["Players"].ToObject<List<string>>());
            string winners = string.Join(", ", json["Winners"].ToObject<List<string>>());
            string winAmount = json["WinAmount"]?.ToString()?.TrimStart('[').TrimEnd(']').Replace("\"", "");

            dictTable[tableName][handId]["Players"] = players;
            dictTable[tableName][handId]["Winners"] = winners;
            dictTable[tableName][handId]["WinAmount"] = winAmount;
        }

        private void SerializeJsonObject(string nameFile,JObject json)
        {
            //JObject json = JObject.Parse(rawJson);
            //if(json == null){return;}

            JObject jsonTable = new JObject
            {
                [json["TableName"].ToString()] = new JObject
                {
                    [json["HandID"].ToString()] = new JObject
                    {
                        ["WinAmount"] = json["WinAmount"],
                        ["Winners"] = json["Winners"],
                        ["Players"] = json["Players"],
                    }
                }
            };
            dictJson.Add(nameFile,jsonTable);   
        }
    }
}