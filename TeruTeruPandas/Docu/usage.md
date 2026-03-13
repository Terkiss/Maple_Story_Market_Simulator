# TeruTeruPandas User Guide

## Overview
TeruTeruPandas is a lightweight C# DataFrame/Series library inspired by pandas.
It provides Core (DataFrame, Series, Index), IO (CSV/JSON/SQLite), and Compat (pandas-like helpers).

## Quick Start
```csharp
using TeruTeruPandas.Core;
using TeruTeruPandas.Compat;

var df = Pd.DataFrame(new Dictionary<string, object[]>
{
    ["id"] = new object[] { 1, 2, 3 },
    ["name"] = new object[] { "A", "B", "C" },
    ["score"] = new object[] { 10, 20, 30 }
});

Console.WriteLine(df.Head(2));
```

## Namespaces
- `TeruTeruPandas.Core`: DataFrame, Series, Index, Join/Pivot, GroupBy
- `TeruTeruPandas.Compat`: pandas-style extension methods and `Pd` helper
- `TeruTeruPandas.IO`: CSV/JSON/SQLite IO

## Core Objects
### DataFrame
- Create: `new DataFrame(Dictionary<string, IColumn> columns, Index? index = null)`
- Properties: `RowCount`, `ColumnCount`, `Columns`, `Index`, `Values`, `Dtypes`, `Size`, `Empty`
- Indexers:
  - `df["col"]`: column access
  - `df[row, "col"]`: row/column access (position-based)
  - `df[rowKey, "col"]`: label-based access
  - `df[mask]`: BoolSeries filtering
- Common methods: `Head`, `Tail`, `DropNA`, `FillNA`, `SortValues`, `SortIndex`, `Info`, `Describe`, `IsNa`, `NotNa`
- Stats: `Std`, `Var`, `Median`, `Min`, `Max`, `Quantile`
- Cumulative/change: `Cumsum`, `Cumprod`, `Cummax`, `Cummin`, `Diff`, `PctChange`

### Series<T> / BoolSeries
- `Series<T>`: 1D vector with NA handling (`IsNA`, `SetNA`)
- `BoolSeries`: boolean mask with `&`, `|`, `!`

### Index
- `RangeIndex`, `IntIndex`, `StringIndex`, `DateTimeIndex`, `MultiIndex`
- DataFrame/Series supports position and label-based access via Index.

## Indexing
- Position: `df.ILoc[row, col]` or `df.Iat(row, col)`
- Label: `df.Loc[rowKey, col]` or `df.At(rowKey, col)`
- Mask: `df[BoolSeries]`

## NA Handling
- Columns keep NA masks.
- `DropNA(how="any|all", thresh=null)` removes rows with NA values.
- `FillNA(value)` or `FillNA(method="ffill|bfill")` fills NA values.

## Grouping and Aggregation
- `df.GroupBy("col").Agg(new Dictionary<string, string[]>{ ... })`
- Supported: `sum`, `mean`, `count`, `max`, `min`, `std`, `var`

## Join and Concat
- `df.Merge(other, on: "key", how: "inner|left|right|outer", strategy: JoinStrategy.Auto|Hash|Index|NestedLoop)`
- `DataFrameJoinExtensions.Concat(dataframes, axis: 0|1)`
- Duplicate column names may get `_right` suffix.

## Pivot and Melt
- `df.SimplePivot(indexCol, columnCol, valueCol)`
- `df.SimpleMelt(idVars, valueVars, varName, valueName)`

## IO
### CSV
- Read: `CsvReader.ReadCsv(path, hasHeader: true, separator: ',')`
- Write: `CsvWriter.ToCsv(df, path, includeHeader: true)`

### JSON
- Read: `JsonIO.ReadJson(path, isJsonLines: false)`
- Write: `JsonIO.ToJson(df, path, pretty: false, asJsonLines: false)`

### SQLite
- Read: `SqliteIO.ReadSqlite(connectionString, query)`
- Read specific table: `SqliteIO.ReadSqliteTable(dbPath, tableName)`
- Write: `SqliteIO.ToSqlite(df, connectionString, tableName, ifExists: false)`
- Table list: `SqliteIO.GetTableNames(dbPath)`

## DataUniverse
- Manage multiple DataFrames with `AddTable`, `UpdateTable`, `GetTable`, `RemoveTable`
- Query with `Join`, `ConcatTables`, `SqlExecute`
- Persist to JSON, directory (CSV), or SQLite

## SQL
- `universe.SqlExecute("SELECT ... FROM ... WHERE ... GROUP BY ... ORDER BY ... LIMIT ...")`
- Supported: `SELECT`, `WHERE`, `JOIN` (INNER/LEFT/RIGHT), `GROUP BY`, `ORDER BY` (ASC/DESC), `LIMIT`
- Note: `ORDER BY` performs actual sorting.

## Limitations
- Type inference is sample-based and favors string/number/bool.
- `Query` only accepts simple expressions like `"col > 5"`.
- `SimplePivot`/`SimpleMelt` are simple implementations and can be slow on large data.
- NA semantics may differ from pandas in edge cases.
