using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CostDaily.DataModel
{
    [DataContract]
    public class LocalizationDictionary
    {

        [DataMember]
        public string Name;

        [DataMember]
        public Dictionary<string, string> LanguageDictionary { get; set; }


        public LocalizationDictionary()
        {
            LanguageDictionary = new Dictionary<string, string>();
        }
    }

    [DataContract]
    public class LocalizationSource
    {
        private string _fileSuffix = ".json";
        const string folderName = "CostsDaily_Folder";
        const string uriFolder = "ms-appx:///Localizations/";
        string _fileName;
        string _uriName;

        bool fileExists = true;
        public bool IsDataLoaded { get; private set; } = false;

        
        private Dictionary<string, Dictionary<string, string>> _languageDictionaries;

        public LocalizationSource()
        {
            if (_languageDictionaries == null) _languageDictionaries = new Dictionary<string, Dictionary<string, string>>();
        }

        private async Task ensureDataLoaded()
        {
            if (_languageDictionaries.Count == 0) await getDataAsync();
            return;
        }

        private async Task getDataAsync()
        {
            if (_languageDictionaries.Count != 0)
                return;

            StorageFile fileLanguageResources = null;

            var jsonSerializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>));

            foreach (string language in App.SupportedLanguages)
            {
                _fileName = language + _fileSuffix;
                _uriName = uriFolder + _fileName;

                Dictionary<string, string> _languageDictionary;

                try
                {
                    fileLanguageResources = await ApplicationData.Current.LocalFolder.GetFileAsync(_fileName);
                }
                catch (FileNotFoundException)
                {
                    fileExists = false;
                }

                if (!fileExists)
                {
                    Uri dataUri = new Uri(_uriName);
                    fileLanguageResources = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                }

                try
                {
                    using (var stream = await fileLanguageResources.OpenStreamForReadAsync())
                    {
                        _languageDictionary = (Dictionary<string, string>)jsonSerializer.ReadObject(stream);
                        IsDataLoaded = true;
                    }
                }
                catch
                {
                    _languageDictionary = new Dictionary<string, string>();
                }

                _languageDictionaries.Add(language, _languageDictionary);
            }

        }

        public async Task<Dictionary<string, Dictionary<string, string>>> GetDictionaries()
        {

            await ensureDataLoaded();
            return _languageDictionaries;
        }

        public async Task LoadDictionaries()
        {
            await ensureDataLoaded();
        }
    }

}
