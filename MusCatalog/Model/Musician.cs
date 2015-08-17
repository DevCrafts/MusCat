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
    using System.Collections.ObjectModel;
    
    public partial class Musician
    {
        public Musician()
        {
            this.Lineups = new ObservableCollection<Lineup>();
        }
    
        public long ID { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public Nullable<short> YearBorn { get; set; }
        public Nullable<short> YearDied { get; set; }
        public Nullable<long> PerformerID { get; set; }

        public virtual ObservableCollection<Lineup> Lineups { get; set; }
    }
}