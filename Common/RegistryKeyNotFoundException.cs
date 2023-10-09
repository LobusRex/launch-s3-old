namespace Common
{
	public class RegistryKeyNotFoundException : IOException
	{
		public RegistryKeyNotFoundException()
		{

		}

		public RegistryKeyNotFoundException(string message) : base(message)
		{

		}

		public RegistryKeyNotFoundException(string message, Exception inner) : base(message, inner)
		{

		}
	}
}
