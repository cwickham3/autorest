// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using Microsoft.Rest.Generator.ClientModel;
using Microsoft.Rest.Generator.Utilities;
using Microsoft.Rest.Modeler.Swagger.Model;

namespace Microsoft.Rest.Modeler.Swagger
{
    /// <summary>
    /// The builder for building a generic swagger object into parameters, 
    /// service types or Json serialization types.
    /// </summary>
    public class ObjectBuilder
    {
        protected SwaggerObject SwaggerObject { get; set; }
        protected SwaggerModeler Modeler { get; set; }

        public ObjectBuilder(SwaggerObject swaggerObject, SwaggerModeler modeler)
        {
            SwaggerObject = swaggerObject;
            Modeler = modeler;
        }

        public virtual IType ParentBuildServiceType(string serviceTypeName)
        {
            // Should not try to get parent from generic swagger object builder
            throw new InvalidOperationException();
        }

        /// <summary>
        /// The visitor method for building service types. This is called when an instance of this class is
        /// visiting a _swaggerModeler to build a service type.
        /// </summary>
        /// <param name="serviceTypeName">name for the service type</param>
        /// <returns>built service type</returns>
        public virtual IType BuildServiceType(string serviceTypeName)
        {
            PrimaryType type = SwaggerObject.ToType();
            if (SwaggerObject.Enum != null && SwaggerObject.Enum.Any() && type == PrimaryType.String)
            {
                var enumType = new EnumType();
                SwaggerObject.Enum.ForEach(v => enumType.Values.Add(new EnumValue { Name = v, SerializedName = v }));
                if (SwaggerObject.Extensions.ContainsKey("x-ms-enum"))
                {
                    enumType.IsExpandable = false;
                    enumType.Name = SwaggerObject.Extensions["x-ms-enum"] as string;
                    enumType.SerializedName = enumType.Name;
                    if (string.IsNullOrEmpty(enumType.Name))
                    {
                        throw new InvalidOperationException("x-ms-enum extension needs to specify an enum name.");
                    }
                    var existingEnum =
                        Modeler.ServiceClient.EnumTypes.FirstOrDefault(
                            e => e.Name.Equals(enumType.Name, StringComparison.OrdinalIgnoreCase));
                    if (existingEnum != null)
                    {
                        if (!existingEnum.Equals(enumType))
                        {
                            throw new InvalidOperationException(
                                string.Format(CultureInfo.InvariantCulture,
                                    "Swagger document contains two or more x-ms-enum extensions with the same name '{0}' and different values.",
                                    enumType.Name));
                        }
                    }
                    else
                    {
                        Modeler.ServiceClient.EnumTypes.Add(enumType);
                    }
                }
                else
                {
                    enumType.IsExpandable = true;
                    enumType.Name = string.Empty;
                    enumType.SerializedName = string.Empty;
                }
                return enumType;
            }
            if (SwaggerObject.Type == DataType.Array)
            {
                string itemServiceTypeName;
                if (SwaggerObject.Items.Reference != null)
                {
                    itemServiceTypeName = SwaggerObject.Items.Reference.StripDefinitionPath();
                }
                else
                {
                    itemServiceTypeName = serviceTypeName + "Item";
                }

                var elementType =
                    SwaggerObject.Items.GetBuilder(Modeler).BuildServiceType(itemServiceTypeName);
                return new SequenceType
                {
                    ElementType = elementType
                };
            }
            if (SwaggerObject.AdditionalProperties != null)
            {
                string dictionaryValueServiceTypeName;
                if (SwaggerObject.AdditionalProperties.Reference != null)
                {
                    dictionaryValueServiceTypeName = SwaggerObject.AdditionalProperties.Reference.StripDefinitionPath();
                }
                else
                {
                    dictionaryValueServiceTypeName = serviceTypeName + "Value";
                }
                return new DictionaryType
                {
                    ValueType =
                        SwaggerObject.AdditionalProperties.GetBuilder(Modeler)
                            .BuildServiceType((dictionaryValueServiceTypeName))
                };
            }

            return type;
        }
    }
}