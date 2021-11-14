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
            if (IsParam(c, "ms")) Unit += char.ToUpper(c);
            else if((numberValue = c - '0') < 10)
            {
                time *= 10;
                time += numberValue;
            }
        }
        Value = time;
        Console.WriteLine("\tSLEEP> Value'{0}' Unit:'{1}'", Value, Unit);
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
        Console.WriteLine("\tCURSOR>  Type:'{0}' X:'{1}' Y:'{2}'", Type, ValueX, ValueY);
    }

    public override void Execute()
    {
        if (Type == null) return;

        if (Type == "POS") SetCursorPos(ValueX, ValueY);
        else if(Type == "MOVE")
        {
            mouse_event(0x0001, ValueX, ValueY, 0, 0);
        }
    }

    public override string Compile() => string.Format("CURSOR {0} {1} {2}", Type, ValueX, ValueY);
}
/// <summary>
/// Example
/// MOUSE LEFT DOWN
/// MOUSE RIGHT UP
/// </summary>
public class MouseInstruction : Instruction
{
    public MouseInstruction() { Name = "MOUSE"; }
    public string Button;
    public string Direction = null;


    private const uint RIGHT_UP = 0x0010,
                       RIGHT_DOWN = 0x0008,
                       LEFT_DOWN = 2,
                       LEFT_UP = 4;

    public override void Parse()
    {
        if (parameters.Length == 1)
        {
            if (!IsParam(parameters[0], "LEFT", "RIGHT")) return;
            Direction = "PRESS";
            Button = parameters[0].ToUpper();
            return;
        }
        
        if (parameters.Length != 2) return;
       
        parameters[0] = parameters[0].ToUpper();
        parameters[1] = parameters[1].ToUpper();
        if (!IsParam(parameters[1], "UP", "DOWN") || !IsParam(parameters[0], "LEFT", "RIGHT")) return;
        Direction = parameters[1];
        Button = parameters[0];

        Console.WriteLine("\tMOUSE> Direction:'{0}' Button:'{1}'", Direction, Button);
    }

    public override void Execute()
    {
        if (Direction == null) return;
        if(Direction == "PRESS")
        {
            mouse_event(Button == "LEFT" ? LEFT_DOWN : RIGHT_DOWN, 0, 0, 0, 0);
            mouse_event(Button == "LEFT" ? LEFT_UP : RIGHT_UP, 0, 0, 0, 0);
            return;
        }
        mouse_event(Direction == "UP" ? Button == "LEFT" ? LEFT_UP : RIGHT_UP :
                                        Button == "LEFT" ? LEFT_DOWN : RIGHT_DOWN, 0, 0, 0, 0);
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
        Console.WriteLine("\tJUMP> Line:'{0}'", Line);
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

        Console.WriteLine("\tKEY> KEY:'{0}' Type:'{1}'", Key, TYPE);
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


public static class PrasingHelper 
{
    public static bool IsParam(string value, params string[] compare)
    {
        string upperValue = value.ToUpper();
        for(int i = 0; i < compare.Length; i++)
            if (compare[i].ToUpper() == upperValue) return true;

        return false;
    }
    public static bool IsParam(char value, string compare)
    {
        char upperValue = char.ToUpper(value);
        for (int i = 0; i < compare.Length; i++)
            if (upperValue == char.ToUpper(compare[i])) return true;

        return false;
    }
}