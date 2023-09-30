
﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;



namespace Bulky.Models
{
    public class Category
    {

        [Key]
        public int Id { get; set; }

        [MaxLength(30)]
        [Required]
        [DisplayName("Category Name")]
        public  string  Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,100)]


        [Required]
  
        public int displayOrder { get; set; }
    }
}
