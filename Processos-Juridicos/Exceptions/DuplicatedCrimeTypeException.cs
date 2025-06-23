namespace Processos_Juridicos.Exceptions
{
    public class DuplicatedCrimeTypeException : Exception
    {
        public DuplicatedCrimeTypeException(string message) : base(message) 
        {
        }
    }
}
