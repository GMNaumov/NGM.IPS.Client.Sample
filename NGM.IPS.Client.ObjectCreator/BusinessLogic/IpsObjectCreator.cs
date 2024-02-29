using Intermech.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGM.IPS.Client.ObjectCreator.BusinessLogic
{
    internal class IpsObjectCreator : IObjectCreator
    {
        public int CreateNewObject(Guid objectType, string objectDesignation, string objectName)
        {
            int createdObjectId = -1;

            using (SessionKeeper sessionKeeper = new SessionKeeper())
            {
                // Получаем из метаданных идентификатор типа создаваемого объекта
                int createdObjectTypeId = MetaDataHelper.GetObjectTypeID(objectType);

                // Получаем коллекцию объектов выбранного типа
                IDBObjectCollection objectCollection = sessionKeeper.Session.GetObjectCollection(createdObjectTypeId);

                // Создаём заготовку нового объекта выбранного типа
                IDBObject createdObjectDummy = objectCollection.Create();

                IDBAttribute createdPkiAttribute = createdObjectDummy.Attributes.FindByGUID(IpsObjectAttributesUtil.Designation);    
            }

            return createdObjectId;
        }
    }
}
