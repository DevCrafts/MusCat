﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using MusCat.Application.Dto;
using MusCat.Application.Interfaces;
using MusCat.Core.Interfaces.Data;
using MusCat.Core.Util;
using MusCat.Util;
using MusCat.ViewModels.Entities;

namespace MusCat.ViewModels
{
    class EditCountryViewModel : ViewModelBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICountryService _countryService;

        private ObservableCollection<CountryViewModel> _countrylist;
        public ObservableCollection<CountryViewModel> Countrylist
        {
            get { return _countrylist; }
            set
            {
                _countrylist = value;
                RaisePropertyChanged();
            }
        }

        private string _countryInput;
        public string CountryInput
        {
            get { return _countryInput; }
            set
            {
                _countryInput = value;
                RaisePropertyChanged();
            }
        }

        public int SelectedCountryIndex { get; set; }
        
        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand ReplaceCommand { get; private set; }
        public ICommand OkCommand { get; private set; }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set
            {
                _dialogResult = value;
                RaisePropertyChanged();
            }
        }


        public EditCountryViewModel(ICountryService countryService, IUnitOfWork unitOfWork)
        {
            Guard.AgainstNull(countryService);
            Guard.AgainstNull(unitOfWork);

            _countryService = countryService;
            _unitOfWork = unitOfWork;
            
            AddCommand = new RelayCommand(async () => await AddCountryAsync());
            RemoveCommand = new RelayCommand(async () => await RemoveCountryAsync());
            ReplaceCommand = new RelayCommand(async () => await UpdateCountryAsync());
            OkCommand = new RelayCommand(() => { DialogResult = true; });
        }

        public async Task LoadCountriesAsync()
        {
            Countrylist = new ObservableCollection<CountryViewModel>();

            var countryModels = (await _unitOfWork.CountryRepository.GetAllAsync()).OrderBy(c => c.Name).ToList();

            foreach (var countryModel in countryModels)
            {
                var country = Mapper.Map<CountryViewModel>(countryModel);
                country.PerformerCount = await _countryService.GetPerformersCountAsync(country.Id);
                Countrylist.Add(country);
            }
        }

        private async Task AddCountryAsync()
        {
            var result = await _countryService.AddCountryAsync(CountryInput);

            if (result.Type != ResultType.Ok)
            {
                MessageBox.Show(result.Error);
                return;
            }
            
            Countrylist.Add(Mapper.Map<CountryDto, CountryViewModel>(result.Data));
        }

        private async Task RemoveCountryAsync()
        {
            var selectedCountry = Countrylist[SelectedCountryIndex];
            var result = await _countryService.RemoveCountryAsync(selectedCountry.Id);

            if (result.Type != ResultType.Ok)
            {
                MessageBox.Show(result.Error);
                return;
            }

            Countrylist.RemoveAt(SelectedCountryIndex);
        }

        private async Task UpdateCountryAsync()
        {
            if (SelectedCountryIndex < 0)
            {
                MessageBox.Show("Choose the country first");
                return;
            }

            var selectedCountry = Countrylist[SelectedCountryIndex];
            var result = await _countryService.UpdateCountryAsync(selectedCountry.Id, CountryInput);

            if (result.Type != ResultType.Ok)
            {
                MessageBox.Show(result.Error);
                return;
            }

            CountryInput = "";

            var updatedCountry = Mapper.Map<CountryViewModel>(result.Data);
            updatedCountry.PerformerCount = selectedCountry.PerformerCount;

            Countrylist[SelectedCountryIndex] = updatedCountry;
        }
    }
}
