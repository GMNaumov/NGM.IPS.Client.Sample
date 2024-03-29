﻿using Intermech.Bars;
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

            // Назначаем текст всплывающей подсказки при наведении на кастомную кнопку 
            menuButton.ToolTipText = "Присвоить литеру на основании документа-решения";

            // Назначаем обработчик события нажатия на кнопку
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
                    IDBTypedObjectID ipsObjectID = _selectedItems.GetItemData(0, typeof(IDBTypedObjectID)) as IDBTypedObjectID;

                    if (IsObjectAssignable(ipsObjectID.ObjectType))
                    {
                        string ipsAttributeLetterCurrentValue = GetLetterCurrentValue(ipsObjectID.ObjectID);

                        if (string.IsNullOrWhiteSpace(ipsAttributeLetterCurrentValue))
                        {
                            long ipsDocumentLetterAssignerID = GetLetterAssignerDocId();

                            long createdRelationBetweenObjectAndDocument = CreateNewRelationBetweenObjectAndLiteraAssigner(ipsObjectID.ObjectID, ipsDocumentLetterAssignerID);
                            if (createdRelationBetweenObjectAndDocument == -1)
                            {
                                MessageBox.Show("Новая связь не создана. Косячок-с...");
                            }
                            else
                            {
                                GetLetterFromDocAndAssignToObject(ipsObjectID.ObjectID, ipsDocumentLetterAssignerID);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Эй, Ара, тормози! У объекта уже есть литера: {ipsAttributeLetterCurrentValue}. Надо думать что придумать...");
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


        /// <summary>
        /// Проверка выделенных в Навигаторе объектов IPS на возможность присвоения литеры
        /// </summary>
        /// <param name="ipsObjectTypeID">Идентификатор типа выделенного объекта IPS</param>
        /// <returns>true - если на выбранный тип объектов можно назначить литеру</returns>
        private bool IsObjectAssignable(int ipsObjectTypeID)
        {
            int detailObjectID = 1052;
            int assemblyObjectID = 1074;

            return (ipsObjectTypeID == detailObjectID) || (ipsObjectTypeID == assemblyObjectID);
        }


        /// <summary>
        /// Отображение стандартного окна IPS для выбора объектов с фильтрацией по типу объектов "Решение о присвоении литеры"
        /// </summary>
        /// <returns>Идентификатор выбранного объекта типа "Решение о присвоении литеры"</returns>
        private long GetLetterAssignerDocId()
        {
            // Получаем целочисленное значение типа объекта IPS по его GUID
            int ipsDocumentLetterAssignerID = MetaDataHelper.GetObjectTypeID(new Guid("95984126-f9fa-452e-b4ce-2bb4572bb347"));

            // Отображаем окно выбора объекта IPS. Используем вариант "попроще" - SelectObjects(). Аргументами SelectionOptions настраиваем процесс выбора объектов в окне. Удобненько.
            // Так же существует вариант "пожощще" - Select(), но там нужно разбираться с дескрипторами окон и прочей гадостью. Фу.
            long[] ipsDocumentLetterAssignerIDs = SelectionWindow.SelectObjects("Выберите объект", "Выберите документ-решение о присвоении литеры", ipsDocumentLetterAssignerID, SelectionOptions.HideTree | SelectionOptions.SelectObjects | SelectionOptions.DisableMultiselect);

            return ipsDocumentLetterAssignerIDs.Length == 1 ? ipsDocumentLetterAssignerIDs[0] : -1L;
        }

        /// <summary>
        /// Получение значения атрибута Литера по идентификатору версии объекта
        /// </summary>
        /// <returns>Текущее строковое значение атрибута Литера</returns>
        private string GetLetterCurrentValue(long ipsObjectVersionID)
        {
            string ipsAttributeLetterCurrentValue;

            // Значение атрибута Идентификатор аттрибута Литера
            int ipsAttributeLetterID = 1145;

            using (SessionKeeper sessionKeeper = new SessionKeeper())
            {
                // Получаем по идентификатору версии объекта и идентификатору атрибута объект типа IDBAttribute
                IDBAttribute attribute = sessionKeeper.Session.GetObjectAttribute(ipsObjectVersionID, ipsAttributeLetterID, false, false);
                ipsAttributeLetterCurrentValue = attribute.AsString;
            }

            return ipsAttributeLetterCurrentValue;
        }


        /// <summary>
        /// Создание новой связи объекта (Детали или Сборочной единицы) и Документа-решения о присвоении литеры
        /// </summary>
        /// <returns>Идентификатор созданной связи</returns>
        private long CreateNewRelationBetweenObjectAndLiteraAssigner(long ipsObjectID, long literaAssignerID)
        {
            long createdRelationBetweenObjectAndDocumentID = -1;

            // Идентификатор типа связи "Основание для присвоения литеры"
            // ToDo На разных установках IPS будет отличаться, необходо вынести в файл конфигурации
            int ipsRelationLiteraAssignID = 1212;

            try
            {
                using(SessionKeeper sessionKeeper = new SessionKeeper())
                {
                    // Получаем коллекцию связей типа "Основание для присвоения литеры"
                    IDBRelationCollection ipsRelationLetterAssignIDCollection = sessionKeeper.Session.GetRelationCollection(ipsRelationLiteraAssignID);

                    // Создаём новую связь типа "Основание для присвоения литеры"
                    createdRelationBetweenObjectAndDocumentID = ipsRelationLetterAssignIDCollection.Create(ipsObjectID, literaAssignerID).RelationID;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            return createdRelationBetweenObjectAndDocumentID;
        }

        /// <summary>
        /// Получаем документ-основание о присвоении литеры, связанный с объектом.
        /// Считываем с него значение атрибута "Литера" и назначаем её объекту.
        /// TODO пока всё делаем принудительно, но обязательно потербуется логика обработки литеры по старшинству
        /// </summary>
        /// <param name="ipsObjectID">Идентификатор версии объекта IPS</param>
        /// <returns>true - если литера с документа успешно назначена на объект. false - если что-то пошло не так</returns>
        private bool GetLetterFromDocAndAssignToObject(long ipsObjectID, long documentID)
        {
            bool result = false;

            // Системный идентификатор атрибута Литера
            int letterAttributeID = 1145;

            try
            {
                using(SessionKeeper sessionKeeper = new SessionKeeper())
                {
                    // По идентификатору документа и идентификатору атрибута Литера получаем значение атрибута с документа
                    IDBAttribute ipsDocumentLetterAttrubute = sessionKeeper.Session.GetObjectAttribute(documentID, letterAttributeID, false, false);
                    string ipsDocumentLetterAttributeValue = ipsDocumentLetterAttrubute.Value.ToString();
                    MessageBox.Show($"С документа считана литера: {ipsDocumentLetterAttributeValue}");


                    // По идентификатору объекта и идентификатору атрибута Литера получаем значение атрибута с объекта
                    IDBAttribute ipsObjectLetterAttribute = sessionKeeper.Session.GetObjectAttribute(ipsObjectID, letterAttributeID, false, false);
                    string ipsObjectLetterAttributeValue = ipsObjectLetterAttribute.Value.ToString();
                    MessageBox.Show($"С объекта считана литера: {ipsObjectLetterAttributeValue}");

                    ipsObjectLetterAttribute.AsString = ipsDocumentLetterAttributeValue;
                    MessageBox.Show($"Объекту назначена литера:  {ipsObjectLetterAttribute.Value}");
                    result = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            return result;
        }
    }
}
