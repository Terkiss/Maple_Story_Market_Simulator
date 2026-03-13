using System;
using System.Collections.Generic;
using System.IO;
using TeruTeruPandas.Core;
using TeruTeruPandas.Compat;
using TeruTeruPandas.IO;

namespace MapleMarketS;

public class MultiTimeframeAnalyzer
{
    private readonly DataUniverse _universe;

    public MultiTimeframeAnalyzer()
    {
        _universe = new DataUniverse();
    }

    public DataFrame RunAnalysis(string directoryPath)
    {
        Console.WriteLine($"[분석 단계 1] '{directoryPath}' 디렉토리 데이터 로드 중...");
        
        // 1. 디렉토리 내 모든 CSV 파일 로드 및 전처리
        string[] files = Directory.GetFiles(directoryPath, "*.csv");
        foreach (var file in files)
        {
            string tableName = Path.GetFileNameWithoutExtension(file);
            
            // 데이터 로드
            DataFrame df = Pd.ReadCsv(file);
            
            // 로드 직후 ffill을 통한 결측치 처리
            df = df.FillNA("ffill");
            
            // 데이터 정보 출력
            Console.WriteLine($"\n--- 테이블: {tableName} 정보 ---");
            df.Info();
            Console.WriteLine(df.Describe().ToString());
            
            // 거시 추세 판단 컬럼 추가 (100억Close > 100억MA20)
            df = AddMacroTrendColumn(df);
            
            // DataUniverse에 등록
            _universe.AddTable(tableName, df);
        }

        Console.WriteLine("\n[분석 단계 2] 다중 시간대 교차 분석 시작...");

        // 2. 시간대별 테이블 가져오기
        DataFrame df1h = _universe.GetTable("1h") ?? throw new Exception("'1h' 테이블이 없습니다.");
        DataFrame df5m = _universe.GetTable("5m") ?? throw new Exception("'5m' 테이블이 없습니다.");

        // 3. 5분봉 데이터에서 1억Close의 PctChange 계산
        DataFrame df5m_pct = df5m.PctChange(1);
        df5m.AddColumn("1억PctChange", df5m_pct["1억Close"]);

        // '5분' 데이터에서 매수 유입 포착 (0.5% 이상)
        bool[] buyInflow = new bool[df5m.RowCount];
        for (int i = 0; i < df5m.RowCount; i++)
        {
            var pct = df5m["1억PctChange"].GetValue(i);
            buyInflow[i] = pct != null && !pct.Equals(DBNull.Value) && Convert.ToDouble(pct) >= 0.005;
        }
        df5m.AddColumn("IsBuyInflow", new TeruTeruPandas.Core.Column.PrimitiveColumn<bool>(buyInflow));

        // 업데이트된 테이블 다시 등록
        _universe.UpdateTable("5m", df5m);

        // 4. 통합 분석: '1시간'이 상승장일 때, '5분'에서 매수 유입 발생 지점 추출
        // 단순화를 위해 5분봉에도 1시간 봉의 추세 데이터가 포함되어 있다고 가정하거나 
        // (예제 데이터는 그렇게 생성함), 시간 기반 JOIN을 시도합니다.
        
        string sql = @"
            SELECT Time, 1억Close, 1억PctChange 
            FROM 5m 
            WHERE IsUptrend == True AND IsBuyInflow == True";

        Console.WriteLine("\n[분석 단계 3] 최종 매수 시그널 추출 중...");
        
        DataFrame finalSignals;
        try 
        {
            finalSignals = _universe.SqlExecute(sql);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SQL 경고] {ex.Message}. DataFrame 필터링으로 대체합니다.");
            
            // SQL 실패 시 수동 필터링 (Uptrend && BuyInflow)
            var mask = new bool[df5m.RowCount];
            for (int i = 0; i < df5m.RowCount; i++)
            {
                bool isUptrend = (bool)df5m["IsUptrend"].GetValue(i)!;
                bool isBuyInflow = (bool)df5m["IsBuyInflow"].GetValue(i)!;
                mask[i] = isUptrend && isBuyInflow;
            }
            finalSignals = df5m[new BoolSeries(mask)];
        }

        return finalSignals;
    }

    /// <summary>
    /// 거시 추세를 판단하여 IsUptrend 컬럼을 추가합니다. (100억Close > 100억MA20)
    /// </summary>
    private DataFrame AddMacroTrendColumn(DataFrame df)
    {
        if (!df.Columns.Contains("100억Close") || !df.Columns.Contains("100억MA20"))
        {
            // 수치 데이터가 없는 테이블은 기본적으로 False 처리
            bool[] falseMask = new bool[df.RowCount];
            df.AddColumn("IsUptrend", new TeruTeruPandas.Core.Column.PrimitiveColumn<bool>(falseMask));
            return df;
        }

        var close = df["100억Close"];
        var ma = df["100억MA20"];
        bool[] trend = new bool[df.RowCount];

        for (int i = 0; i < df.RowCount; i++)
        {
            var cVal = close.GetValue(i);
            var mVal = ma.GetValue(i);
            if (cVal != null && mVal != null)
            {
                trend[i] = Convert.ToDouble(cVal) > Convert.ToDouble(mVal);
            }
        }

        df.AddColumn("IsUptrend", new TeruTeruPandas.Core.Column.PrimitiveColumn<bool>(trend));
        return df;
    }
}
