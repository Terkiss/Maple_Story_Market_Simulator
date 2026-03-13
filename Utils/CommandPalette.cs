using System;
using System.Collections.Generic;

namespace MapleMarketS.Utils;

public static class CommandPalette
{
    public static int ShowMenu(string title, List<string> options)
    {
        int selectedIndex = 0;
        ConsoleKey key;

        // 메뉴 높이 계산 (제목 1줄 + 옵션들)
        int menuHeight = options.Count + 1;

        // 공간 미리 확보: 화면 하단에서 스크롤이 미리 발생하게 하여 startTop이 밀리지 않게 함
        for (int i = 0; i < menuHeight; i++) Console.WriteLine();

        // 확보된 위치의 시작점 계산
        int startTop = Math.Max(0, Console.CursorTop - menuHeight);
        Console.CursorVisible = false;

        do
        {
            for (int i = 0; i < menuHeight; i++)
            {
                int targetTop = startTop + i;
                if (targetTop >= 0 && targetTop < Console.BufferHeight)
                {
                    Console.SetCursorPosition(0, targetTop);
                }
                string line = "";

                if (i == 0) // 제목 줄
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    line = $"[ {title} ]".PadRight(Console.WindowWidth - 1);
                    Console.Write(line);
                    Console.ResetColor();
                }
                else // 옵션 줄
                {
                    int optIdx = i - 1;
                    line = (optIdx == selectedIndex) ? $" > {options[optIdx]} " : $"   {options[optIdx]} ";
                    line = line.PadRight(Console.WindowWidth - 1);

                    if (optIdx == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(line);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(line);
                    }
                }
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? options.Count - 1 : selectedIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == options.Count - 1) ? 0 : selectedIndex + 1;
            }
            else if (key == ConsoleKey.Escape)
            {
                // ESC 선택 시 메뉴 영역을 지우고 복구
                for (int i = 0; i < menuHeight; i++)
                {
                    Console.SetCursorPosition(0, startTop + i);
                    Console.Write("".PadRight(Console.WindowWidth - 1));
                }
                Console.SetCursorPosition(0, startTop);
                Console.CursorVisible = true;
                return -1;
            }

        } while (key != ConsoleKey.Enter);

        Console.CursorVisible = true;
        // 메뉴 영역 바로 아래로 커서 이동
        Console.SetCursorPosition(0, startTop + menuHeight);
        return selectedIndex;
    }
}
