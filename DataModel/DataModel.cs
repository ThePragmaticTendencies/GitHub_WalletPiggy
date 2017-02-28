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

namespace CostDaily.Common
{
    public interface IStatistics
    {
        string GroupKey { get; set; }
        decimal GroupSum { get; set; }
        int GroupCount { get; set; }
    }

    public interface IResponsiveDimensions
    {
        int RespWidth { get; set; }
        int RespHeight { get; set; }
    }

    [DataContract]
    public class Currency
    {
        [DataMember]
        public string CurrencySymbol { get; set; }
        [DataMember]
        public double? ExchangeRate { get; set; }

        public Currency()
        {
            this.CurrencySymbol = App.AppCurrentRegionalInformation.NumberFormat.CurrencySymbol;
            this.ExchangeRate = 1.0d;
        }

        public Currency(string currencySymbol, double? exchangeRate)
        {
            this.CurrencySymbol = currencySymbol;
            this.ExchangeRate = exchangeRate;
        }
    }

    public interface ICurrencyController
    {
        Currency CurrencyInfo { get; set; }

        string GetCurrencySymbol();
        double? GetExchangedValue();
        bool IsForeignCurrency();
        bool IsExchangeRateProvided();
    }

    [DataContract]
    public class Cost : ICurrencyController
    {

        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public string Date { get; set; }
        [DataMember]
        public decimal Value { get; set; }
        [DataMember]
        public string CategoryImage { get; set; }
        [DataMember]
        public Currency CurrencyInfo { get; set; }

        public Cost(string categoryName, string date, decimal value, string categoryImage)
        {
            CategoryName = categoryName;
            Date = date;
            Value = value;
            CategoryImage = categoryImage;
            CurrencyInfo = new Currency();
        }

        public Cost(string categoryName, string date, decimal value, string categoryImage, string currencySymbol, double exchangeRate)
        {
            CategoryName = categoryName;
            Date = date;
            Value = value;
            CategoryImage = categoryImage;
            CurrencyInfo = new Currency(currencySymbol, exchangeRate);
        }

        public bool CompareCosts(Cost comparator)
        {
            string hashThisCost = this.CategoryImage + this.Date + this.Value;
            string hashComparatorCost = comparator.CategoryImage + comparator.Date + comparator.Value;
            return (hashThisCost == hashComparatorCost);

        }

        public string GetCurrencySymbol()
        {
            if (this.CurrencyInfo != null)
            {
                return this.CurrencyInfo.CurrencySymbol;
            }
            else
            {
                return "empty";
            }
        }

