using System.Collections;

public class Credentials {
	private string email;
	private string password;

	public Credentials(string email, string password)
	{
		this.email = email;
		this.password = password;
	}

	public string GetEmail()
	{
		return email;
	}

	public string GetPassword() 
	{
		return password;
	}
}
