// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Microsoft.EntityFrameworkCore.Csv.ValueGeneration.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class CsvIntegerValueGeneratorFactory : ValueGeneratorFactory
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ValueGenerator Create(IProperty property)
        {
            Check.NotNull(property, nameof(property));

            var type = property.ClrType.UnwrapNullableType().UnwrapEnumType();

            if (type == typeof(long))
            {
                return new CsvIntegerValueGenerator<long>();
            }

            if (type == typeof(int))
            {
                return new CsvIntegerValueGenerator<int>();
            }

            if (type == typeof(short))
            {
                return new CsvIntegerValueGenerator<short>();
            }

            if (type == typeof(byte))
            {
                return new CsvIntegerValueGenerator<byte>();
            }

            if (type == typeof(ulong))
            {
                return new CsvIntegerValueGenerator<ulong>();
            }

            if (type == typeof(uint))
            {
                return new CsvIntegerValueGenerator<uint>();
            }

            if (type == typeof(ushort))
            {
                return new CsvIntegerValueGenerator<ushort>();
            }

            if (type == typeof(sbyte))
            {
                return new CsvIntegerValueGenerator<sbyte>();
            }

            throw new ArgumentException(
                CoreStrings.InvalidValueGeneratorFactoryProperty(
                    nameof(CsvIntegerValueGeneratorFactory), property.Name, property.DeclaringEntityType.DisplayName()));
        }
    }
}
