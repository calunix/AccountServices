using System.Diagnostics;
using System.DirectoryServices.AccountManagement;

namespace AccountServices.Classes
{
    public class UserAccount
    {
        private readonly IConfiguration config;
        private readonly PrincipalContext domainContext;
        private UserPrincipal userAccount;
        private string samAccountName;

        public UserAccount(IConfiguration config)
        {
            this.config = config;
            string domain = config.GetValue<string>("ActiveDirectory:DomainName");
            string accessUser = config.GetValue<string>("ActiveDirectory:ServiceAccountUser");
            string accessPass = config.GetValue<string>("ActiveDirectory:ServiceAccountPass");

            this.domainContext = new(ContextType.Domain, domain, accessUser, accessPass);
        }

        public void SetUser(string samAccountName)
        {
            this.samAccountName = samAccountName;
            this.userAccount = UserPrincipal.FindByIdentity(this.domainContext, samAccountName);
        }

        public (bool, string) ResetPassword(string newPassword)
        {
            bool resetSuccess = false;
            string errorMessage = "none";
            
            try
            {
                this.userAccount.SetPassword(newPassword);
                this.userAccount.Save(domainContext);
                resetSuccess = true;
            }
            catch (Exception ex) {
                errorMessage = ex.Message;
            }

            return (resetSuccess, errorMessage);
        }

        public bool Exists()
        {
            bool exists;

            if (this.userAccount == null)
            {
                exists = false;
            }
            else
            {
                exists = true;
            }
            return exists;
        }

        public string GetEmailAddress()
        {  
            string email = this.userAccount.EmailAddress;
            return email;
        }

        public bool Enabled()
        {
            if (this.userAccount.Enabled == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string? GetPasswordExpiry()
        {
            string? expiry = null;

            Process netCommand = new();
            netCommand.StartInfo.FileName = "powershell.exe";
            netCommand.StartInfo.Arguments = $"net user {this.samAccountName} /domain";
            netCommand.StartInfo.UseShellExecute = false;
            netCommand.StartInfo.RedirectStandardOutput = true;
            netCommand.StartInfo.RedirectStandardError = true;
            netCommand.Start();
            string netCommandResults = netCommand.StandardOutput.ReadToEnd() + "\n" + netCommand.StandardError.ReadToEnd();
            netCommand.WaitForExit();

            if (netCommandResults.Contains("The user name could not be found."))
            {
                return expiry;
            }

            string[] lines = netCommandResults.Split('\n');
            foreach (string line in lines)
            {
                if (line.Contains("Password expires")) {
                    expiry = line;
                    break;
                }
            }

            for (int i = 0; i < expiry.Length; ++i)
            {
                if (char.IsDigit(expiry[i]))
                {
                    expiry = expiry.Substring(i);
                    break;
                }
            }

            return expiry;
        }
         
        public PrincipalSearchResult<Principal> FindUserByEmail(string email)
        {
            UserPrincipal user = new(this.domainContext);
            user.EmailAddress = email;
            PrincipalSearcher ps = new PrincipalSearcher();
            ps.QueryFilter = user;
            PrincipalSearchResult<Principal> results = ps.FindAll();
            
            return results;
        }
    }
}
