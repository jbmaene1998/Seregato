using System;
using System.Diagnostics;

namespace Seregato
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var funBot = new Process())
            {
                funBot.StartInfo.FileName = @"C:\Users\jbmae\source\repos\Seregato-Moderation\Seragato-Fun\bin\Debug\net5.0\Seregato-Fun.exe";
                funBot.Start();
            }
            using (var managerBot = new Process())
            {
                managerBot.StartInfo.FileName = @"C:\Users\jbmae\source\repos\Seregato-Moderation\Seregato-Manager\bin\Debug\net5.0\Seregato-Manager.exe";
                managerBot.Start();
            }
            using (var miscBot = new Process())
            {
                miscBot.StartInfo.FileName = @"C:\Users\jbmae\source\repos\Seregato-Moderation\Seregato-Misc\bin\Debug\net5.0\Seregato-Misc.exe";
                miscBot.Start();
            }
            using (var moderationBot = new Process())
            {
                moderationBot.StartInfo.FileName = @"C:\Users\jbmae\source\repos\Seregato-Moderation\Seregato-Moderation\bin\Debug\net5.0\Seregato-Moderation.exe";
                moderationBot.Start();
            }
            using (var rolesBot = new Process())
            {
                rolesBot.StartInfo.FileName = @"C:\Users\jbmae\source\repos\Seregato-Moderation\Seregato-Roles\bin\Debug\net5.0\Seregato-Roles.exe";
                rolesBot.Start();
            }
            Console.ReadKey();
        }
    }
}
