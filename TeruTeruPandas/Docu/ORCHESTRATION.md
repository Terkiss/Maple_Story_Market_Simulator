# TeruTeruPandas: 오케스트레이션 및 개발 컨텍스트

이 문서는 AI 기반 오케스트레이션 및 개발을 위해 `TeruTeruPandas` 라이브러리의 종합적인 기술 개요를 제공합니다.

## 1. 라이브러리 철학
`TeruTeruPandas`는 Python의 `pandas`에서 영감을 받은 가볍고 고성능인 C# DataFrame 라이브러리입니다. `ArrayPool` 기반의 열 단위(Columnar) 저장 방식을 사용하여 캐시 지역성을 극대화하고 가비지 컬렉터(GC) 부하를 최소화하며, 가능한 한 제로 할당(Zero-allocation)을 지향하도록 설계되었습니다.

## 2. 핵심 아키텍처
- **네임스페이스:** `TeruTeruPandas.Core`
- **주요 객체:**
    - `DataFrame`: 서로 다른 타입의 컬럼들을 모은 2차원 라벨링 데이터 구조.
    - `Series<T>`: 모든 데이터 타입을 보유할 수 있는 1차원 라벨링 배열.
    - `BoolSeries`: 불리언 마스킹 및 필터링을 위한 특화된 시리즈.
    - `Index`: `RangeIndex`, `IntIndex`, `StringIndex`, `DateTimeIndex`, `MultiIndex`를 지원하는 라벨링 시스템.

### 주요 DataFrame 기능
- **접근자(Accessors):** 
    - `df.Loc[rowKey, col]`: 라벨 기반 인덱싱.
    - `df.ILoc[rowIdx, colIdx]`: 정수 위치 기반 인덱싱.
    - `df.At(rowKey, col)` / `df.Iat(rowIdx, colIdx)`: 최적화된 스칼라 값 접근.
- **조작(Manipulation):** `Head()`, `Tail()`, `DropNA()`, `FillNA()`, `SortValues()`, `SortIndex()`.
- **분석(Analysis):** `Info()`, `Describe()`, `Std()`, `Var()`, `Median()`, `Quantile()`.
- **변환(Transformation):** `SimplePivot()`, `SimpleMelt()`, `Merge()`, `Concat()`.

## 3. 입출력(IO) 기능
- **네임스페이스:** `TeruTeruPandas.IO`
- **CSV:** `CsvReader.ReadCsv`, `CsvWriter.ToCsv`.
- **JSON:** `JsonIO.ReadJson`, `JsonIO.ToJson` (JsonLines 지원).
- **SQLite:** `SqliteIO.ReadSqlite`, `SqliteIO.ToSqlite`, `SqliteIO.ReadSqliteTable`.

## 4. 데이터유니버스(DataUniverse) 및 SQL 엔진
`DataUniverse` 클래스는 여러 DataFrame을 관계형 시스템처럼 관리할 수 있게 해줍니다.
- **SQL 파서:** `SELECT`, `FROM`, `JOIN` (INNER/LEFT/RIGHT), `WHERE`, `GROUP BY`, `ORDER BY`, `LIMIT` 지원.
- **사용법:** `universe.SqlExecute("SELECT * FROM TableA JOIN TableB ON ...")`.

## 5. AI를 위한 구현 참조 사항
이 라이브러리를 위한 코드를 생성할 때:
1.  **Pd 헬퍼 권장:** pandas 스타일의 객체 생성을 위해 `TeruTeruPandas.Compat.Pd`를 사용하세요.
2.  **명시적 타입 정의:** 라이브러리가 타입을 추론하지만, `Dictionary<string, IColumn>`에서 타입을 명시하는 것이 모호함을 방지합니다.
3.  **메모리 관리:** `DataFrame`은 `IDisposable`을 구현합니다. 대규모 데이터셋 처리 시 `using` 블록을 사용하여 버퍼를 `ArrayPool`에 반환하도록 하세요.
4.  **예외 처리:** 존재하지 않는 키 인덱싱 시 `KeyNotFoundException`, 범위를 벗어난 위치 접근 시 `IndexOutOfRangeException`이 발생합니다.

## 6. 프로젝트 통합 정보 (MapleMarketS)
- `TeruTeruPandas`는 **프로젝트 참조(ProjectReference)** 방식으로 통합되어 있습니다.
- 메인 진입점: `MapleMarketS/Program.cs`
- 유틸리티 UI: `MapleMarketS/Utils/CommandPalette.cs`

---
*최종 업데이트: 2026-03-14*
