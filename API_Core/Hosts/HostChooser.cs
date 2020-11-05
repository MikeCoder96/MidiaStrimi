using API_Core.Hosts.Websites;

namespace API_Core.Hosts
{
    public static class HostChooser
    {
        public static AbstractStreamPage chooseWebsite(int option)
        {
            switch (option)
            {
                case 1:
                    return new CB01_Wrapper();

                case 2:
                    return new AltaDefinizione_Wrapper();
                default:
                    return new CB01_Wrapper();
            }
        }
    }
}
