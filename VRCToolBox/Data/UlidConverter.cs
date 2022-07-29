using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace VRCToolBox.Data
{
    class UlidConverter
    {
    }
    public class UlidToBytesConverter : ValueConverter<Ulid, byte[]>
    {
        private static readonly ConverterMappingHints defaultHints = new ConverterMappingHints(size: 16);

        public UlidToBytesConverter(ConverterMappingHints? mappingHints = null)
            : base(
                    convertToProviderExpression: x => x.ToByteArray(),
                    convertFromProviderExpression: x => new Ulid(x),
                    mappingHints: defaultHints.With(mappingHints))
        {
        }
    }

    public class UlidToStringConverter : ValueConverter<Ulid, string>
    {
        private static readonly ConverterMappingHints defaultHints = new ConverterMappingHints(size: 26);

        public UlidToStringConverter(ConverterMappingHints? mappingHints = null)
            : base(
                    convertToProviderExpression: x => x.ToString(),
                    convertFromProviderExpression: x => Ulid.Parse(x),
                    mappingHints: defaultHints.With(mappingHints))
        {
        }
    }
}
