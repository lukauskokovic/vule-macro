using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class WindowsAPI
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern void keybd_event(int bVk, byte bScan, int dwFlags, int dwExtraInfo);
    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, uint dwExtraInfo);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCursorPos(int x, int y);

    public static Dictionary<string, int> KeyMap = new Dictionary<string, int>();

    public static void InitKeyMap()
    {
        #region Numbers and alphabet
        KeyMap.Add("0", 48);
        KeyMap.Add("1", 49);
        KeyMap.Add("2", 50);
        KeyMap.Add("3", 51);
        KeyMap.Add("4", 52);
        KeyMap.Add("5", 53);
        KeyMap.Add("6", 54);
        KeyMap.Add("7", 55);
        KeyMap.Add("8", 56);
        KeyMap.Add("9", 57);

        KeyMap.Add("A", 65);
        KeyMap.Add("B", 66);
        KeyMap.Add("C", 67);
        KeyMap.Add("D", 68);
        KeyMap.Add("E", 69);
        KeyMap.Add("F", 70);
        KeyMap.Add("G", 71);
        KeyMap.Add("H", 72);
        KeyMap.Add("I", 73);
        KeyMap.Add("J", 74);
        KeyMap.Add("K", 75);
        KeyMap.Add("L", 76);
        KeyMap.Add("M", 77);
        KeyMap.Add("N", 78);
        KeyMap.Add("O", 79);
        KeyMap.Add("P", 80);
        KeyMap.Add("Q", 81);
        KeyMap.Add("R", 82);
        KeyMap.Add("S", 83);
        KeyMap.Add("T", 84);
        KeyMap.Add("U", 85);
        KeyMap.Add("V", 86);
        KeyMap.Add("W", 87);
        KeyMap.Add("X", 88);
        KeyMap.Add("Y", 89);
        KeyMap.Add("Z", 90);
        #endregion
        #region F1-F124
        KeyMap.Add("F1", 0x70);
        KeyMap.Add("F2", 0x71);
        KeyMap.Add("F3", 0x72);
        KeyMap.Add("F4", 0x73);
        KeyMap.Add("F5", 0x74);
        KeyMap.Add("F6", 0x75);
        KeyMap.Add("F7", 0x76);
        KeyMap.Add("F8", 0x77);
        KeyMap.Add("F9", 0x78);
        KeyMap.Add("F10", 0x79);
        KeyMap.Add("F11", 0x7A);
        KeyMap.Add("F12", 0x7B);
        KeyMap.Add("F13", 0x7C);
        KeyMap.Add("F14", 0x7D);
        KeyMap.Add("F15", 0x7E);
        KeyMap.Add("F16", 0x7F);
        KeyMap.Add("F17", 0x80);
        KeyMap.Add("F18", 0x81);
        KeyMap.Add("F19", 0x82);
        KeyMap.Add("F20", 0x83);
        KeyMap.Add("F21", 0x84);
        KeyMap.Add("F22", 0x85);
        KeyMap.Add("F23", 0x86);
        KeyMap.Add("F24", 0x87);
        #endregion
        #region NUMPAD
        KeyMap.Add("NUM0", 0x60);
        KeyMap.Add("NUM1", 0x61);
        KeyMap.Add("NUM2", 0x62);
        KeyMap.Add("NUM3", 0x63);
        KeyMap.Add("NUM4", 0x64);
        KeyMap.Add("NUM5", 0x65);
        KeyMap.Add("NUM6", 0x66);
        KeyMap.Add("NUM7", 0x67);
        KeyMap.Add("NUM8", 0x68);
        KeyMap.Add("NUM9", 0x69);
        #endregion

        KeyMap.Add("LWIN", 91);
        KeyMap.Add("RWIN", 92);
        KeyMap.Add("ENTER", 0x0D);
        KeyMap.Add("BACKSPACE", 0x08);
        KeyMap.Add("NONE", -1);
    }

    [DllImport("USER32.dll")]
    public static extern short GetKeyState(int nVirtKey);
}
