using Intermech.Bars;
using Intermech.Interfaces.Plugins;
using System;

namespace NGM.IPS.Client.AssignLetter
{
    public class Program : IPackage
    {
        public string Name => "Клиентский плагин для присвоения литеры";

        private static IServiceProvider _ipsServiceProvider;

        public void Load(IServiceProvider serviceProvider)
        {
            _ipsServiceProvider = serviceProvider;

            IPluginManager plugins = (IPluginManager)_ipsServiceProvider.GetService(typeof(IPluginManager));

            // Получаем ссылку на сервис управления главным меню IPS
            BarManager barManager = _ipsServiceProvider.GetService(typeof (BarManager)) as BarManager;

            // Получаем ссылку на главное меню IPS
            MenuBar menuBar = barManager.MenuBar;

            // Имя, которое будет отображатся в главном меню IPS
            MenuItemBase menuItem = new MenuBarItem("Клиентские плагины");

            // Уникальное имя элемента, по которому будет выполняться поиск методом FindMenuBar()
            menuItem.CommandName = "MenuClientPlugins";

            // Создаём новую кнопку элемента меню, которая будет отображаться в кастомном меню
            MenuButtonItem menuButton = new MenuButtonItem("Присвоить литеру");

            // Уникальное имя кнопки, по которому будет выполняться поиск 
            menuButton.CommandName = "AssignLetterButton";

            menuButton.ToolTipText = "Присвоить литеру на основании документа-решения";
            
            menuBar.Items.Add(menuItem);
            
            menuItem.Items.Add(menuButton);
        }

        public void Unload()
        {
            
        }
    }
}
