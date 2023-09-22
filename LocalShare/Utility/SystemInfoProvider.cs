using System;

namespace LocalShare.Utility
{
    internal class SystemInfoProvider
    {

        public static string GetPCName()
        {
            return $"{Environment.UserName} @ {Environment.MachineName}";
        }


    }
}
