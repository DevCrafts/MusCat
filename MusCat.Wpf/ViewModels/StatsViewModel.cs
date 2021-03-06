﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using AutoMapper;
using LiveCharts;
using LiveCharts.Wpf;
using MusCat.Core.Interfaces.Stats;
using MusCat.Core.Util;
using MusCat.ViewModels.Entities;

namespace MusCat.ViewModels
{
    class StatsViewModel : ViewModelBase
    {
        private readonly IStatsService _stats;

        private int _performerCount;
        public int PerformerCount
        {
            get { return _performerCount; }
            set
            {
                _performerCount = value;
                RaisePropertyChanged();
            }
        }

        private int _albumCount;
        public int AlbumCount
        {
            get { return _albumCount; }
            set
            {
                _albumCount = value;
                RaisePropertyChanged();
            }
        }

        private int _songCount;
        public int SongCount
        {
            get { return _songCount; }
            set
            {
                _songCount = value;
                RaisePropertyChanged();
            }
        }

        public string _country;
        public string Country
        {
            get { return _country; }
            set
            {
                _country = value;
                RaisePropertyChanged();
            }
        }

        public const int TopPerformersCount = 12;

        private ObservableCollection<CanvasPerformerViewModel> _topPerformers;
        public ObservableCollection<CanvasPerformerViewModel> TopPerformers
        {
            get { return _topPerformers; }
            set
            {
                _topPerformers = value;
                RaisePropertyChanged();
            }
        }

        private SeriesCollection _decades;
        public SeriesCollection Decades
        {
            get { return _decades; }
            set
            {
                _decades = value;
                RaisePropertyChanged();
            }
        }

        private SeriesCollection _countries;
        public SeriesCollection Countries
        {
            get { return _countries; }
            set
            {
                _countries = value;
                RaisePropertyChanged();
            }
        }

        private string[] _labels;
        public string[] Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                RaisePropertyChanged();
            }
        }

        public Func<double, string> Formatter { get; set; }
        public ColorsCollection SeriesColor { get; set; } =
            new ColorsCollection
            {
                Color.FromRgb(195,225,125),
                Color.FromRgb(125,190,240)
            };

        public StatsViewModel(IStatsService stats)
        {
            Guard.AgainstNull(stats);
            _stats = stats;
        }

        public async Task LoadStatsAsync()
        {
            PerformerCount = await _stats.PerformerCountAsync();
            AlbumCount = await _stats.AlbumCountAsync();
            SongCount = await _stats.SongCountAsync();


            // bar chart "decades - album count - average album rate"

            var decades = await _stats.GetAlbumDecadesAsync();

            Labels = decades.Select(d => d.Decade).ToArray();

            Decades = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Album count",
                        Values = new ChartValues<int>(decades.Select(d => d.TotalCount)),
                        DataLabels = true,
                        Foreground = new SolidColorBrush(Colors.Gold)
                    },
                    new ColumnSeries
                    {
                        Title = "Max rated album count",
                        Values = new ChartValues<int>(decades.Select(d => d.MaxRatedCount)),
                        DataLabels = true,
                        Foreground = new SolidColorBrush(Colors.HotPink)
                    }
                };


            // pie chart "performer - countries"

            var countries = await _stats.GetPerformerCountriesAsync();

            Countries = new SeriesCollection();

            var rareCountriesCount = 0;

            foreach (var country in countries)
            {
                if (country.Value >= 10)
                {
                    Countries.Add(new PieSeries
                    {
                        Title = country.Key,
                        Values = new ChartValues<int> { country.Value },
                        DataLabels = true
                    });
                }
                else
                {
                    rareCountriesCount += country.Value;
                }
            }

            Countries.Add(new PieSeries
            {
                Title = "Others",
                Values = new ChartValues<int> { rareCountriesCount },
                DataLabels = true
            });


            // performers with top rated albums

            if (countries.Any())
            {
                var maxPerformerCount = countries.Max(c => c.Value);

                await UpdateTopPerformersAsync(countries.Where(c => c.Value == maxPerformerCount)
                                                        .Select(c => c.Key)
                                                        .First());
            }
        }

        public async Task UpdateTopPerformersAsync(string country)
        {
            if (country == "Others")
            {
                return;
            }

            var topPerformers = await _stats.GetTopPerformersAsync(TopPerformersCount, country);

            TopPerformers = new ObservableCollection<CanvasPerformerViewModel>();

            foreach (var performer in topPerformers)
            {
                AddPerformerToCanvas(Mapper.Map<PerformerViewModel>(performer));
            }
            
            Country = country;
        }

        #region place items on canvas

        public void AddPerformerToCanvas(PerformerViewModel performerViewModel)
        {
            var pos = TopPerformers.Count;

            var left = (pos % ItemsPerRow) * ItemWidth + _randomizer.Next(-OffsetRandomness / 2, OffsetRandomness / 2);
            var top =  (pos / ItemsPerRow) * ItemHeight + _randomizer.Next(OffsetRandomness);

            TopPerformers.Add(new CanvasPerformerViewModel
            {
                Performer = performerViewModel,
                Left = left,
                Top = top
            });
        }

        public class CanvasPerformerViewModel
        {
            public PerformerViewModel Performer { get; set; }
            public int Left { get; set; }
            public int Top { get; set; }
        }

        private static readonly Random _randomizer = new Random();

        private const int ItemsPerRow = 3;
        private const int OffsetRandomness = 60;
        private const int ItemWidth = 90;
        private const int ItemHeight = 120;

        #endregion
    }
}
