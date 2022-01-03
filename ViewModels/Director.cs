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
    public class Director
    {
        public Director()
        {
            Movies = new List<Movie>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }

        public List<Movie> Movies { get; set; }
        //public string PhotoURL { get; set; }
        //public ICollection<Movie> Movies { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image Name")]
        public string ImageName { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        [JsonIgnore]
        public IFormFile ImageFile { get; set; }
    }
}
