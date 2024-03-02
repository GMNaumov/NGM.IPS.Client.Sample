using Intermech.Bars;
using Intermech.DataFormats;
using Intermech.Interfaces.Plugins;
using Intermech.Navigator.ContextMenu;
using Intermech.Navigator.SelectionView;
using Intermech.Navigator.Interfaces;
using System;
using System.Windows.Forms;
using Intermech.Search;

namespace NGM.IPS.Client.AssignLetter
{
    public class Program : IPackage
    {
        public string Name => "Клиентский плагин для присвоения литеры";

        private static IServiceProvider _ipsServiceProvider;

        private static ISelectedItems _selectedItems;

        public void Load(IServiceProvider serviceProvider)
        {
            _ipsServiceProvider = serviceProvider;

            IPluginManager plugins = (IPluginManager)_ipsServiceProvider.GetService(typeof(IPluginManager));

            // Получаем ссылку на сервис управления главным меню IPS
            BarManager barManager = _ipsServiceProvider.GetService(typeof(BarManager)) as BarManager;

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

            menuButton.Click += LetterAssign;

            menuBar.Items.Add(menuItem);

            menuItem.Items.Add(menuButton);
        }

        private void LetterAssign(object sender, EventArgs e)
        {
            try
            {
                // Получаем объекты, выделенные пользователем в навигаторе IPS
                _selectedItems = SelectedItemsHelper.GetNavigatorSelection();

                int count = _selectedItems.Count;

                if (count > 10)
                {
                    MessageBox.Show("Выделено слишком много объектов");
                }
                else
                {
                    MessageBox.Show($"Выделено {count} объектов");

                    for (int i = 0; i < count; i++)
                    {
                        // Попытка получить описание следующего выделенного объекта из коллекции
                        IDBTypedObjectID objectID = _selectedItems.GetItemData(i, typeof(IDBTypedObjectID)) as IDBTypedObjectID;

                        MessageBox.Show($"objectID.ID: {objectID.ID} || objectID.ObjectType: {objectID.ObjectType} || objectID.ObjectID:  {objectID.ObjectID} || objectID.Owner:  {objectID.Owner} || objectID.Caption:  {objectID.Caption} || objectID.SiteID:  {objectID.SiteID}");
                    }
                }
            }
            catch (NullReferenceException exeption)
            {
                MessageBox.Show("Выделите объекты типа Сборочная единица или Деталь в Навигаторе IPS для назначения литеры");
            }
        }

        public void Unload()
        {

        }
    }
}
