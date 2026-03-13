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
            "데이터 분석 시작 (TeruTeruPandas)",
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
                    Console.WriteLine("\n[데이터 분석 시작] 기능은 아직 준비 중입니다.");
                    Console.WriteLine("아무 키나 누르면 메뉴로 돌아갑니다...");
                    Console.ReadKey(true);
                    break;
                case 1:
                    Console.WriteLine("\n[환경 설정] 기능은 아직 준비 중입니다.");
                    Console.WriteLine("아무 키나 누르면 메뉴로 돌아갑니다...");
                    Console.ReadKey(true);
                    break;
                case 2:
                case -1: // ESC
                    running = false;
                    Console.WriteLine("\n프로그램을 종료합니다.");
                    break;
            }
        }
    }
}
