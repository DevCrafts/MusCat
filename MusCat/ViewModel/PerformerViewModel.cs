﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MusCat.Model;
using MusCat.Utils;

namespace MusCat.ViewModel
{
    public class PerformerViewModel : INotifyPropertyChanged
    {
        public Performer Performer { get; set; }

        private byte? _albumCollectionRate;
        public byte? AlbumCollectionRate
        {
            get { return _albumCollectionRate; }
            set
            {
                _albumCollectionRate = value;
                RaisePropertyChanged("AlbumCollectionRate");
            }
        }

        private int _albumCount;
        public int AlbumCount
        {
            get { return _albumCount; }
            set
            {
                _albumCount = value;
                RaisePropertyChanged("AlbumCount");
            }
        }

        public AlbumViewModel SelectedAlbum { get; set; }

        private ObservableCollection<AlbumViewModel> _albums = new ObservableCollection<AlbumViewModel>();
        public ObservableCollection<AlbumViewModel> Albums
        {
            get { return _albums; }
            set
            {
                _albums = value;
                RaisePropertyChanged("Albums");
            }
        }

        private readonly RateCalculator _rateCalculator = new RateCalculator();

        public void UpdateAlbumCollectionRate()
        {
            var albums = _albums.Select(a => a.Album);

            AlbumCollectionRate = _rateCalculator.Calculate(albums);
        }
        
        #region INotifyPropertyChanged event and method

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}