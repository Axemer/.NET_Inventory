using System;
using System.DirectoryServices.AccountManagement;

public class ActiveDirectoryAuth
{
    private readonly string _domain;

    public ActiveDirectoryAuth(string domain)
    {
        _domain = domain;
    } //

    public bool AuthenticateUser(string username, string password)
    {
        try
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, _domain))
            {
                return context.ValidateCredentials(username, password);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка аутентификации: {ex.Message}");
            return false;
        }
    }
}