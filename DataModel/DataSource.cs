using System;
using CostDaily.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Windows.Storage;
using Windows.Data.Json;
using System.ComponentModel;
using System.Globalization;
using Windows.Storage.Pickers;
using Windows.Storage.AccessCache;

namespace CostDaily.DataModel
{
    //Need to rewrite this classes till it will not be so fat and stubborn with its flexibility..
    //In other words need to implement ViewModel for Main Page

    public class DataBaseContainer : INotifyPropertyChanged
    {

        private int _pivotHeaderWidth = 0;
        private bool _isCostsHeaderVisible = true;
        private bool _isCollectionFirstTimeCreated = true;

        private ObservableCollection<CostGroupped> _monthlyGroupedCosts;
        private IEnumerable<Statistics> _dailyCategoryCosts;
        private CummulativeStatistics _monthlyCategoryCosts;
        private CummulativeStatistics _previousMonthCategoryCosts;
        private ObservableCollection<CategoryWithStats> _categoriesWithCosts;
        private ObservableCollection<MultiBindChartContainer> _monthlyStatistics;
        private ObservableCollection<Cost> _costs;

        private bool _isDataLoaded = false;

        public ObservableCollection<Category> Categories {get; set;}
        public ObservableCollection<Cost> Costs
        {
            get { return _costs; }
            set
            {
                _costs = value;
                if (_costs != null)
                {
                    isCollectionFirstTimeCreated();
                }
            }
        }

        private void _costs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _isCostHeaderVisible();
        }

        private void isCollectionFirstTimeCreated()
        {
            if (_isCollectionFirstTimeCreated)
            {
                _isCostHeaderVisible();
                _costs.CollectionChanged += _costs_CollectionChanged;
                _isCollectionFirstTimeCreated = false;
            } 
        }
        public ObservableCollection<CostGroupped> MonthlyGroupedCosts
        {
            get { return _monthlyGroupedCosts; }
            set
            {
                _monthlyGroupedCosts = value;
                OnPropertyChanged("MonthlyGroupedCosts");
            }
        }
        public ObservableCollection<MultiBindChartContainer> MonthlyStatistics
        {
            get { return _monthlyStatistics; }
            set
            {
                _monthlyStatistics = value;
                OnPropertyChanged("MonthlyStatistics");
            }
        }
        public IEnumerable<Statistics> DailyCategoryCosts
        {
            get { return _dailyCategoryCosts; }
            set
            {
                _dailyCategoryCosts = value;
                OnPropertyChanged("DailyCategoryCosts");
            }
        }
        public CummulativeStatistics MonthlyCategoryCosts
        {
            get { return _monthlyCategoryCosts; }
            set
            {
                _monthlyCategoryCosts = value;
                OnPropertyChanged("MonthlyCategoryCosts");
            }
        }
        public CummulativeStatistics PreviousMonthCategoryCosts
        {
            get { return _previousMonthCategoryCosts; }
            set
            {
                _previousMonthCategoryCosts = value;
                OnPropertyChanged("PreviousMonthCategoryCosts");
            }
        }
        public ObservableCollection<CategoryWithStats> CategoriesWithCosts
        {
            get { return _categoriesWithCosts; }
            set
            {
                _categoriesWithCosts = value;
                OnPropertyChanged("CategoriesWithCosts");
            }
        }
        public bool IsDataLoaded
        {
            get { return _isDataLoaded; }
            set
            {
                _isDataLoaded = value;
                OnPropertyChanged("IsDataLoaded");
            }
        }

