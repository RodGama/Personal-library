namespace App.MVC.DTOs
{
    public class BookDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public long ISBN { get; set; }
        public string Publisher { get; set; }
        public string ImageBase64 { get; set; }            
    }
}
