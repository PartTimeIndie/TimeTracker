using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnityBase
{
    /// <summary>
    /// A class to make serialization easier
    /// </summary>
    public static class TimeTrackerSerializationHelper
    {
        /// <summary>
        /// Serializes the specified object to XML
        /// </summary>
        /// <typeparam name="T">The Type you want to serializes, only classes are possible</typeparam>
        /// <param name="objectToSerialize">The class to serialize.</param>
        /// <returns>The JSON as a string</returns>
        public static string Serialize<T>(T objectToSerialize) where T : class
        {
            if (objectToSerialize == null)
            {
                return null;
            }

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings
            {
                Indent = true,               // Enable indentation
                IndentChars = "  ",          // Use two spaces for indentation
                NewLineOnAttributes = false // Keep attributes on the same line
            };

            using (var stringWriter = new StringWriter())
            using (var writer = XmlWriter.Create(stringWriter, settings))
            {
                serializer.Serialize(writer, objectToSerialize);
                return stringWriter.ToString();
            }
        }


        /// <summary>
        /// Deserializes the class from a Xml string
        /// </summary>
        /// <typeparam name="T">The type of the class you want to deserialize</typeparam>
        /// <param name="objectString">Xml Representation of the object as a string</param>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(string objectString) where T : class
        {
            if (string.IsNullOrEmpty(objectString))
            {
                return null;
            }

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(objectString))
            using (var reader = XmlReader.Create(stringReader))
            {
                return serializer.Deserialize(reader) as T;
            }
        }

    }
}
