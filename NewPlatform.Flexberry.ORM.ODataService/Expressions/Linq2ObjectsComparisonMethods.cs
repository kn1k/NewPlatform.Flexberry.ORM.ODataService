﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.
// Branch of https://github.com/OData/WebApi/blob/v5.7.0/OData/src/System.Web.OData/OData/Query/Expressions/Linq2ObjectsComparisonMethods.cs

namespace NewPlatform.Flexberry.ORM.ODataService.Expressions
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Linq2Objects comparison methods.
    /// </summary>
    internal static class Linq2ObjectsComparisonMethods
    {
        /// <summary>Method info for byte array comparison.</summary>
        public static readonly MethodInfo AreByteArraysEqualMethodInfo =
            typeof(Linq2ObjectsComparisonMethods).GetMethod("AreByteArraysEqual");

        /// <summary>Method info for byte array comparison.</summary>
        public static readonly MethodInfo AreByteArraysNotEqualMethodInfo =
            typeof(Linq2ObjectsComparisonMethods).GetMethod("AreByteArraysNotEqual");

        /// <summary>Compares two byte arrays for equality.</summary>
        /// <param name="left">First byte array.</param>
        /// <param name="right">Second byte array.</param>
        /// <returns>true if the arrays are equal; false otherwise.</returns>
        public static bool AreByteArraysEqual(byte[] left, byte[] right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            if (left.Length != right.Length)
            {
                return false;
            }

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Compares two byte arrays for equality.</summary>
        /// <param name="left">First byte array.</param>
        /// <param name="right">Second byte array.</param>
        /// <returns>true if the arrays are not equal; false otherwise.</returns>
        public static bool AreByteArraysNotEqual(byte[] left, byte[] right)
        {
            return !AreByteArraysEqual(left, right);
        }
    }
}
