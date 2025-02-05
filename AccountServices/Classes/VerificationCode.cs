using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace AccountServices.Classes
{
    public class VerificationCode
    {
        private readonly int verificationCode;
        private readonly IConfiguration configuration;
        private const int LOWER_BOUND = 10000000;
        private const int UPPER_BOUND = 100000000;

        public VerificationCode(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public VerificationCode(string emailAddress)
        {
            Random rnd = new(GenerateSeed(emailAddress));
            verificationCode = rnd.Next(LOWER_BOUND, UPPER_BOUND);
        }

        public string GetCode()
        {
            return verificationCode.ToString();
        }

        private static int GenerateSeed(string emailAddress)
        {
            int seed = 0;
            int alphaIndex;
            char c;
            DateTime utcDate = DateTime.UtcNow;
            seed += utcDate.Second + 60 * utcDate.Minute + 3600 * utcDate.Hour;

            for (int i = 0; i < emailAddress.Length; ++i)
            {
                c = emailAddress[i];
                if ( (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') )
                {
                    alphaIndex = (int)c % 32;
                    seed += alphaIndex;
                }
            }

            return seed;
        }

        public bool IsValid(DateTime timeStamp)
        {
            int maxAgeMinutes = this.configuration.GetValue<int>("VerificationCode:MaxAgeMinutes");
            DateTime currentUtcDate = DateTime.UtcNow;
            TimeSpan codeAge = currentUtcDate - timeStamp;

            if (codeAge.TotalMinutes < maxAgeMinutes)
            {
                return true;
            }

            return false;
        }
    }
}
