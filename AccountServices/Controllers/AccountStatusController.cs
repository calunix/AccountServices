using AccountServices.Classes;
using AccountServices.Data;
using AccountServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices.AccountManagement;

namespace AccountServices.Controllers
{
    public class AccountStatusController : Controller
    {
        private readonly IConfiguration configuration;

        public AccountStatusController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccountStatusResult()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("EmailAddress")] AccountStatus accountInfo)
        {
            ModelState.Remove("AccountStatusResults");

            if (ModelState.IsValid)
            {
                UserAccount user = new(configuration);
                PrincipalSearchResult<Principal> accountsFound = user.FindUserByEmail(accountInfo.EmailAddress);

                int count = 0;
                foreach (Principal account in accountsFound)
                {
                    count++;
                }

                if (count == 0)
                {
                    accountInfo.AccountStatusResults += "None found\n";
                }

                Console.WriteLine("Number of accounts found: " + count);

                UserAccount userNeedingStatus;
                foreach (Principal account in accountsFound)
                {
                    userNeedingStatus = new(configuration);
                    userNeedingStatus.SetUser(account.SamAccountName);
                    accountInfo.AccountStatusResults += $"Username: {account.SamAccountName}\n";
                    accountInfo.AccountStatusResults += $"Account enabled: {userNeedingStatus.Enabled()}\n";
                    accountInfo.AccountStatusResults += $"Password expires: {userNeedingStatus.GetPasswordExpiry()}\n";
                    accountInfo.AccountStatusResults += " \n";
                    Console.WriteLine(accountInfo.AccountStatusResults);
                }
                

                return View("AccountStatusResult", accountInfo);
            }

            return View();
        }
    }
}
