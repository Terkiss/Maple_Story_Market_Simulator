using System;
using System.Collections.Generic;
using MapleMarketS.Utils;

namespace MapleMarketS;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.WriteLine("MapleMarketS 프로젝트에 오신 것을 환영합니다.");
        
        List<string> mainMenuOptions = new List<string>
        {
            "단일 데이터 분석 (MarketDataAnalyzer)",
            "다중 시간대 교차 분석 (MultiTimeframeAnalyzer)",
            "환경 설정",
            "종료"
        };

        bool running = true;
        while (running)
        {
            int selection = CommandPalette.ShowMenu("메인 메뉴", mainMenuOptions);

            switch (selection)
            {
                case 0:
                    try
                    {
                        var analyzer = new MarketDataAnalyzer();
                        analyzer.Analyze("market_data.csv");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[오류 발생] {ex.Message}");
                    }
                    Console.WriteLine("\n아무 키나 누르면 메뉴로 돌아갑니다...");
                    Console.ReadKey(true);
                    break;
                case 1:
                    try
                    {
                        var mtAnalyzer = new MultiTimeframeAnalyzer();
                        var signals = mtAnalyzer.RunAnalysis("data");
                        
                        Console.WriteLine("\n=== 최종 통합 매수 시그널 목록 ===");
                        Console.WriteLine(signals.ToString());
                        Console.WriteLine($"총 발견된 시그널 수: {signals.RowCount}개");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[오류 발생] {ex.Message}");
                    }
                    Console.WriteLine("\n아무 키나 누르면 메뉴로 돌아갑니다...");
                    Console.ReadKey(true);
                    break;
                case 2:
                    Console.WriteLine("\n[환경 설정] 기능은 아직 준비 중입니다.");
                    Console.WriteLine("아무 키나 누르면 메뉴로 돌아갑니다...");
                    Console.ReadKey(true);
                    break;
                case 3:
                case -1: // ESC
                    running = false;
                    Console.WriteLine("\n프로그램을 종료합니다.");
                    break;
            }
        }
    }
}
