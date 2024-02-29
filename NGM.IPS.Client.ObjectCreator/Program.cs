using Intermech.Interfaces.Plugins;
using NGM.IPS.Client.ObjectCreator.UserInterface;
using System;

namespace NGM.IPS.Client.ObjectCreator
{
    public class Program : IPackage
    {
        public string Name => "Клиентское расширение IPS для создания новых объектов";

        private static IServiceProvider _ipsServiceProvider;

        public void Load(IServiceProvider serviceProvider)
        {
            _ipsServiceProvider = serviceProvider;

            UserMenu userMenu = new UserMenu();
            
            
        }

        public void Unload()
        {
            
        }
    }
}
