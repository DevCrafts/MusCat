namespace MusCat.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Country
    {
        public Country()
        {
            Performers = new HashSet<Performer>();
        }

        public byte ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        public virtual ICollection<Performer> Performers { get; set; }
    }
}