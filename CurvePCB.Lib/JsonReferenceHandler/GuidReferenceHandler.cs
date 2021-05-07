using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CurvePCB.Lib.JsonReferenceHandler
{

    public class GuidReferenceHandler : ReferenceHandler
    {
        public GuidReferenceHandler() => Reset();
        private ReferenceResolver _rootedResolver;
        public override ReferenceResolver CreateResolver() => _rootedResolver;
        public void Reset() => _rootedResolver = new GuidReferenceResolver();
    }

    public class GuidReferenceResolver : ReferenceResolver
    {
        private readonly Dictionary<string, object> _referenceIdToObjectMap = new();
        private readonly Dictionary<object, string> _objectToReferenceIdMap = new(ReferenceEqualityComparer.Instance);

        public override void AddReference(string referenceId, object value)
        {
            if (!_referenceIdToObjectMap.TryAdd(referenceId, value))
            {
                throw new JsonException();
            }
        }

        public override string GetReference(object value, out bool alreadyExists)
        {
            if (_objectToReferenceIdMap.TryGetValue(value, out string referenceId))
            {
                alreadyExists = true;
            }
            else
            {
                referenceId = Guid.NewGuid().ToString();
                _objectToReferenceIdMap.Add(value, referenceId);
                alreadyExists = false;
            }

            return referenceId;
        }

        public override object ResolveReference(string referenceId)
        {
            if (!_referenceIdToObjectMap.TryGetValue(referenceId, out object value))
            {
                throw new JsonException();
            }

            return value;
        }
    }
}