        public int PivotHeaderWidth
        {
            get { return _pivotHeaderWidth; }
            set
            {
                _pivotHeaderWidth = value;
                OnPropertyChanged("PivotHeaderWidth");
            }
        }
        public bool IsCostsHeaderVisible
        {
            get { return _isCostsHeaderVisible; }
            set
            {
                _isCostsHeaderVisible = value;
                OnPropertyChanged("IsCostsHeaderVisible");
            }
        }
        private void _isCostHeaderVisible()
        {
            IsCostsHeaderVisible = (Costs.Count == 0) ? true : false;
        }
        public void UpdateChartElementsWidthForSummary(int displaySize)
        {
            foreach (Statistics grouppedElement in _monthlyCategoryCosts.CummulatedData)
            {
                double cropFactor = 0.50;
                int maxWidth = (int)Math.Floor(displaySize * cropFactor);
                double scaleFactor = (maxWidth / Convert.ToDouble(_monthlyCategoryCosts.CummulatedData.First().GroupSum));
                grouppedElement.ChartWidth = (int)Math.Floor(Convert.ToDouble(grouppedElement.GroupSum) * scaleFactor);
            }

        }
        public void UpdateChartWidthForStatistics(int displaySize, double crop, bool turnOnOneScale)
        {
            decimal maxSum = 0.0m;
            int maxWidth = (int)Math.Floor(displaySize * crop);
            double scaleFactor=1.0f;

            if (turnOnOneScale == true)
            {
                foreach (MultiBindChartContainer categoryStats in MonthlyStatistics)
                {
                    if (categoryStats.MaxGroupSum > maxSum) maxSum = categoryStats.MaxGroupSum;
                }
                scaleFactor = (maxWidth / Convert.ToDouble(maxSum));
            }

            foreach (MultiBindChartContainer categoryStats in MonthlyStatistics)
            {
                if (turnOnOneScale == false) scaleFactor = (maxWidth / Convert.ToDouble(categoryStats.MaxGroupSum));

                    foreach (Statistics individualStats in categoryStats.CategoryStatisticsData.Values)
                    {
                        int length = (int)Math.Floor(Convert.ToDouble(individualStats.GroupSum) * scaleFactor);
                        if (length <4)
                        {
                            individualStats.ChartWidth = 4;
                        }
                        else
                        {
                            individualStats.ChartWidth = length;
                        }
                    }
            }

        }

        public void UpdateCategoryImagesWidth(int displaySize)
        {
            if (Categories == null)
                return;

            double cropFactor = 0.80;
            double columnsNumber = 4.0;
            if (displaySize<401)
            {
                columnsNumber = 3.0;
            }
            int maxWidth = (int)Math.Floor(displaySize * cropFactor);
            int imageSize = (int)Math.Floor(maxWidth/columnsNumber);

            foreach (Category CategoryElement in Categories)
            {
                CategoryElement.ImageWidth = imageSize;
            }
            //mieszam BARDZO
            if (CategoriesWithCosts != null)
            {
                foreach (CategoryWithStats CategoryElement in CategoriesWithCosts)
                {
                    CategoryElement.ImageWidth = imageSize;
                }
            }

        }
        public void UpdatePivoteHeaderWidth(int displaySize, int itemCount)
        {
            double cropFactor = 1;
            int maxWidth = (int)Math.Floor(displaySize * cropFactor);
            int headerWidth =50;

            if (itemCount != 0) headerWidth = (int)Math.Floor(maxWidth / Convert.ToDouble(itemCount));

            PivotHeaderWidth = headerWidth;
        }

