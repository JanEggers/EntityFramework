﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Csv.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class CsvTypeMappingSource : TypeMappingSource
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public CsvTypeMappingSource([NotNull] TypeMappingSourceDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override CoreTypeMapping FindMapping(in TypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            Debug.Assert(clrType != null);

            if (clrType.IsValueType
                || clrType == typeof(string))
            {
                return new CsvTypeMapping(clrType);
            }

            if (clrType == typeof(byte[]))
            {
                return new CsvTypeMapping(clrType, deepComparer: new ArrayDeepComparer<byte>());
            }

            if (clrType.FullName == "GeoAPI.Geometries.IGeometry"
                || clrType.GetInterface("GeoAPI.Geometries.IGeometry") != null)
            {
                var comparer = (ValueComparer)Activator.CreateInstance(typeof(GeometryValueComparer<>).MakeGenericType(clrType));

                return new CsvTypeMapping(
                    clrType,
                    comparer,
                    comparer,
                    comparer);
            }

            return base.FindMapping(mappingInfo);
        }
    }
}
