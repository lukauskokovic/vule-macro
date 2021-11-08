using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using static WindowsAPI;
using static PrasingHelper;

public abstract class Instruction
{
    public string Name;
    public string[] parameters;
    public abstract void Execute();
    public abstract void Parse();
    public abstract string Compile();
}

/// <summary>
/// Examples
/// SLEEP 10MS(mili secconds)
/// SLEEP 10M(10 minutes)
/// SLEEP 10S(10 seconds)
/// </summary>
public class SleepInstruction : Instruction
{
    public int Value;
    public string Unit;
    public SleepInstruction() { Name = "SLEEP"; }
    public override void Parse()
    {
        if (parameters.Length != 1)
            return;
        
        string timeStr = parameters[0];
        Unit = "";
        int time = 0;
        for (int i = 0; i < timeStr.Length; i++)
        {
            char c = timeStr[i];
            int numberValue;
            Console.WriteLine(isParam(c, "ms"));
            if (isParam(c, "ms")) Unit += char.ToUpper(c);
            else if((numberValue = c - '0') < 10)
            {
                time *= 10;
                time += numberValue;
            }
        }
        Value = time;
        Console.WriteLine("SLEEP> Value'{0}' Unit:'{1}'", Value, Unit);
    }

    public override void Execute()
    {
        
        int time = 1;
        if (Unit == "MS") time = Value;
        else if (Unit == "S") time = Value * 1000;
        else if (Unit == "M") time = Value * 1000 * 60;
        Thread.Sleep(time);
    }

    public override string Compile() => string.Format("SLEEP {0}{1}", Value, Unit);
}
/// <summary>
/// Example
/// 
/// CURSOR POS 1920 1080
/// CURSOR MOVE 100 100
/// </summary>
public class CursorInstruction : Instruction
{
    public string Type = null;
    public int ValueX, ValueY;

    string[] ValidTypes = new string[]
    {
        "POS",
        "MOVE"
    };
    public CursorInstruction() { Name = "CURSOR"; }
    public override void Parse()
    {
        if (parameters.Length != 3) return;
        parameters[0] = parameters[0].ToUpper();
        for(int i = 0; i < ValidTypes.Length; i++)
            if(parameters[0] == ValidTypes[i])
                Type = ValidTypes[i];

        if (!int.TryParse(parameters[1], out ValueX)) return;
        if (!int.TryParse(parameters[2], out ValueY)) return;
        Console.WriteLine("CURSOR>  Type:'{0}' X:'{1}' Y:'{2}'", Type, ValueX, ValueY);
    }

    public override void Execute()
    {
        if (Type == null) return;

        if (Type == "POS") SetCursorPos(ValueX, ValueY);
        else if(Type == "MOVE")
        {
            mouse_event(0x0001, ValueX, ValueY, 0, (UIntPtr)0);
        }
    }

    public override string Compile() => string.Format("CURSOR {0} {1} {2}", Type, ValueX, ValueY);
}
/// <summary>
/// Example
/// MOUSE DOWN LEFT
/// MOUSE UP RIGHT
/// </summary>
public class MouseInstruction : Instruction
{
    public MouseInstruction() { Name = "MOUSE"; }
    public string Button;
    public string Direction = null;


    private const uint RIGHT_UP = 0x0010,
                       RIGHT_DOWN = 0x0008,
                       LEFT_DOWN = 0x0002,
                       LEFT_UP = 0x0004;

    public override void Parse()
    {
        if(parameters.Length == 1)
        {
            if (!isParam(parameters[0], "LEFT", "RIGHT")) return;
            Direction = "PRESS";
            Button = parameters[0].ToUpper();
            return;
        }

        if (parameters.Length != 2) return;
        parameters[0] = parameters[0].ToUpper();
        parameters[1] = parameters[1].ToUpper();

        if (!isParam(parameters[0], "UP", "DOWN") || !isParam(parameters[1], "LEFT", "RIGHT")) return;
        Direction = parameters[0];
        Button = parameters[1];

        Console.WriteLine("MOUSE> Direction:'{0}' Button:'{1}'", Direction, Button);
    }

