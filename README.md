# MapleMarketS

MapleMarketS는 메이플스토리 시장 데이터를 분석하고 시뮬레이션하기 위한 .NET 9.0 기반 콘솔 애플리케이션입니다.

## 주요 기능

- **메이플스토리 시장 데이터 분석**: 다양한 시간대(5분, 10분, 30분, 1시간 등)의 시장 데이터를 분석합니다.
- **TeruTeruPandas 통합**: C# 환경에서 데이터프레임과 시리즈를 다룰 수 있는 커스텀 데이터 분석 라이브러리를 포함하고 있습니다.
- **인터랙티브 콘솔 UI**: 화살표 키와 엔터를 사용하는 편리한 메뉴 시스템(`CommandPalette`)을 제공합니다.

## 프로젝트 구조

- `MapleMarketS`: 메인 콘솔 애플리케이션 프로젝트
  - `Program.cs`: 메인 진입점 및 메뉴 루프
  - `Utils/CommandPalette.cs`: 커스텀 콘솔 메뉴 라이브러리
  - `data/`: 분석에 사용되는 CSV 데이터 파일들
- `TeruTeruPandas`: 데이터 분석을 위한 핵심 라이브러리
  - `Core/`: DataFrame, Series 등 핵심 자료구조 및 논리
  - `IO/`: 데이터 읽기/쓰기 기능

## 시작하기

### 필수 조건

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### 실행 방법

1. 저장소를 클론합니다:
   ```bash
   git clone https://github.com/Terkiss/Maple_Story_Market_Simulator.git
   cd Maple_Story_Market_Simulator
   ```

2. 프로젝트를 실행합니다:
   ```bash
   dotnet run --project MapleMarketS/MapleMarketS.csproj
   ```

## 제외 폴더 (.gitignore)

다음 폴더들은 빌드 산출물 및 대용량 데이터 관리를 위해 추적에서 제외됩니다:
- `bin/`
- `obj/`
- `data/`
