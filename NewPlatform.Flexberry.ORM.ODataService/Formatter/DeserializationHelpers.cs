﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace NewPlatform.Flexberry.ORM.ODataService.Formatter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Common;
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    // using System.Web.Http;
    using Microsoft.AspNet.OData.Formatter.Deserialization;
    using NewPlatform.Flexberry.ORM.ODataService.Expressions;

    internal static class DeserializationHelpers
    {
        internal static void ApplyProperty(ODataProperty property, IEdmStructuredTypeReference resourceType, object resource,
            ODataDeserializerProvider deserializerProvider, ODataDeserializerContext readContext)
        {
            IEdmProperty edmProperty = resourceType.FindProperty(property.Name);

            bool isDynamicProperty = false;
            string propertyName = property.Name;
            if (edmProperty != null)
            {
                /*
                 * Закомментировано, потому что это является ошибкой Microsoft. Нельзя использовать
                 * имена свойств DataObject в присвоениях значений свойств в EdmEntityObject.
                propertyName = EdmLibHelpers.GetClrPropertyName(edmProperty, readContext.Model);
                */
            }
            else
            {
                IEdmStructuredType structuredType = resourceType.StructuredDefinition();
                isDynamicProperty = structuredType != null && structuredType.IsOpen;
            }

            // dynamic properties have null values
            IEdmTypeReference propertyType = edmProperty != null ? edmProperty.Type : null;

            EdmTypeKind propertyKind;
            object value = ConvertValue(property.Value, ref propertyType, deserializerProvider, readContext,
                out propertyKind);

            if (isDynamicProperty)
            {
                SetDynamicProperty(resource, resourceType, propertyKind, propertyName, value, propertyType,
                    readContext);
            }
            else
            {
                SetDeclaredProperty(resource, propertyKind, propertyName, value, edmProperty, readContext);
            }
        }

        internal static void SetDynamicProperty(object resource, IEdmStructuredTypeReference resourceType,
            EdmTypeKind propertyKind, string propertyName, object propertyValue, IEdmTypeReference propertyType,
            ODataDeserializerContext readContext)
        {  
            if (propertyKind == EdmTypeKind.Collection && propertyValue.GetType() != typeof(EdmComplexObjectCollection)
                && propertyValue.GetType() != typeof(EdmEnumObjectCollection))
            {
                SetDynamicCollectionProperty(resource, propertyName, propertyValue, propertyType.AsCollection(),
                    resourceType.StructuredDefinition(), readContext);
            }
            else
            {
                SetDynamicProperty(resource, propertyName, propertyValue, resourceType.StructuredDefinition(),
                    readContext);
            }
        }

        internal static void SetDeclaredProperty(object resource, EdmTypeKind propertyKind, string propertyName,
            object propertyValue, IEdmProperty edmProperty, ODataDeserializerContext readContext)
        {
            if (propertyKind == EdmTypeKind.Collection)
            {
                SetCollectionProperty(resource, edmProperty, propertyValue, propertyName);
            }
            else
            {
                var isUntypedProp = readContext.GetType().GetProperty("IsUntyped", BindingFlags.NonPublic | BindingFlags.Instance);
                bool isUntyped = (bool)isUntypedProp.GetValue(readContext, null);

                //if (!readContext.IsUntyped)
                if (!isUntyped)
                {
                    if (propertyKind == EdmTypeKind.Primitive)
                    {
                        propertyValue = EdmPrimitiveHelpers.ConvertPrimitiveValue(propertyValue,
                            GetPropertyType(resource, propertyName));
                    }
                }

                SetProperty(resource, propertyName, propertyValue);
            }
        }

        internal static void SetCollectionProperty(object resource, IEdmProperty edmProperty, object value, string propertyName)
        {
            if (edmProperty == null)
            {
                throw new ArgumentNullException(nameof(edmProperty), "Contract assertion not met: edmProperty != null");
            }

            SetCollectionProperty(resource, propertyName, edmProperty.Type.AsCollection(), value, clearCollection: false);
        }

        internal static void SetCollectionProperty(object resource, string propertyName,
            IEdmCollectionTypeReference edmPropertyType, object value, bool clearCollection)
        {
            if (value != null)
            {
                IEnumerable collection = value as IEnumerable;
                if (collection == null)
                {
                    throw new ArgumentException("SetCollectionProperty is always passed the result of ODataFeedDeserializer or ODataCollectionDeserializer", "value");
                }

                Type resourceType = resource.GetType();
                Type propertyType = GetPropertyType(resource, propertyName);

                Type elementType;
                if (!propertyType.IsCollection(out elementType))
                {
                    string message = Error.Format(SRResources.PropertyIsNotCollection, propertyType.FullName, propertyName, resourceType.FullName);
                    throw new SerializationException(message);
                }

                IEnumerable newCollection;
                if (CanSetProperty(resource, propertyName) &&
                    CollectionDeserializationHelpers.TryCreateInstance(propertyType, edmPropertyType, elementType, out newCollection))
                {
                    // settable collections
                    collection.AddToCollection(newCollection, elementType, resourceType, propertyName, propertyType);
                    if (propertyType.IsArray)
                    {
                        newCollection = CollectionDeserializationHelpers.ToArray(newCollection, elementType);
                    }

                    SetProperty(resource, propertyName, newCollection);
                }
                else
                {
                    // get-only collections.
                    newCollection = GetProperty(resource, propertyName) as IEnumerable;
                    if (newCollection == null)
                    {
                        string message = Error.Format(SRResources.CannotAddToNullCollection, propertyName, resourceType.FullName);
                        throw new SerializationException(message);
                    }

                    if (clearCollection)
                    {
                        newCollection.Clear(propertyName, resourceType);
                    }

                    collection.AddToCollection(newCollection, elementType, resourceType, propertyName, propertyType);
                }
            }
        }

        internal static void SetDynamicCollectionProperty(object resource, string propertyName, object value,
            IEdmCollectionTypeReference edmPropertyType, IEdmStructuredType structuredType,
            ODataDeserializerContext readContext)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Contract assertion not met: value != null");
            }

            if (readContext == null)
            {
                throw new ArgumentNullException(nameof(readContext), "Contract assertion not met: readContext != null");
            }

            if (readContext.Model == null)
            {
                throw new ArgumentException("Contract assertion not met: readContext.Model != null", nameof(readContext));
            }

            IEnumerable collection = value as IEnumerable;
            if (collection == null)
            {
                throw new ArgumentException("Contract assertion not met: collection != null", nameof(value));
            }

            Type resourceType = resource.GetType();
            Type elementType = EdmLibHelpers.GetClrType(edmPropertyType.ElementType(), readContext.Model);
            Type propertyType = typeof(ICollection<>).MakeGenericType(elementType);
            IEnumerable newCollection;
            if (CollectionDeserializationHelpers.TryCreateInstance(propertyType, edmPropertyType, elementType,
                out newCollection))
            {
                collection.AddToCollection(newCollection, elementType, resourceType, propertyName, propertyType);
                SetDynamicProperty(resource, propertyName, newCollection, structuredType, readContext);
            }
        }

        internal static void SetProperty(object resource, string propertyName, object value)
        {
            IDelta delta = resource as IDelta;
            if (delta == null)
            {
                resource.GetType().GetProperty(propertyName).SetValue(resource, value, index: null);
            }
            else
            {
                delta.TrySetPropertyValue(propertyName, value);
            }
        }

        internal static void SetDynamicProperty(object resource, string propertyName, object value,
            IEdmStructuredType structuredType, ODataDeserializerContext readContext)
        {
            IDelta delta = resource as IDelta;
            if (delta != null)
            {
                delta.TrySetPropertyValue(propertyName, value);
            }
            else
            {
                PropertyInfo propertyInfo = EdmLibHelpers.GetDynamicPropertyDictionary(structuredType,
                    readContext.Model);
                if (propertyInfo == null)
                {
                    return;
                }

                IDictionary<string, object> dynamicPropertyDictionary;
                object dynamicDictionaryObject = propertyInfo.GetValue(resource);
                if (dynamicDictionaryObject == null)
                {
                    if (!propertyInfo.CanWrite)
                    {
                        throw Error.InvalidOperation(SRResources.CannotSetDynamicPropertyDictionary, propertyName,
                            resource.GetType().FullName);
                    }

                    dynamicPropertyDictionary = new Dictionary<string, object>();
                    propertyInfo.SetValue(resource, dynamicPropertyDictionary);
                }
                else
                {
                    dynamicPropertyDictionary = (IDictionary<string, object>)dynamicDictionaryObject;
                }

                if (dynamicPropertyDictionary.ContainsKey(propertyName))
                {
                    throw Error.InvalidOperation(SRResources.DuplicateDynamicPropertyNameFound,
                        propertyName, structuredType.FullTypeName());
                }

                dynamicPropertyDictionary.Add(propertyName, value);
            }
        }

        internal static object ConvertValue(object oDataValue, ref IEdmTypeReference propertyType, ODataDeserializerProvider deserializerProvider,
            ODataDeserializerContext readContext, out EdmTypeKind typeKind)
        {
            if (oDataValue == null)
            {
                typeKind = EdmTypeKind.None;
                return null;
            }

            ODataEnumValue enumValue = oDataValue as ODataEnumValue;
            if (enumValue != null)
            {
                typeKind = EdmTypeKind.Enum;
                return ConvertEnumValue(enumValue, ref propertyType, deserializerProvider, readContext);
            }

            ODataCollectionValue collection = oDataValue as ODataCollectionValue;
            if (collection != null)
            {
                typeKind = EdmTypeKind.Collection;
                return ConvertCollectionValue(collection, ref propertyType, deserializerProvider, readContext);
            }

            ODataUntypedValue untypedValue = oDataValue as ODataUntypedValue;
            if (untypedValue != null)
            {
                if (untypedValue.RawValue == null)
                {
                    throw new ArgumentNullException(nameof(untypedValue.RawValue), "Contract assertion not met: untypedValue.RawValue != null");
                }

                if (untypedValue.RawValue.StartsWith("[", StringComparison.Ordinal) ||
                    untypedValue.RawValue.StartsWith("{", StringComparison.Ordinal))
                {
                    throw new ODataException(Error.Format(SRResources.InvalidODataUntypedValue, untypedValue.RawValue));
                }

                oDataValue = ConvertPrimitiveValue(untypedValue.RawValue);
            }

            typeKind = EdmTypeKind.Primitive;
            return oDataValue;
        }

        internal static Type GetPropertyType(object resource, string propertyName)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource), "Contract assertion not met: resource != null");
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName), "Contract assertion not met: propertyName != null");
            }

            IDelta delta = resource as IDelta;
            if (delta != null)
            {
                Type type;
                delta.TryGetPropertyType(propertyName, out type);
                return type;
            }
            else
            {
                PropertyInfo property = resource.GetType().GetProperty(propertyName);
                return property == null ? null : property.PropertyType;
            }
        }

        private static bool CanSetProperty(object resource, string propertyName)
        {
            IDelta delta = resource as IDelta;
            if (delta != null)
            {
                return true;
            }
            else
            {
                PropertyInfo property = resource.GetType().GetProperty(propertyName);
                return property != null && property.GetSetMethod() != null;
            }
        }

        private static object GetProperty(object resource, string propertyName)
        {
            IDelta delta = resource as IDelta;
            if (delta != null)
            {
                object value;
                delta.TryGetPropertyValue(propertyName, out value);
                return value;
            }
            else
            {
                PropertyInfo property = resource.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    throw new ArgumentException("ODataLib should have already verified that the property exists on the type.", "value");
                }

                return property.GetValue(resource, index: null);
            }
        }

        private static object ConvertCollectionValue(ODataCollectionValue collection,
            ref IEdmTypeReference propertyType, ODataDeserializerProvider deserializerProvider,
            ODataDeserializerContext readContext)
        {
            IEdmCollectionTypeReference collectionType;
            if (propertyType == null)
            {
                // dynamic collection property
                if (string.IsNullOrEmpty(collection.TypeName))
                {
                    throw new ArgumentException("ODataLib should have verified that dynamic collection value has a type name " + "since we provided metadata.", nameof(collection));
                }

                string elementTypeName = GetCollectionElementTypeName(collection.TypeName, isNested: false);
                IEdmModel model = readContext.Model;
                IEdmSchemaType elementType = model.FindType(elementTypeName);
                if (elementType == null)
                {
                    throw new ArgumentException("Contract assertion not met: elementType != null", "value");
                }

                collectionType =
                    new EdmCollectionTypeReference(
                        new EdmCollectionType(elementType.ToEdmTypeReference(isNullable: false)));
                propertyType = collectionType;
            }
            else
            {
                collectionType = propertyType as IEdmCollectionTypeReference;
                if (collectionType == null)
                {
                    throw new ArgumentException("The type for collection must be a IEdmCollectionType.", "value");
                }
            }

            ODataEdmTypeDeserializer deserializer = deserializerProvider.GetEdmTypeDeserializer(collectionType);
            return deserializer.ReadInline(collection, collectionType, readContext);
        }

        private static object ConvertPrimitiveValue(string value)
        {
            double doubleValue;
            int intValue;
            decimal decimalValue;

            if (String.CompareOrdinal(value, "null") == 0)
            {
                return null;
            }

            if (Int32.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out intValue))
            {
                return intValue;
            }

            // todo: if it is Ieee754Compatible, parse decimal after double
            if (Decimal.TryParse(value, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out decimalValue))
            {
                return decimalValue;
            }

            if (Double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out doubleValue))
            {
                return doubleValue;
            }

            if (!value.StartsWith("\"", StringComparison.Ordinal) || !value.EndsWith("\"", StringComparison.Ordinal))
            {
                throw new ODataException(Error.Format(SRResources.InvalidODataUntypedValue, value));
            }

            return value.Substring(1, value.Length - 2);
        }

        private static object ConvertEnumValue(ODataEnumValue enumValue, ref IEdmTypeReference propertyType,
            ODataDeserializerProvider deserializerProvider, ODataDeserializerContext readContext)
        {
            IEdmEnumTypeReference edmEnumType;
            if (propertyType == null)
            {
                // dynamic enum property
                if (string.IsNullOrEmpty(enumValue.TypeName))
                {
                    throw new ArgumentException("ODataLib should have verified that dynamic enum value has a type name since we provided metadata.", nameof(enumValue));
                }

                IEdmModel model = readContext.Model;
                IEdmType edmType = model.FindType(enumValue.TypeName);
                if (edmType.TypeKind != EdmTypeKind.Enum)
                {
                    throw new ArgumentException("ODataLib should have verified that enum value has a enum resource type.", "value");
                }

                edmEnumType = new EdmEnumTypeReference(edmType as IEdmEnumType, isNullable: true);
                propertyType = edmEnumType;
            }
            else
            {
                edmEnumType = propertyType.AsEnum();
            }

            ODataEdmTypeDeserializer deserializer = deserializerProvider.GetEdmTypeDeserializer(edmEnumType);
            return deserializer.ReadInline(enumValue, propertyType, readContext);
        }

        // The same logic from ODL to get the element type name in a collection.
        private static string GetCollectionElementTypeName(string typeName, bool isNested)
        {
            const string CollectionTypeQualifier = "Collection";
            int collectionTypeQualifierLength = CollectionTypeQualifier.Length;

            // A collection type name must not be null, it has to start with "Collection(" and end with ")"
            // and must not be "Collection()"
            if (typeName != null &&
                typeName.StartsWith(CollectionTypeQualifier + "(", StringComparison.Ordinal) &&
                typeName[typeName.Length - 1] == ')' &&
                typeName.Length != collectionTypeQualifierLength + 2)
            {
                if (isNested)
                {
                    throw new ODataException(Error.Format(SRResources.NestedCollectionsNotSupported, typeName));
                }

                string innerTypeName = typeName.Substring(collectionTypeQualifierLength + 1,
                    typeName.Length - (collectionTypeQualifierLength + 2));

                // Check if it is not a nested collection and throw if it is
                GetCollectionElementTypeName(innerTypeName, true);

                return innerTypeName;
            }

            return null;
        }
    }
}
