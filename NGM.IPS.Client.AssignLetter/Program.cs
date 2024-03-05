using Intermech.Bars;
using Intermech.DataFormats;
using Intermech.Interfaces;
using Intermech.Interfaces.Client;
using Intermech.Interfaces.Plugins;
using Intermech.Navigator;
using Intermech.Navigator.Interfaces;
using Intermech.Search;
using System;
using System.Windows.Forms;

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

                if (count == 1)
                {
                    IDBTypedObjectID objectID = _selectedItems.GetItemData(0, typeof(IDBTypedObjectID)) as IDBTypedObjectID;

                    if (IsObjectAssignable(objectID.ObjectType))
                    {
                        string letterCurrentValue = GetLetterCurrentValue(objectID.ObjectID);

                        if (string.IsNullOrWhiteSpace(letterCurrentValue))
                        {
                            MessageBox.Show("Литера документу не назначена. Продолжаем разговор!");
                            long letterAssignerDocID = GetLetterAssignerDocId();
                            MessageBox.Show(letterAssignerDocID.ToString());
                        }
                        else
                        {
                            MessageBox.Show($"Эй, Ара, тормози! У объекта уже есть литера: {letterCurrentValue}. Надо думать что придумать...");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Для данного типа объекта нельзя назначить решение о присвоении литеры.\n\nВыберите тип объекта Деталь или Сборочная единица", "Некорректный тип объекта", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
                else
                {
                    MessageBox.Show("Выделено слишком много объектов");
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

        private bool IsObjectAssignable(int objectType)
        {
            return (objectType == 1052) || (objectType == 1074);
        }


        /// <summary>
        /// Отображение стандартного окна IPS для выбора объектов с фильтрацией по типу объектов "Решение о присвоении литеры"
        /// </summary>
        /// <returns>Идентификатор выбранного объекта типа "Решение о присвоении литеры"</returns>
        private long GetLetterAssignerDocId()
        {
            // Получаем целочисленное значение типа объекта IPS по его GUID
            int letterAssignerDocTypeID = MetaDataHelper.GetObjectTypeID(new Guid("95984126-f9fa-452e-b4ce-2bb4572bb347"));

            // Отображаем окно выбора объекта IPS. Используем вариант "попроще" - SelectObjects(). Аргументами SelectionOptions настраиваем процесс выбора объектов в окне. Удобненько.
            // Так же существует вариант "пожощще" - Select(), но там нужно разбираться с дескрипторами окон и прочей гадостью. Фу.
            long[] letterAssignerDocsIDs = SelectionWindow.SelectObjects("Выберите объект", "Выберите документ-решение о присвоении литеры", letterAssignerDocTypeID, SelectionOptions.HideTree | SelectionOptions.SelectObjects | SelectionOptions.DisableMultiselect);

            return letterAssignerDocsIDs.Length == 1 ? letterAssignerDocsIDs[0] : -1L;
        }

        /// <summary>
        /// Получение значения атрибута Литера по идентификатору версии объекта
        /// </summary>
        /// <returns>Текущее строковое значение атрибута Литера</returns>
        private string GetLetterCurrentValue(long objectVersionID)
        {
            string letterCurrentValue;

            // Значение атрибута Идентификатор аттрибута Литера
            int letterAttributeID = 1145;

            using (SessionKeeper sessionKeeper = new SessionKeeper())
            {
                IDBAttribute attribute = sessionKeeper.Session.GetObjectAttribute(objectVersionID, letterAttributeID, false, false);
                letterCurrentValue = attribute.AsString;
            }

            return letterCurrentValue;
        }
    }
}