        public DataBaseContainer()
        {
            Categories = null;
            Costs= null;
            IsDataLoaded = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class DataSource
    {
        const string fileName = "simpleDB.json";
        const string folderName = "CostsDaily_Folder";
        const string uriName = "ms-appx:///DataModel/simpleDB.json";

        const string costDB = "costDB.json";
        const string costDB_URI = "ms-appx:///DataModel/costDB.json";

        const string categoryDB = "categoryDB.json";
        const string categoryDB_URI = "ms-appx:///DataModel/categoryDB.json";

        const string backupFolderName = "WalletPiggy_BackUps";
        const string backupLoadFolderName = "LoadFromHere";
        bool backupLoaded = false;
        bool cannotCompleteBackup = false;

        private string _today = DateTime.Now.ToString("yyyy-MM-dd");
        private string _thisMonth = DateTime.Now.ToString("yyyy-MM");
        private string _lastMonth = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");

        bool fileExists = true;
        bool _isCategoryDataLoaded = false;
        bool _isCostDataLoaded = false;
        bool _isThisMonthDataLoaded = false;
        bool _isPreviousMonthDataLoaded = false;
        bool _isDailyDataLoaded = false;
        private bool IsDailyDataLoaded
        {
            set
            {
                _isDailyDataLoaded = value;
                if (value) _loadCategoriesWithCosts();
            }
        }

        public bool IsDataLoaded { get; private set; } = false;

        private void isDataLoaded()
        {
            if (IsCategoryDataLoaded && IsCostDataLoaded)
            {
                IsDataLoaded = true;
                _dbContainer.Costs.CollectionChanged += Costs_CollectionChanged;
                RefreshComputedData();
            }
        }
        public bool IsCategoryDataLoaded
        {
            get
            {
                return _isCategoryDataLoaded;
            }

            private set
            {
                _isCategoryDataLoaded = value;
                isDataLoaded();
            }

        }
        public bool IsCostDataLoaded
        {
            get
            {
                return _isCostDataLoaded;
            }

            private set
            {
                _isCostDataLoaded = value;
                isDataLoaded();
            }

        }

        private void _isMonthlyDataLoaded()
        {
            if (IsThisMonthDataLoaded && IsPreviousMonthDataLoaded)
            {
                IsDataLoaded = true;
                _populateStatisticsChartData();
            }
        }
        public bool IsThisMonthDataLoaded
        {
            get
            {
                return _isThisMonthDataLoaded;
            }

            private set
            {
                _isThisMonthDataLoaded = value;
                _isMonthlyDataLoaded();
            }

        }
        public bool IsPreviousMonthDataLoaded
        {
            get
            {
                return _isPreviousMonthDataLoaded;
            }

            private set
            {
                _isPreviousMonthDataLoaded = value;
                _isMonthlyDataLoaded();
            }

        }

        private void _resetStatisticsBoolians()
        {
            bool _isThisMonthDataLoaded = false;
            bool _isPreviousMonthDataLoaded = false;
        }

        private DataBaseContainer _dbContainer = new DataBaseContainer();

        public DataSource()
        {
            _dbContainer = new DataBaseContainer();
        }

        private void Costs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshComputedData();
        }

        public event EventHandler DataRefreshed;
        protected virtual void OnDataRefreshed(EventArgs e)
        {
            EventHandler handler = DataRefreshed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler DataLoaded;
        protected virtual void OnDataLoaded(EventArgs e)
        {
            EventHandler handler = DataLoaded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public async Task<DataBaseContainer> GetDataBase()
        {
            await ensureDataLoaded();
            return _dbContainer;
        }
        public async Task LoadData()
        {
            await ensureDataLoaded();
        }

        private async Task ensureDataLoaded()
        {
            if (_dbContainer.Categories == null || _dbContainer.Costs == null)
                await getAllDataAsync();
            return;
        }
        private async Task getAllDataAsync()
        {
            await _loadCostDataAsync();
            await _loadCategoriesDataAsync();
        }

        private async Task<StorageFolder> _accessSD()
        {
            StorageFolder externalDevices = Windows.Storage.KnownFolders.RemovableDevices;
            StorageFolder sdCard = (await externalDevices.GetFoldersAsync()).FirstOrDefault();

            return sdCard;
        }
        private async Task _loadCategoriesDataAsync()
        {
            if (_dbContainer.Categories != null)
            {
                return;
            }

            StorageFile fileDataBase = null;

            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Category>));

            try
            {
                fileDataBase = await ApplicationData.Current.LocalFolder.GetFileAsync(categoryDB);
            }
            catch (FileNotFoundException)
            {
                fileExists = false;
            }

            if (!fileExists)
            {
                Uri dataUri = new Uri(categoryDB_URI);
                fileDataBase = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            }

            try
            {
                using (var stream = await fileDataBase.OpenStreamForReadAsync())
                {
                    _dbContainer.Categories = (ObservableCollection<Category>)jsonSerializer.ReadObject(stream);
                }
            }
            catch
            {
                _dbContainer.Categories = new ObservableCollection<Category>();
            }

            IsCategoryDataLoaded = true;
        }
        private async Task _loadCostDataAsync()
        {
            if (_dbContainer.Costs != null)
            {
                return;
            }

            StorageFile fileDataBase = null;

            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Cost>));

            try
            {
                fileDataBase = await ApplicationData.Current.LocalFolder.GetFileAsync(costDB);
            }
            catch (FileNotFoundException)
            {
                fileExists = false;
            }

            if (!fileExists)
            {
                Uri dataUri = new Uri(costDB_URI);
                fileDataBase = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            }

            try
            {
                using (var stream = await fileDataBase.OpenStreamForReadAsync())
                {
                    _dbContainer.Costs = (ObservableCollection<Cost>)jsonSerializer.ReadObject(stream);
                }
            }
            catch
            {
                _dbContainer.Costs = new ObservableCollection<Cost>();
            }

