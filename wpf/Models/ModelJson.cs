using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;


namespace wpf.Models
{
    [Serializable]
    public class ModelJson
    {
        public Dictionary<string,string> dictFolder = new Dictionary<string, string>();

        public ModelJson(){}

        protected void ScanFolder(string pathFolder)
        {
            var jsonFiles = Directory.GetFiles(pathFolder, "*.json", SearchOption.AllDirectories);

            dictFolder.Clear();
            foreach (var filePath in jsonFiles)
            {
                try
                {
                    string content = File.ReadAllText(filePath);
                    dictFolder.Add(filePath,content);
                }
                catch (Exception ex)
                {
                    //return $"Ошибка чтения {filePath}: {ex.Message}";
                }
            }
        }
        
        public void LoadJson(string nameFile)
        {

        }
    }
}