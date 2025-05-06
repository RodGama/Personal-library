namespace App.MVC.Extensions
{
    public static class ImageExtension
    {
        public static string ConvertToBase64(this IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                var imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }
        public static string GetImageType(this IFormFile file)
        {
            return file.ContentType;
        }
    }
}
