using System.ComponentModel.DataAnnotations;

namespace App.MVC.Models.Forms
{
    public class RegisterBookForm
    {
        [Required(ErrorMessage = "Title cannot be empty.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author name cannot be empty.")]
        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Genre cannot be empty.")]
        [EnumDataType(typeof(Genre), ErrorMessage = "Genre not valid.")] 
        public Genre Genre { get; set; }

        [Required(ErrorMessage = "Description cannot be empty.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 4000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "ISBN cannot be empty.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Publisher name cannot be empty.")]
        [StringLength(100, ErrorMessage = "Publisher name cannot exceed 100 characters.")]
        public string Publisher { get; set; }

        //[Required(ErrorMessage = "File cannot be empty")]
        //[FileExtensions(Extensions = "jpg,jpeg,png", ErrorMessage = "Only image files (JPG, PNG, JPEG)")]
        //[DataType(DataType.Upload)]
        public IFormFile File { get; set; }
    }
    public enum Genre
    {
        // Fiction Genres
        Fantasy = 1,
        ScienceFiction = 2,
        Horror = 3,
        Mystery = 4,
        Romance = 5,
        HistoricalFiction = 6,
        Adventure = 7,
        Dystopian = 8,
        LiteraryFiction = 9,
        YoungAdult = 10,
        ShortStory = 11,

        // Nonfiction Genres
        Biography = 12,
        Autobiography = 13,
        History = 14,
        Cookbooks = 15,
        ArtAndPhotography = 16,
        HealthAndFitness = 17,
        SelfHelp = 18,
        Childrens = 19,
        CraftsAndHobbies = 20
    }
}
