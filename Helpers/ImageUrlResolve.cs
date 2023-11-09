namespace TazaFood_API.Helpers
{
    public static class ImageUrlResolve
    {
        public static string ResolveUrl(string url)
        {
            string path = url;
            string delimiter = "wwwroot/";

            int index = path.LastIndexOf(delimiter);

            if (index != -1)
            {
                string result =  path.Substring(index + delimiter.Length);
                return  result;
            }
            else
            {
                return string.Empty;
            }

        }
    }
}
