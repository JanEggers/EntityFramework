// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Csv.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore.Csv.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class CsvStoreCache : ICsvStoreCache
    {
        private readonly ICsvTableFactory _tableFactory;
        private readonly bool _useNameMatching;
        private readonly ConcurrentDictionary<string, ICsvStore> _namedStores;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Obsolete("Use the constructor that also accepts options.")]
        public CsvStoreCache([NotNull] ICsvTableFactory tableFactory)
            : this(tableFactory, null)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public CsvStoreCache(
            [NotNull] ICsvTableFactory tableFactory,
            [CanBeNull] ICsvSingletonOptions options)
        {
            _tableFactory = tableFactory;

            if (options?.DatabaseRoot != null)
            {
                _useNameMatching = true;

                LazyInitializer.EnsureInitialized(
                    ref options.DatabaseRoot.Instance,
                    () => new ConcurrentDictionary<string, ICsvStore>());

                _namedStores = (ConcurrentDictionary<string, ICsvStore>)options.DatabaseRoot.Instance;
            }
            else
            {
                _namedStores = new ConcurrentDictionary<string, ICsvStore>();
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ICsvStore GetStore(string name)
            => _namedStores.GetOrAdd(name, _ => new CsvStore(_tableFactory, _useNameMatching));
    }
}
