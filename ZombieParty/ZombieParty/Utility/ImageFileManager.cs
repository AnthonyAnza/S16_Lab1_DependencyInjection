namespace ZombieParty.Utility
{
    public class ImageFileManager : IImageFileManager
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageFileManager(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string? UploadImage(IFormCollection form, bool edit, string? oldImagePath = null)
        {
            var files = form.Files; 

            if (files.Count > 0)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                
                var uploads = webRootPath + @"\images\zombie\";
                var extension = Path.GetExtension(files[0].FileName);

                // Créer un cannal pour transférer le fichier 
                using (var filesStreams = new FileStream(uploads + fileName + extension, FileMode.Create))
                {
                    files[0].CopyTo(filesStreams);
                }

                
                if (edit && !string.IsNullOrWhiteSpace(oldImagePath))
                {
                    try
                    {
                        System.IO.File.Delete(uploads + oldImagePath);
                    }
                    catch { }
                }

                return fileName + extension;
            }

            return oldImagePath;
        }
    }
}