using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MastersOfCinema.ViewModels
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        public int DirectorId { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image Name")]
        public string ImageName { get; set; }
        [NotMapped]
        [DisplayName("Upload Movie Poster")]
        [JsonIgnore]
        public IFormFile ImageFile { get; set; }
    }
}