    public override void Execute()
    {
        if (Direction == null) return;
        if(Direction == "PRESS")
        {
            mouse_event(Button == "LEFT" ? LEFT_DOWN : RIGHT_DOWN, 0, 0, 0, (UIntPtr)0);
            mouse_event(Button == "LEFT" ? LEFT_UP : RIGHT_UP, 0, 0, 0, (UIntPtr)0);
            return;
        }
        mouse_event(Direction == "UP" ? Button == "LEFT" ? LEFT_UP : RIGHT_UP :
                                        Button == "LEFT" ? LEFT_DOWN : RIGHT_DOWN, 0, 0, 0, (UIntPtr)0);
    }

    public override string Compile()
    {
        if (Direction == null) return "";
        else if (Direction == "PRESS") return string.Format("MOUSE {0}", Button);
        else return string.Format("MOUSE {0} {1}", Direction, Button);
    }
}

/// <summary>
/// Example
/// JUMP 1
/// JUMP 3
/// </summary>
public class JumpInstruction : Instruction
{
    public int Line;
    private readonly MacroScript parentScript;
    public JumpInstruction(MacroScript script) { Name = "JUMP"; parentScript = script; }
    public override void Execute()
    {
        if (Line != -1 && Line < parentScript.Instructions.Count) parentScript.CurrentLine = Line-1;
    }

    public override void Parse()
    {
        if (parameters.Length != 1) return;
        if(!int.TryParse(parameters[0], out Line))
        {
            Line = -1;
            return;
        }
        Console.WriteLine("JUMP> Line:'{0}'", Line);
    }

    public override string Compile()
    {
        if (Line < 0) return "";
        else return "JUMP " + Line;
    }
}
/// <summary>
/// EXAMPLE
/// KEY W HOLD
/// KEY W
/// KEY L RELEASE
/// </summary>
public class KeyInstruction : Instruction
{
    const int KEYUP = 0x0002,
              KEYDOWN = 0x0000;
    public string TYPE = null;
    public int Key = -1;
    private string KeyName = "A";
    public KeyInstruction() { Name = "KEY"; }
    public override void Parse()
    {
        if (parameters.Length == 0 || parameters.Length > 2)
            return;

        if (!KeyMap.ContainsKey(parameters[0])) return;
        Key = KeyMap[parameters[0]];
        KeyName = parameters[0];
        if (parameters.Length == 2 && (parameters[1].ToUpper() == "HOLD" || parameters[1].ToUpper() == "RELEASE")) TYPE = (parameters[1].ToUpper() == "HOLD") ? "HOLD" : "RELEASE";
        else TYPE = "PRESS";

        Console.WriteLine("KEY> KEY:'{0}' Type:'{1}'", Key, TYPE);
    }
    public override void Execute()
    {
        if (TYPE == null || Key == -1) return;
        if (TYPE == "PRESS")
        {
            keybd_event(Key, 0, KEYDOWN, 0);
            keybd_event(Key, 0, KEYUP, 0);
        }
        else if (TYPE == "HOLD") keybd_event(Key, 0, KEYDOWN | 0x0001, 0);
        else if (TYPE == "RELEASE") keybd_event(Key, 0, KEYUP, 0);
    }

    public override string Compile()
    {
        if (TYPE == null) return "";
        else
        {
            if (TYPE == "PRESS") return "KEY " + KeyName;
            else return string.Format("KEY {0} {1}", KeyName, TYPE);
        }
    }
}

public static class WindowsAPI 
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern void keybd_event(int bVk, byte bScan, int dwFlags, int dwExtraInfo);
    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
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
        KeyMap.Add("NONE", -1);
    }

    [DllImport("USER32.dll")]
    public static extern short GetKeyState(int nVirtKey);
}

public static class PrasingHelper 
{
    public static bool isParam(string value, params string[] compare)
    {
        string upperValue = value.ToUpper();
        for(int i = 0; i < compare.Length; i++)
            if (compare[i].ToUpper() == upperValue) return true;

        return false;
    }
    public static bool isParam(char value, string compare)
    {
        char upperValue = char.ToUpper(value);
        for (int i = 0; i < compare.Length; i++)
            if (upperValue == char.ToUpper(compare[i])) return true;

        return false;
    }
}