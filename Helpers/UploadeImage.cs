namespace TazaFood_API.Helpers
{
    public static class UploadeImage
    {
        public static async Task<string> SaveImage(IFormFile formfile,string source,string imageName)
        {
            //upload images firstand save it on the database 

            string filePath = source + "/images/products";

            //ask if the folder is exixted or not 
            if (!Directory.Exists(filePath))
            {
                //create folder with this path 
                Directory.CreateDirectory(filePath);
            }
            //string ImagePath = filePath + "/"+name+".png";

            string fileName = $"{imageName}_{DateTime.Now.Ticks}{Path.GetExtension(formfile.FileName)}";
            string ImagePath = Path.Combine(filePath, fileName);

            //handle the point of the image is existed
            if (File.Exists(ImagePath))
            {
                File.Delete(ImagePath);
            }

            using (FileStream stream = File.Create(ImagePath))
            {
                await formfile.CopyToAsync(stream);
            }
            return  ImagePath;
        }
    }
}
