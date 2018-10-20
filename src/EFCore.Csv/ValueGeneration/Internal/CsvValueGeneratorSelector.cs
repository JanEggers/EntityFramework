// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Csv.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Microsoft.EntityFrameworkCore.Csv.ValueGeneration.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class CsvValueGeneratorSelector : ValueGeneratorSelector
    {
        private readonly CsvIntegerValueGeneratorFactory _factory;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public CsvValueGeneratorSelector([NotNull] ICsvStoreCache storeCache, [NotNull] IDbContextOptions contextOptions, [NotNull] ValueGeneratorSelectorDependencies dependencies)
            : base(dependencies)
        {
            var store = storeCache.GetStore(contextOptions);
            _factory = new CsvIntegerValueGeneratorFactory(store);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(entityType, nameof(entityType));

            return property.ClrType.IsInteger()
                ? _factory.Create(property)
                : base.Create(property, entityType);
        }
    }
}
