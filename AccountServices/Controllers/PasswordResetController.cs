using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AccountServices.Data;
using AccountServices.Models;
using AccountServices.Classes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics;
using System.Security;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountServices.Controllers
{
    public class PasswordResetController : Controller
    {
        private readonly AccountServicesContext _context;
        private readonly IConfiguration configuration;

        public PasswordResetController(AccountServicesContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        // GET: PasswordReset
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> SubmitPasswordReset()
        {
            return View();
        }

        // POST: PasswordReset
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Id,Username,UserEmailAddress")] PasswordReset passwordReset)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmedPassword");
            ModelState.Remove("VerificationCode");
            ModelState.Remove("VerificationCode2");
            ModelState.Remove("CodeExpiry");

            if (ModelState.IsValid)
            {
                UserAccount user = new(configuration);
                user.SetUser(passwordReset.Username);
                VerificationCode code = new(passwordReset.UserEmailAddress);
                Console.WriteLine(passwordReset.Username);
                Console.WriteLine(passwordReset.UserEmailAddress);

                
                if (!user.Exists())
                {
                    Console.WriteLine("User doesn't exist");
                    ModelState.AddModelError("Username", @"Specified user does not exist");
                    return View(passwordReset);
                }

                if (user.GetEmailAddress() != passwordReset.UserEmailAddress)
                {
                    Console.WriteLine("Email doesn't match our records");
                    ModelState.AddModelError("UserEmailAddress", @"Specified email does not match email on record");
                    return View(passwordReset);
                }

                if (user.Enabled() == false)
                {
                    ModelState.AddModelError("Username", @"Specified account is disabled");
                    return View(passwordReset);
                }

                // look up username in database and get the entry if it exists
                var existingUserEntry = _context.PasswordReset.SingleOrDefault(x => x.Username == passwordReset.Username);
                DateTime utcDate = DateTime.UtcNow;

                if (existingUserEntry == null)
                {
                    passwordReset.VerificationCode = code.GetCode();
                    passwordReset.CodeExpiry = utcDate;
                    _context.Add(passwordReset);
                }
                else
                {
                    existingUserEntry.VerificationCode = code.GetCode();
                    existingUserEntry.CodeExpiry = utcDate;
                    _context.Update(existingUserEntry);
                }
                
                Email mailClient = new(configuration);
                (bool, string) mailResult = mailClient.SendCode(passwordReset.UserEmailAddress, code.GetCode());
                if (!mailResult.Item1)
                {
                    ModelState.AddModelError("UserEmailAddress", $@"{mailResult.Item2}");
                    return View(passwordReset);
                }
                

                await _context.SaveChangesAsync();
                return View("SubmitNewPassword");

            }
            Console.WriteLine("Model not valid");
            return View(passwordReset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitNewPassword([Bind("Id,VerificationCode2,Password,ConfirmedPassword")] PasswordReset passwordReset)
        {
            ModelState.Remove("CodeExpiry");
            ModelState.Remove("Username");
            ModelState.Remove("UserEmailAddress");

            if (ModelState.IsValid)
            {
                var existingUserEntry = _context.PasswordReset.SingleOrDefault(x => x.VerificationCode == passwordReset.VerificationCode2);
                VerificationCode codeValidator = new(configuration);

                if (existingUserEntry == null ||  
                    codeValidator.IsValid(existingUserEntry.CodeExpiry) == false ||
                    passwordReset.VerificationCode2 != existingUserEntry.VerificationCode)
                {
                    ModelState.AddModelError("VerificationCode2", @"Invalid code");
                    return View(passwordReset);
                }

                UserAccount user = new(configuration);
                user.SetUser(existingUserEntry.Username);
                (bool, string) resetResults = user.ResetPassword(passwordReset.Password);
                if (resetResults.Item1)
                {
                    existingUserEntry.VerificationCode = null;
                    _context.Update(existingUserEntry);
                    await _context.SaveChangesAsync();
                    return View("PasswordResetComplete");
                }
                else
                {
                    ModelState.AddModelError("Password", $@"{resetResults.Item2}");
                    return View(passwordReset); // update the view return here
                }
            }
            return View("SubmitNewPassword");
        }
    }
}
