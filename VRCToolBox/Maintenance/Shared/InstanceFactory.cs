using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Shared
{
    internal class InstanceFactory
    {
        internal static T CreateInstance<T>(object[] parameters) where T : class, IDataModel
        {
            T instance;

            if (typeof(T).GenericTypeArguments.Contains(typeof(AvatarData)))
            {
                _ = parameters.Length switch
                {
                    0 => throw new ArgumentNullException("parameters"),
                    1 => instance =(T)(IDataModel)new DataModelWithAuthor<AvatarData>((IDBOperator)parameters[0]),
                    2 => instance =(T)(IDataModel)new DataModelWithAuthor<AvatarData>((IDBOperator)parameters[0], parameters[1]),
                    _ => throw new InvalidOperationException(),
                };
                return instance;
            }

            if (typeof(T).GenericTypeArguments.Contains(typeof(WorldData)))
            {
                _ = parameters.Length switch
                {
                    0 => throw new ArgumentNullException("parameters"),
                    1 => instance = (T)(IDataModel)new DataModelWithAuthor<WorldData>((IDBOperator)parameters[0]),
                    2 => instance = (T)(IDataModel)new DataModelWithAuthor<WorldData>((IDBOperator)parameters[0], parameters[1]),
                    _ => throw new InvalidOperationException(),
                };
                return instance;
            }

            if (typeof(T) == typeof(DataModelUser)) 
            {
                _ = parameters.Length switch
                {
                    0 => throw new ArgumentNullException("parameters"),
                    1 => instance = (T)(IDataModel)new DataModelUser((IDBOperator)parameters[0]),
                    2 => instance = (T)(IDataModel)new DataModelUser((IDBOperator)parameters[0], parameters[1]),
                    _ => throw new InvalidOperationException(),
                };
                return instance;
            }
            throw new NotSupportedException();
        }

    }
}
