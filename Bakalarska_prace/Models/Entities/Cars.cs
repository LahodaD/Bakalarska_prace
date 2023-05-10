﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bakalarska_prace.Models.Entities
{
    [Table(nameof(Cars))]
    public class Cars
    {
        [Key]
        public int Id { get; set; }

        public int CreateYear { get; set; }
        public double Price { get; set; }
        
        [Required]
        [StringLength(56)]
        public string VehicleBrand { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
    }
}
