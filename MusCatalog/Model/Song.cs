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
    
    public partial class Song
    {
        public long PerformerID { get; set; }
        public long AlbumID { get; set; }
        public long ID { get; set; }
        public byte TrackNo { get; set; }
        public string Name { get; set; }
        public string TimeLength { get; set; }
        public string Info { get; set; }
        public Nullable<byte> Rate { get; set; }
    
        public virtual Album Album { get; set; }
    }
}
