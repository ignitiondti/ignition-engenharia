namespace summary.api.Clients.Files
{
    public interface IFileReader
    {
        string ReadTxtFile(Stream stream);
        string ReadDocFile(Stream stream);
        string ReadDocxFile(Stream stream);
    }
}