        public double? GetExchangedValue()
        {
            if (this.CurrencyInfo != null)
            {
                if (IsForeignCurrency() && IsExchangeRateProvided())
                {
                    return (Convert.ToDouble(this.Value) * this.CurrencyInfo.ExchangeRate);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public bool IsForeignCurrency()
        {
            if (this.CurrencyInfo != null)
            {
                return !this.CurrencyInfo.CurrencySymbol.Contains(App.AppCurrentRegionalInformation.NumberFormat.CurrencySymbol);
            }
            else
            {
                return false;
            }
        }

        public bool IsExchangeRateProvided()
        {
            if (this.CurrencyInfo != null)
            {
                return (this.CurrencyInfo.ExchangeRate != null);
            }
            else
            {
                return false;
            }
        }
    }

    public class CostGroupped : ObservableCollection<Cost>, IStatistics
    {
        public string GroupKey { get; set; }
        public decimal GroupSum { get; set; }
        public int GroupCount { get; set; }

        public CostGroupped(IEnumerable<Cost> items) : base(items)
        {
        }
    }

    public class Statistics : INotifyPropertyChanged, IStatistics
    {
        private decimal _groupSum;
        private int _chartWidth;

        public string GroupKey { get; set; }
        public decimal GroupSum
        {
            get { return _groupSum; }
            set { _groupSum = value; }
        }
        public int GroupCount { get; set; }
        public int ChartWidth
        {
            get { return _chartWidth; }
            set
            {
                _chartWidth = value;
                OnPropertyChanged("ChartWidth");
            }
        }
        public string GroupImgPath { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public Statistics()
        {
            ChartWidth = 0;
        }

        public Statistics(string name, string path)
        {
            GroupKey = name;
            GroupSum = 0.0m;
            GroupImgPath = path;
            GroupCount = 0;

        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CummulativeStatistics : IStatistics
    {
        private decimal _groupSum;
        private ObservableCollection<Statistics> _cummulatedData;

        public string GroupKey { get; set; }
        public decimal GroupSum
        {
            get { return _groupSum; }
            set { _groupSum = value; }
        }
        public int GroupCount { get; set; }

        public ObservableCollection<Statistics> CummulatedData
        {
            get { return _cummulatedData; }
            set { _cummulatedData = value; }
        }

        public CummulativeStatistics(IEnumerable<Statistics> source)
        {
            _cummulatedData = new ObservableCollection<Statistics>(source);
            _groupSum = 0m;

            foreach (Statistics data in source)
            {
                _groupSum += data.GroupSum;
            }

        }

    }
    public class MultiBindChartContainer
    {
        private Dictionary<string, Statistics> _categoryStatisticsData;

        public string CategoryName;
        public decimal MaxGroupSum;
        public Dictionary<string, Statistics> CategoryStatisticsData
        {
            get { return _categoryStatisticsData; }
            set { _categoryStatisticsData = value; }
        }

        public MultiBindChartContainer(string firstKey, Statistics first, string secondKey, Statistics second)
        {
            _categoryStatisticsData = new Dictionary<string, Statistics>();

            CategoryName = first.GroupKey;
            CategoryStatisticsData.Add(firstKey, first);
            if (second == null)
            {
                second = new Statistics(first.GroupKey, first.GroupImgPath);
            }

            CategoryStatisticsData.Add(secondKey, second);
            MaxGroupSum = ReturnMaxGroupSum(first, second);

        }

        public MultiBindChartContainer(string firstKey, Statistics first, string secondKey, bool exists)
        {
            _categoryStatisticsData = new Dictionary<string, Statistics>();

            CategoryName = first.GroupKey;
            CategoryStatisticsData.Add(firstKey, first);
            if (exists == false)
            {
                Statistics second = new Statistics(first.GroupKey, first.GroupImgPath);
                CategoryStatisticsData.Add(secondKey, second);
                MaxGroupSum = ReturnMaxGroupSum(first, second); //co jezeli true?
            }
        }

        private decimal ReturnMaxGroupSum(Statistics first, Statistics second)
        {
            if (first.GroupSum > second.GroupSum)
            {
                return first.GroupSum;
            }
            else
            {
                return second.GroupSum;
            }
        }
    }

    public class CategoryWithStats : INotifyPropertyChanged, IStatistics
    {
        private decimal _groupSum = 0.0m;
        private int _imageWidth { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ImagePath { get; set; }

        public string GroupKey { get; set; }
        public decimal GroupSum
        {
            get { return _groupSum; }
            set
            {
                _groupSum = value;
                OnPropertyChanged("GroupSum");
            }
        }
        public int GroupCount { get; set; }

        public int ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                _imageWidth = value;
                OnPropertyChanged("ImageWidth");
            }
        }

        public CategoryWithStats(string categoryName, string path, decimal sum, int width)
        {
            GroupSum = sum;
            GroupKey = categoryName;
            ImagePath = path;
            ImageWidth = width;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [DataContract]
    public class Category : INotifyPropertyChanged
    {
        [DataMember]
        private Int32 UniqueID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ImagePath { get; set; }
        [DataMember]
        public string Color { get; set; }

        private int _imageWidth { get; set; }
        public int ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                _imageWidth = value;
                OnPropertyChanged("ImageWidth");
            }
        }

        public Category()
        {
            UniqueID = 0;
            Name = null;
            ImagePath = null;
            Color = null;
            ImageWidth = 0;
        }

        public Category(Int32 uniqueID, string name, string imagePath, string color)
        {
            UniqueID = uniqueID;
            Name = name;
            ImagePath = imagePath;
            Color = color;
            ImageWidth = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
