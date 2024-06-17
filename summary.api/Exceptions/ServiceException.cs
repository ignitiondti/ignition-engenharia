using summary.api.Models;

namespace summary.api.Exceptions
{
    public class ServiceException : Exception
    {
        public MessageError Error { get; set; }

        /// <summary>
        /// Código de erro, pode se diferenciar do código contido dentro da propriedade Error caso a mesma não exista em base.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Construtor que recebe o código da mensagem e monta o objeto de erro.
        /// </summary>
        /// <param name="code">Código da mensagem</param>
        public ServiceException(int code)
        {
            Error = MessageHelp.Get(code);
            Code = code;
        }

    }
}
