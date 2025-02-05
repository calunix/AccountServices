namespace AccountServices.Classes
{
    public class HomePage
    {
        public string domain { get; }

        public HomePage(IConfiguration config)
        {
            this.domain = config.GetValue<string>("ActiveDirectory:DomainName");
        }
    }
}
