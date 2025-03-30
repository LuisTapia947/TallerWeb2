namespace Necli.WebApi.Exceptions;

public class NegocioException : Exception
{
    public NegocioException(string mensaje) : base(mensaje) { }
}