            IsCostDataLoaded = true;
        }
        private async Task _saveCostDataAsync()
        {
            var jasonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Cost>));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(costDB, CreationCollisionOption.ReplaceExisting))
            {
                jasonSerializer.WriteObject(stream, App.DataModel._dbContainer.Costs);
            }

            await _doBackUp();
        }
        public async Task _doBackUp()
                {
                    StorageFolder sdCard = await _accessSD();

                    if (sdCard != null)
                    {
                        var jasonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Cost>));

                        StorageFolder backupFolder = await sdCard.CreateFolderAsync(backupFolderName, CreationCollisionOption.OpenIfExists);

                        await backupFolder.CreateFolderAsync(backupLoadFolderName, CreationCollisionOption.OpenIfExists);

                        StorageFile backupJSON = await backupFolder.CreateFileAsync(costDB, CreationCollisionOption.ReplaceExisting);

                        using (var stream = await backupJSON.OpenStreamForWriteAsync())
                        {
                            jasonSerializer.WriteObject(stream, App.DataModel._dbContainer.Costs);
                        }
                    }
                }
        private async Task<NotificationHelperExtended> _readBackUp()
        {
            StorageFolder sdCard = await _accessSD();
            IReadOnlyList<StorageFile> backupsList;
            ObservableCollection<Cost> backedDB = new ObservableCollection<Cost>();

            if (sdCard == null)
            {
                _dbContainer.Costs = backedDB;
                IsCostDataLoaded = true;
                return new NotificationHelperExtended("NoSdCard");
            }
            else
            {
                StorageFile backupDB = null;

                StorageFolder backupFolder;

                backupFolder = await sdCard.GetFolderAsync(backupFolderName);
                try
                {
                    backupFolder = await backupFolder.GetFolderAsync(backupLoadFolderName);
                }
                catch
                {
                    backupFolder = null;
                }

                if (backupFolder == null)
                {
                    _dbContainer.Costs = backedDB;
                    IsCostDataLoaded = true;
                    return new NotificationHelperExtended("NoAccess");
                }
                else
                {
                    try
                    {
                        backupsList = await backupFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery, 0, 3);
                    }
                    catch
                    {
                        backupsList = new List<StorageFile>();
                    }

                    if (backupsList.Count == 0)
                    {
                        _dbContainer.Costs = backedDB;
                        IsCostDataLoaded = true;
                        return new NotificationHelperExtended("LoadFileLack");
                    }
                    else
                    {
                        int i = 0;
                        var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Cost>));

                        while (backupLoaded == false || cannotCompleteBackup != false)
                        {
                            if (i >= backupsList.Count) cannotCompleteBackup = true;

                            while (i < backupsList.Count)
                            {
                                backupDB = await backupFolder.GetFileAsync(backupsList[i].Name);

                                try
                                {
                                    using (var stream = await backupDB.OpenStreamForReadAsync())
                                    {
                                        backedDB = (ObservableCollection<Cost>)jsonSerializer.ReadObject(stream);
                                        backupLoaded = true;
                                        i = backupsList.Count;
                                        IsCostDataLoaded = true;
                                    }
                                }
                                catch
                                {
                                    i++;
                                }
                            }

                        }

                        if (backupLoaded == false && cannotCompleteBackup == true)
                        {
                            _dbContainer.Costs = backedDB;
                            IsCostDataLoaded = true;
                            return new NotificationHelperExtended("BackUpFailed");
                        }
                        else
                        {
                            _dbContainer.Costs = backedDB;
                            return new NotificationHelperExtended("BackUpLoaded", true);
                        }

                    }
                }
            }
        }

        public async Task<NotificationHelperExtended> LoadBackUp()
        {
            NotificationHelperExtended notification = await _readBackUp();
            await ensureDataLoaded();

            return notification;
        }

        public async Task<StorageFile> GetFileJSON()
        {
            string datePrefix = DateTime.Now.ToString("yyMMdd");
            string fileName = datePrefix + "_Costs.json";
             
            StorageFolder appFolder = ApplicationData.Current.TemporaryFolder;
            StorageFile fileJSON = await appFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            var jasonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Cost>));

            using (var stream = await fileJSON.OpenStreamForWriteAsync())
            {
                jasonSerializer.WriteObject(stream, App.DataModel._dbContainer.Costs);
            }

            return fileJSON;
        }
        public async Task<StorageFile> GetFileCSV()
        {
            string datePrefix = DateTime.Now.ToString("yyMMdd");
            string fileName = datePrefix + "_Costs.csv";

            StorageFolder appFolder = ApplicationData.Current.TemporaryFolder;

            StorageFile fileCSV = await appFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (var stream = await fileCSV.OpenStreamForWriteAsync())
            {
                StreamWriter streamWriter = new StreamWriter(stream);
                List<Cost> newList = _dbContainer.Costs.ToList();

                string stringCSV = Services.Hacks.CostToCSV(newList);

                await streamWriter.WriteAsync(stringCSV);
            }

            return fileCSV;
        }

        public static IEnumerable<Cost> _loadIntervalData(string period, IEnumerable<Cost> source)
        {
            var intervalData =
                 from item in source
                 where item.Date.Contains(period)
                 select item;
            return intervalData;
        }
        public static IEnumerable<IStatistics> _loadIntervalGroupedCategoryStatistics(string period, IEnumerable<Cost> source)
        {
            IEnumerable<IStatistics> intervalGroupedData =
            (from item in source
             where item.Date.Contains(period)
             group item by item.CategoryName into categoryGroup
             select new Statistics()
             {
                 GroupKey = categoryGroup.Key,
                 GroupSum = categoryGroup.Sum(c => c.Value),
                 GroupCount = categoryGroup.Count(),
                 GroupImgPath = categoryGroup.ElementAt(0).CategoryImage
             }
             ).OrderByDescending(p => p.GroupSum);

            return intervalGroupedData;
        }

        public void _loadGroupedDateData()
        {
            IEnumerable<CostGroupped> tempContainer =
            (from item in _dbContainer.Costs
             group item by Convert.ToDateTime(item.Date).ToString("yyyy-MM-dd", App.AppCurrentRegionalInformation)
             into categoryGroup//Shorten please..

             select new CostGroupped(categoryGroup)
             {
                 GroupKey = categoryGroup.Key,
                 GroupSum = categoryGroup.Sum(c => c.Value),
             }
             ).OrderByDescending(p => p.GroupKey);

            if (_dbContainer.MonthlyGroupedCosts != null) _dbContainer.MonthlyGroupedCosts.Clear();

            _dbContainer.MonthlyGroupedCosts = new ObservableCollection<CostGroupped>(tempContainer);
        }
        public void RefreshComputedData()
        {
            App.DataModel._populateStatistics();
            App.DataModel._loadGroupedDateData();         
        }
        public void _populateStatistics()
        {
            _populateThisMonthStatistics();
            _populatePreviousMonthStatistics();
        }

        public void _populateThisMonthStatistics()
        {
            if (_isThisMonthDataLoaded) _isThisMonthDataLoaded = false;

            IEnumerable<Cost> thisMonthResults = _loadIntervalData(this._thisMonth, _dbContainer.Costs);
            IEnumerable<Statistics> thisMonthGrouppedResults = _loadIntervalGroupedCategoryStatistics(this._thisMonth, thisMonthResults) as IEnumerable<Statistics>;
            _dbContainer.MonthlyCategoryCosts = new CummulativeStatistics(thisMonthGrouppedResults);
            IsThisMonthDataLoaded = true;

            _populateDailyStatistics(thisMonthResults);
        }
        public void _populatePreviousMonthStatistics()
        {
            if (_isPreviousMonthDataLoaded) _isPreviousMonthDataLoaded = false;
            IEnumerable<Cost> previousMonthResults = _loadIntervalData(this._lastMonth, _dbContainer.Costs);
            IEnumerable<Statistics> previousMonthGrouppedResults = _loadIntervalGroupedCategoryStatistics(this._lastMonth, previousMonthResults) as IEnumerable<Statistics>;
            _dbContainer.PreviousMonthCategoryCosts = new CummulativeStatistics(previousMonthGrouppedResults);
            IsPreviousMonthDataLoaded = true;
        }
        public void _populateDailyStatistics(IEnumerable<Cost> period)
        {
            if (_isDailyDataLoaded) _isDailyDataLoaded = false;
            IEnumerable<Statistics> dailyGrouppedResults = _loadIntervalGroupedCategoryStatistics(this._today, period) as IEnumerable<Statistics>;
            _dbContainer.DailyCategoryCosts = dailyGrouppedResults;
            IsDailyDataLoaded = true;
        }

        public ObservableCollection<CategoryWithStats> _getCategoriesWithStats()
        {
            ObservableCollection<CategoryWithStats> categoriesWithCosts = new ObservableCollection<CategoryWithStats>();

            foreach (Category category in _dbContainer.Categories)
            {
                decimal groupSum;

                Statistics statsForCategory = _dbContainer.DailyCategoryCosts.FirstOrDefault(p => p.GroupKey.Contains(category.Name));

                if (statsForCategory == null)
                {
                    groupSum = 0.0m;
                }
                else
                {
                    groupSum = statsForCategory.GroupSum;
                }


                categoriesWithCosts.Add(new CategoryWithStats(
                    category.Name,
                    category.ImagePath,
                    groupSum,
                    category.ImageWidth)
                    );
            }

            return categoriesWithCosts;
        }
        public void _loadCategoriesWithCosts()
        {
            if (_dbContainer.CategoriesWithCosts == null)
            {
                _dbContainer.CategoriesWithCosts = _getCategoriesWithStats();
            }
            else
            {
                _updateCategoriesWithCosts();
            }
        }
        public void _updateCategoriesWithCosts()
        {
            ObservableCollection<CategoryWithStats> categoriesWithCosts = _getCategoriesWithStats();

            foreach (CategoryWithStats category in categoriesWithCosts)
            {
                CategoryWithStats existingCategory = _dbContainer.CategoriesWithCosts.FirstOrDefault(p => p.GroupKey.Contains(category.GroupKey));

                if (existingCategory != null)
                {
                    existingCategory.GroupSum = category.GroupSum;
                }
                else
                {
                    _dbContainer.CategoriesWithCosts.Add(category);
                }
            }
        }

        public void _populateStatisticsChartData()
        {
            ObservableCollection<MultiBindChartContainer> cummulativeStatistics = new ObservableCollection<MultiBindChartContainer>();
            
            foreach (Statistics thisMonthStat in _dbContainer.MonthlyCategoryCosts.CummulatedData)
            {
                Statistics lastMonthStat = _dbContainer.PreviousMonthCategoryCosts.CummulatedData.FirstOrDefault(p => p.GroupKey.Contains(thisMonthStat.GroupKey));
                cummulativeStatistics.Add(new MultiBindChartContainer("ThisMonth", thisMonthStat, "LastMonth", lastMonthStat));
            }

            if (_dbContainer.MonthlyStatistics != null) _dbContainer.MonthlyStatistics.Clear();
            _dbContainer.MonthlyStatistics = (cummulativeStatistics);
            OnDataLoaded(null);
        }

        public void UpdateResponsiveChartParametersForSummary(int displaySize, double crop)
        {
            _dbContainer.UpdateChartWidthForStatistics(displaySize, crop, true); //what about Grid not found?
        }
        public void UpdateResponsiveCategoryImageSize(int displaySize)
        {
            _dbContainer.UpdateCategoryImagesWidth(displaySize);
        }
        public void UpdateResponsivePivotHeaderWidth(int displaySize, int itemCount)
        {
            _dbContainer.UpdatePivoteHeaderWidth(displaySize, itemCount);
        }

        public async void SavaDataBase()
        {
            await _saveCostDataAsync();
            OnDataRefreshed(null);
        }
        public void AddCost(Cost costObject)
        {
            _dbContainer.Costs.Add(costObject);
            //IncreaseCategoriesWithCosts(costObject);
        }
        public void RemoveCost(Cost costObject)
        {
            foreach (Cost element in _dbContainer.Costs.ToList())
            {
                if (element.CompareCosts(costObject))
                {
                    _dbContainer.Costs.Remove(element);
                    //DecreaseCategoriesWithCosts(costObject);
                }
            }

            SavaDataBase();
        }

        public void CheckTimeChanged()
        {
            DateTime now = DateTime.Now.Date;
            DateTime before = Convert.ToDateTime(_today);
            //DateTime test = Convert.ToDateTime("2017-02-20");
            if (now != before)
            {
                _updateStatisticsDates();
                _populateStatistics();
            }
        }
        private void _updateStatisticsDates()
        {
            //DateTime test = Convert.ToDateTime("2017-02-20");

            //_today = test.ToString("yyyy-MM-dd");
            //_thisMonth = test.ToString("yyyy-MM");
            //_lastMonth = test.AddMonths(-1).ToString("yyyy-MM");

            _today = DateTime.Now.ToString("yyyy-MM-dd");
            _thisMonth = DateTime.Now.ToString("yyyy-MM");
            _lastMonth = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
        }
    }
}

