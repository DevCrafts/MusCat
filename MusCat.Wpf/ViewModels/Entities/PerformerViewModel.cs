﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using AutoMapper;
using MusCat.Application.Validators;
using MusCat.Core.Entities;
using MusCat.Core.Interfaces;
using MusCat.Infrastructure.Services;

namespace MusCat.ViewModels.Entities
{
    public class PerformerViewModel : ViewModelBase, IDataErrorInfo
    {
        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        private string _info;
        public string Info
        {
            get { return _info; }
            set
            {
                _info = value;
                RaisePropertyChanged();
            }
        }

        private Country _country;
        public Country Country
        {
            get { return _country; }
            set
            {
                _country = value;
                RaisePropertyChanged();
            }
        }

        private byte? _albumCollectionRate;
        public byte? AlbumCollectionRate
        {
            get { return _albumCollectionRate; }
            set
            {
                _albumCollectionRate = value;
                RaisePropertyChanged();
            }
        }
        
        private ObservableCollection<AlbumViewModel> _albums = new ObservableCollection<AlbumViewModel>();
        public ObservableCollection<AlbumViewModel> Albums
        {
            get { return _albums; }
            set
            {
                _albums = value;
                RaisePropertyChanged();
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                RaisePropertyChanged();
            }
        }

        public void UpdateAlbumCollectionRate(IRateCalculator rateCalculator)
        {
            if (rateCalculator == null)
            {
                return;
            }

            var rates = _albums.Select(a => a.Rate);
            AlbumCollectionRate = rateCalculator.Calculate(rates);
        }

        private readonly object _lock = new object();


        public PerformerViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(_albums, _lock);
        }

        public void LocateImagePath()
        {
            ImagePath = FileLocator.GetPerformerImagePath(Mapper.Map<Performer>(this));
        }


        #region IDataErrorInfo methods

        private readonly PerformerValidator _validator = new PerformerValidator();

        public string Error => this["Name"];

        public string this[string columnName]
        {
            get
            {
                var result = _validator.Validate(Mapper.Map<Performer>(this));

                if (!result.Errors.Any(e => e.PropertyName == columnName))
                {
                    return string.Empty;
                }

                var error = result.Errors
                                  .First(e => e.PropertyName == columnName)
                                  .ErrorMessage;
                return error;
            }
        }

        #endregion
    }
}
