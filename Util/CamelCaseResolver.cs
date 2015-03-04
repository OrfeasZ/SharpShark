using System;
using Newtonsoft.Json.Serialization;

namespace GS.Lib.Util
{
    class CamelCaseResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type p_ObjectType)
        {
            var s_Contract = base.CreateDictionaryContract(p_ObjectType);

            s_Contract.PropertyNameResolver = p_PropertyName => p_PropertyName;

            return s_Contract;
        }
    }
}
