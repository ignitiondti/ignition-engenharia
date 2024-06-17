using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using NPOI.XWPF.Extractor;
using NPOI.XWPF.UserModel;
using System.Text;

namespace summary.api.Clients.Files
{
    public class FileReader : IFileReader
    {
        public string ReadTxtFile(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public string ReadDocFile(Stream stream)
        {
            var document = new XWPFDocument(stream);
            var wordExtractor = new XWPFWordExtractor(document);
            return wordExtractor.Text;
        }

        public string ReadDocxFile(Stream stream)
        {
            var stringBuilder = new StringBuilder();

            using var wordDocument = WordprocessingDocument.Open(stream, false);
            Body body = wordDocument.MainDocumentPart.Document.Body;

            foreach (var paragraph in body.Elements<Paragraph>())
            {
                foreach (var run in paragraph.Elements<Run>())
                {
                    stringBuilder.Append(run.InnerText);
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
