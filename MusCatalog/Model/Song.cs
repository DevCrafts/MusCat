//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MusCatalog.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    public partial class Song : INotifyPropertyChanged, IDataErrorInfo
    {
        private byte trackNo;
        private string name;
        private string timeLength;
        private byte? rate;
        
        public long AlbumID { get; set; }
        public long ID { get; set; }
        public byte TrackNo
        {
            get { return trackNo; }
            set
            {
                trackNo = value;
                RaisePropertyChanged("TrackNo");
            } 
        }
        public string Name
        {
            get { return name; }
            set
            { 
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        public string TimeLength
        {
            get { return timeLength; }
            set
            {
                timeLength = value;
                RaisePropertyChanged("TimeLength");
            }
        }
        public Nullable<byte> Rate
        {
            get { return rate; }
            set
            {
                rate = value;
                RaisePropertyChanged("Rate");
            }
        }
    
        public virtual Album Album { get; set; }

        #region INotifyPropertyChanged event and method

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IDataErrorInfo methods

        public string Error
        {
            get
            { 
                return this["Name"] + this["TimeLength"];
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "TimeLength":
                        Regex regex = new Regex(@"^\d+:\d{2}$");
                        if (!regex.IsMatch(TimeLength))
                        {
                            error = "Time length should be in the format mm:ss";
                        }
                        break;

                    case "Name":
                        if (Name.Length > 50)
                        {
                            error = "Song title should contain not more than 50 symbols";
                        }
                        else if (Name == "")
                        {
                            error = "Song title can't be empty";
                        }
                        break;
                }
                return error;
            }
        }

        #endregion
    }
}
