// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update;

namespace Microsoft.EntityFrameworkCore.Csv.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class CsvStore : ICsvStore
    {
        private readonly ICsvTableFactory _tableFactory;
        private readonly object _lock = new object();

        private readonly Dictionary<object, ICsvTable> _tables = new Dictionary<object, ICsvTable>();
        
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public CsvStore(
            [NotNull] ICsvTableFactory tableFactory)
        {
            _tableFactory = tableFactory;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool EnsureCreated(
            StateManagerDependencies stateManagerDependencies,
            IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
        {
            lock (_lock)
            {
                var stateManager = new StateManager(stateManagerDependencies);
                var entries = new List<IUpdateEntry>();
                foreach (var entityType in stateManagerDependencies.Model.GetEntityTypes())
                {
                    foreach (var targetSeed in entityType.GetData())
                    {
                        var entry = stateManager.CreateEntry(targetSeed, entityType);
                        entry.SetEntityState(EntityState.Added);
                        entries.Add(entry);
                    }
                }

                ExecuteTransaction(entries, updateLogger);

                return true;
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool Clear()
        {
            lock (_lock)
            {
                if (_tables.Keys.Count == 0)
                {
                    return false;
                }

                _tables.Clear();
                return true;
            }
        }
        
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<CsvTableSnapshot> GetTableSnapshots(IEntityType entityType)
        {
            var data = new List<CsvTableSnapshot>();
            lock (_lock)
            {
                foreach (var et in entityType.GetConcreteTypesInHierarchy())
                {
                    data.Add(new CsvTableSnapshot(et, GetTable(et).SnapshotRows()));
                }
            }

            return data;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual int ExecuteTransaction(
            IReadOnlyList<IUpdateEntry> entries,
            IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
        {
            var rowsAffected = 0;

            lock (_lock)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    var entityType = entry.EntityType;

                    Debug.Assert(!entityType.IsAbstract());

                    var table = GetTable(entityType);

                    if (entry.SharedIdentityEntry != null)
                    {
                        if (entry.EntityState == EntityState.Deleted)
                        {
                            continue;
                        }

                        table.Delete(entry);
                    }

                    switch (entry.EntityState)
                    {
                        case EntityState.Added:
                            table.Create(entry);
                            break;
                        case EntityState.Deleted:
                            table.Delete(entry);
                            break;
                        case EntityState.Modified:
                            table.Update(entry);
                            break;
                    }

                    rowsAffected++;
                }
            }

            updateLogger.ChangesSaved(entries, rowsAffected);

            return rowsAffected;
        }

        /// <inheritdoc/>
        public ICsvTable GetTable(IEntityType entityType)
        {
            var key = entityType.ToString();
            if (!_tables.TryGetValue(key, out var table))
            {
                _tables.Add(key, table = _tableFactory.Create(entityType));
            }

            return table;
        }
    }
}
