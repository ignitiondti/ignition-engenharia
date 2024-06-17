using summary.api.Models;

namespace summary.api.Exceptions
{
    public static class MessageHelp
    {
        public static MessageError Get(int code)
        {
            var messageDescription = MessagesResource.ResourceManager.GetString(code.ToString());

            if (string.IsNullOrEmpty(messageDescription)) return GetNonExistentMessage();

            return new MessageError()
            {
                Code = code,
                Description = messageDescription,
            };
        }
        internal static int NonExistentMessageCode => -1;
        internal static string NonExistentMessageDescription => "Falha ao obter mensagem de erro.";
        private static MessageError GetNonExistentMessage()
        {
            return new MessageError()
            {
                Code = NonExistentMessageCode,
                Description = NonExistentMessageDescription,
            };
        }
    }
}
