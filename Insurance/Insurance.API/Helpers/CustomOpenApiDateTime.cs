using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace Insurance.API.Helpers
{
    /// <summary>
    /// Changes the default ISO DateFormat to MM/dd/yyyy HH:mm:ss
    /// </summary>
    public class CustomOpenApiDateTime : IOpenApiPrimitive
    {
        /// <summary>
        /// Initializes
        /// </summary>
        /// <param name="value"></param>
        public CustomOpenApiDateTime(DateTime value)
        {
            Value = value;
        }

        /// <summary>
        /// OpenApiType
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Primitive;

        /// <summary>
        /// DateTime format
        /// </summary>
        public string Format { get; set; } = "MM/dd/yyyy HH:mm:ss";

        /// <summary>
        /// Primitive type as Datetime
        /// </summary>
        public PrimitiveType PrimitiveType { get; } = PrimitiveType.DateTime;

        /// <summary>
        /// DateTime value, its readonly
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="specVersion"></param>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion) =>
            writer.WriteValue(Value.ToString(Format));
    }
}
