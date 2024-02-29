using Intermech.Interfaces.Plugins;
using System;
using System.Windows.Forms;

namespace NGM.IPS.Client.HelloWorld
{
    public class Program : IPackage
    {
        // Имя, отображаемое в описании модуля расширения
        public string Name => "Пример клиентского расширения IPS";

        private static IServiceProvider _ipsServiceProvider;

        public void Load(IServiceProvider serviceProvider)
        {
            _ipsServiceProvider = serviceProvider;

            IPluginManager plugins = (IPluginManager)_ipsServiceProvider.GetService(typeof(IPluginManager));
            plugins.LoadComplete += PluginLoadComplete;
        }

        public void Unload()
        {
            // Оставлено для совместимости со старыми версиями IPS. Не чеши, и оно тебя не побеспокоит.
        }

        private void PluginLoadComplete(object sender, EventArgs e)
        {
            MessageBox.Show("Привет, IPS!");
        }
    }
}
