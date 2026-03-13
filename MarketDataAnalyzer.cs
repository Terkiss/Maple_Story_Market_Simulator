using System;
using System.Collections.Generic;
using TeruTeruPandas.Core;
using TeruTeruPandas.Compat;

namespace MapleMarketS;

public class MarketDataAnalyzer
{
    public void Analyze(string csvPath)
    {
        Console.WriteLine($"[분석 시작] 파일 로드 중: {csvPath}");

        // 1. 데이터 로드 및 결측치 처리 (ffill)
        DataFrame df = Pd.ReadCsv(csvPath);
        df = df.FillNA("ffill");

        Console.WriteLine("데이터 로드 및 ffill 완료.");

        // 2. 100억 단위 분석: '100억Close' > '100억MA20' 구간 식별
        var close100B = df["100억Close"];
        var ma20_100B = df["100억MA20"];
        
        // 직접 비교 컬럼 생성 (마스크 용도)
        bool[] maskArray = new bool[df.RowCount];
        for (int i = 0; i < df.RowCount; i++)
        {
            var closeVal = Convert.ToDouble(close100B.GetValue(i));
            var maVal = Convert.ToDouble(ma20_100B.GetValue(i));
            maskArray[i] = closeVal > maVal;
        }
        
        // 'IsUptrend' 컬럼 추가
        df.AddColumn("IsUptrend", new TeruTeruPandas.Core.Column.PrimitiveColumn<bool>(maskArray));

        // 3. 1억 단위 분석: '1억Close'의 전봉 대비 변화율(PctChange) 계산
        var pctChangeDf = df.PctChange(1);
        var close1B_Pct = pctChangeDf["1억Close"];
        
        // '매수 유입' 판별 (변화율 0.5% 이상)
        bool[] buyInflowArray = new bool[df.RowCount];
        for (int i = 0; i < df.RowCount; i++)
        {
            var pct = close1B_Pct.GetValue(i);
            if (pct != null && !pct.Equals(DBNull.Value))
            {
                buyInflowArray[i] = Convert.ToDouble(pct) >= 0.005;
            }
            else
            {
                buyInflowArray[i] = false;
            }
        }
        df.AddColumn("IsBuyInflow", new TeruTeruPandas.Core.Column.PrimitiveColumn<bool>(buyInflowArray));

        // 4. DataUniverse를 활용한 최종 리포트 생성
        DataUniverse universe = new DataUniverse();
        universe.AddTable("MarketData", df);

        // SQL 쿼리 시도
        DataFrame report;
        try 
        {
            // SimpleSqlParser가 '==' 및 'True' 리터럴을 지원하는지 확인하며 쿼리
            string sql = "SELECT * FROM MarketData WHERE IsUptrend == True AND IsBuyInflow == True";
            report = universe.SqlExecute(sql);
        }
        catch
        {
            // SQL 실패 시 직접 필터링
            var finalMask = new bool[df.RowCount];
            for(int i=0; i<df.RowCount; i++) {
                finalMask[i] = (bool)df["IsUptrend"].GetValue(i)! && (bool)df["IsBuyInflow"].GetValue(i)!;
            }
            var boolSeries = new BoolSeries(finalMask);
            report = df[boolSeries];
        }

        // 5. 결과 출력
        Console.WriteLine("\n=== Top-Down Tiered Analysis Report ===");
        Console.WriteLine(report.ToString());
        
        Console.WriteLine("\n[분석 결과 통계]");
        report.Info();
        
        Console.WriteLine("\n[수치 데이터 요약]");
        Console.WriteLine(report.Describe().ToString());
        
        Console.WriteLine($"총 분석 데이터 수: {df.RowCount}행");
        Console.WriteLine($"조건 충족 데이터 수: {report.RowCount}행");
    }
}
