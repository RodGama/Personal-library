using System.ComponentModel.DataAnnotations;

namespace Modules.Books.Domain
{
    public class Book
    {
        public Guid Id { get; set; }
        private string _title;
        private string _author;
        private string _description;
        private string _genre;
        private string _publisher;
        private string _imageBase64;
        private long _isbn;
        private Guid _userId;
        public string Title
        {
            get => _title;
            set => _title = value?.Trim();
        }
        public string Author
        {
            get => _author;
            set => _author = value?.Trim();
        }
        public string Description
        {
            get => _description;
            set => _description = value?.Trim();
        }
        public string Publisher
        {
            get => _publisher;
            set => _publisher = value?.Trim();
        }
        public string Genre
        {
            get => _genre;
            set => _genre = value?.Trim();
        }
        public string ImageBase64
        {
            get => _imageBase64;
            set => _imageBase64 = value?.Trim();
        }
        public long ISBN
        {
            get => _isbn;
            set => _isbn = value;
        }
        public Guid UserId
        {
            get => _userId;
            set => _userId = value;
        }
    }
}
