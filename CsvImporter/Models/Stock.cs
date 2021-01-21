using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CsvImporter.Models
{
    /// <summary>
    /// Class to represent a Stock into Store
    /// </summary>
    [Table("Stock")]
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string PointOfSale { get; set; }

        [Required]
        [MaxLength(80)]
        public string Product { get; set; }
                
        public DateTime Date { get; set; }
                
        public int AvailableQuantity { get; set; }
    }
}