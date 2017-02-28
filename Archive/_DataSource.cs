using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostDaily.Archive
{
    class _DataSource
    {
        private async Task getDataAsync()
        {
            if (_dbContainer.Categories != null || _dbContainer.Costs != null)
            {
                OnDataLoaded(null);
                return;
            }

            StorageFile fileDataBase = null;

            var jsonSerializer = new DataContractJsonSerializer(typeof(DataBaseContainer));

            try
            {
                fileDataBase = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            }
            catch (FileNotFoundException)
            {
                fileExists = false;
            }

            if (!fileExists)
            {
                Uri dataUri = new Uri(uriName);
                fileDataBase = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            }

            try
            {
                using (var stream = await fileDataBase.OpenStreamForReadAsync())
                {
                    _dbContainer = (DataBaseContainer)jsonSerializer.ReadObject(stream);
                    IsDataLoaded = true;
                }
            }
            catch
            {
                _dbContainer = new DataBaseContainer();
            }

            OnDataLoaded(null);

        }
        private async Task saveDataAsync()
        {
            //Acces to SD card granting: Esternal Devices, Folder resp for SD
            StorageFolder externalDevices = Windows.Storage.KnownFolders.RemovableDevices;
            StorageFolder sdCard = (await externalDevices.GetFoldersAsync()).FirstOrDefault();

            var jasonSerializer = new DataContractJsonSerializer(typeof(DataBaseContainer));

            if (sdCard != null)
            {
                StorageFolder appFolder = await sdCard.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
                StorageFile exportFile = await appFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using (var stream = await exportFile.OpenStreamForWriteAsync())
                {
                    jasonSerializer.WriteObject(stream, App.DataModel._dbContainer);
                }
            }

            //Problem with individual Class serialization. Maybe Typeof should be different, aiming serializable class

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting))
            {
                jasonSerializer.WriteObject(stream, App.DataModel._dbContainer);
            }
        }

        public async Task<ObservableCollection<Category>> GetCategories()
        {
            //Omit data loading
            await ensureDataLoaded();
            return _dbContainer.Categories; //DataBase.Categories;
        }
        public async Task<ObservableCollection<Cost>> GetCosts()
        {
            //Omit data loading
            await ensureDataLoaded();
            return _dbContainer.Costs; //DataBase.Costs;
        }

        public void UpdateCategoriesWithCosts(Cost manipulatedCost)
        {

            if (manipulatedCost.Date.Contains(this._today))
            {
                var tempDayContainer =
                (from item in _dbContainer.Costs
                 where item.Date.Contains(this._today)
                 where item.CategoryName.Contains(manipulatedCost.CategoryName)
                 group item by item.CategoryName into categoryGroup
                 select new Statistics()
                 {
                     GroupKey = categoryGroup.Key,
                     GroupSum = categoryGroup.Sum(c => c.Value),
                     GroupCount = categoryGroup.Count()
                 }).OrderByDescending(p => p.GroupSum);

                ObservableCollection<Statistics> dayCategoryCosts = new ObservableCollection<Statistics>(tempDayContainer);

                if (dayCategoryCosts.Count != 0)
                {
                    _dbContainer.CategoriesWithCosts.First(p => p.GroupKey.Contains(manipulatedCost.CategoryName)).GroupSum = dayCategoryCosts[0].GroupSum;
                }
                else
                {
                    _dbContainer.CategoriesWithCosts.First(p => p.GroupKey.Contains(manipulatedCost.CategoryName)).GroupSum = 0.0m;
                }
            }
        }
        public void IncreaseMonthlyCategoryCosts(Cost manipulatedCost)
        {

            if (manipulatedCost.Date.Contains(this._thisMonth))
            {
                _dbContainer.MonthlyCategoryCosts.CummulatedData.First(p => p.GroupKey.Contains(manipulatedCost.CategoryName)).GroupSum += manipulatedCost.Value;
            }
            else if (manipulatedCost.Date.Contains(this._lastMonth))
            {
                _dbContainer.PreviousMonthCategoryCosts.CummulatedData.First(p => p.GroupKey.Contains(manipulatedCost.CategoryName)).GroupSum += manipulatedCost.Value;
            }

        }
        public void DecreaseMonthlyCategoryCosts(Cost manipulatedCost)
        {

            if (manipulatedCost.Date.Contains(this._today))
            {
                _dbContainer.MonthlyCategoryCosts.CummulatedData.First(p => p.GroupKey.Contains(manipulatedCost.CategoryName)).GroupSum -= manipulatedCost.Value;
            }
            else if (manipulatedCost.Date.Contains(this._lastMonth))
            {
                _dbContainer.PreviousMonthCategoryCosts.CummulatedData.First(p => p.GroupKey.Contains(manipulatedCost.CategoryName)).GroupSum -= manipulatedCost.Value;
            }
        }

        public void _populateStatistics()
        {
            IEnumerable<Cost> thisMonthResults = _loadIntervalData(this._thisMonth, _dbContainer.Costs); ;
            IEnumerable<Cost> previousMonthResults = _loadIntervalData(this._lastMonth, _dbContainer.Costs);

            IEnumerable<Statistics> thisMonthGrouppedResults = _loadIntervalGroupedCategoryStatistics(this._thisMonth, thisMonthResults) as IEnumerable<Statistics>;
            IEnumerable<Statistics> previousMonthGrouppedResults = _loadIntervalGroupedCategoryStatistics(this._lastMonth, previousMonthResults) as IEnumerable<Statistics>;

            if (_dbContainer.DailyCategoryCosts == null || refreshDaily)
            {
                IEnumerable<Statistics> dailyGrouppedResults;
                dailyGrouppedResults = _loadIntervalGroupedCategoryStatistics(this._today, thisMonthResults) as IEnumerable<Statistics>;
                _dbContainer.DailyCategoryCosts = dailyGrouppedResults;
                IsDailyDataLoaded = true;
            }

            _dbContainer.MonthlyCategoryCosts = new CummulativeStatistics(thisMonthGrouppedResults);
            IsThisMonthDataLoaded = true;
            _dbContainer.PreviousMonthCategoryCosts = new CummulativeStatistics(previousMonthGrouppedResults);
            IsPreviousMonthDataLoaded = true;
        }

        public void IncreaseCategoriesWithCosts(Cost manipulatedCost)
        {
            if (manipulatedCost.Date.Contains(this._today))
            {
                _dbContainer.CategoriesWithCosts.First(p => p.GroupKey.Contains(manipulatedCost.CategoryName)).GroupSum += manipulatedCost.Value;
            }
        }
        public void DecreaseCategoriesWithCosts(Cost manipulatedCost)
        {

            if (manipulatedCost.Date.Contains(this._today))
            {
                _dbContainer.CategoriesWithCosts.First(p => p.GroupKey.Contains(manipulatedCost.CategoryName)).GroupSum -= manipulatedCost.Value;
            }

        }

        //Part responsible for additional DB loading
        //jsonText = await FileIO.ReadTextAsync(fileDataBase);
        //JsonObject jsonObject = JsonObject.Parse(jsonText);
        //JsonArray jsonCostsArray = jsonObject["Costs"].GetArray();

        //foreach (JsonValue costValue in jsonCostsArray)
        //{
        //    JsonObject costObject = costValue.GetObject();
        //    Cost costTemp = new Cost(Int32.Parse(costObject["CategoryID"].GetString()),
        //                             costObject["Date"].GetString(),
        //                             float.Parse(costObject["Value"].GetString()));
        //    _costs.Add(costTemp);
        //}

        //JsonArray jsonCategoriesArray = jsonObject["Categories"].GetArray();

        //    foreach (JsonValue categoryValue in jsonCategoriesArray)
        //    {
        //        JsonObject categoryObject = categoryValue.GetObject();
        //        Category categoryTemp = new Category(Int32.Parse(categoryObject["UniqueID"].GetString()),
        //                                             categoryObject["Name"].GetString(),
        //                                             categoryObject["ImagePath"].GetString(),
        //                                             categoryObject["Color"].GetString());
        //        _categories.Add(categoryTemp);
        //    }
    }
}
