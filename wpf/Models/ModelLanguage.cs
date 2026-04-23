using System;
using System.Resources;
using System.Collections.Generic;
using System.Windows.Input;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace wpf.Models
{

    //BaseModelLanguage//LanguageEnUs//LanguageRuRu//LangHelper//
    public class BaseModelLanguage
    {
        private ResourceManager rm;
        public BaseModelLanguage(string usingFile)
        {
            rm = new ResourceManager(usingFile,Assembly.GetExecutingAssembly()); 
        }
        public void UpdateLanguage()
        {

        }
        public string GetString(string key)
        {
            try
            {
                return rm.GetString(key)!;
            }
            catch(Exception e)
            {
                return "error: " + e;
            }
        }
    }
    public class LanguageEnUs : BaseModelLanguage
    {
        public LanguageEnUs(string usingFile) : base(usingFile){}
    }
    public class LanguageRuRu : BaseModelLanguage
    {
        public LanguageRuRu(string usingFile) : base(usingFile){}
    }
    public static class LangHelper
    {
        private static Dictionary<string,BaseModelLanguage> dictLanguage;
        static LangHelper()
        {
            dictLanguage = new Dictionary<string,BaseModelLanguage>();
            dictLanguage.Add("en-US",new LanguageEnUs("wpf.Resources.lang_en_us"));
            dictLanguage.Add("ru-RU",new LanguageRuRu("wpf.Resources.lang_ru_ru"));
        }
        public static string? GetValue(string key)
        {
            return dictLanguage[CultureInfo.CurrentUICulture.Name].GetString(key);
        }
        public static string ChangeCultureSystem()
        {
            string lang = InputLanguageManager.Current.CurrentInputLanguage.ToString();
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
            return CultureInfo.CurrentCulture.Name + " "+ 
                    CultureInfo.CurrentUICulture.Name + " " + 
                    Thread.CurrentThread.CurrentCulture.Name + " " + 
                    CultureInfo.CurrentUICulture.NativeName;
        }

        // public static async Task<string> Get()
        // {
        //     var assembly = Assembly.GetEntryAssembly();
        //     var resourceStream = assembly?.GetManifestResourceStream("wpf.Resources.lang");
        //     using(var reader = new StreamReader(resourceStream!))
        //     {
        //         return await reader.ReadToEndAsync();
        //     }
        // }

        // public static string GetCulture()
        // {
            //var rm = new ResourceManager("stringtable", System.Reflection.Assembly.GetExecutingAssembly())
            //Dim culture As System.Globalization.CultureInfo;
            //CultureInfo.Culture = DataSet1.Locale;
            //return culture.DisplayName + " " + culture.EnglishName;

        //     CultureInfo culture = CultureInfo.CurrentUICulture;
        //     return culture.NativeName + " " + culture.Name;
        // }
        //
    }


}