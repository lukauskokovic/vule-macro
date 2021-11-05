using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class MacroScript
{
    public List<Instruction> Instructions = new List<Instruction>();

    public void ParseFile(string FilePath)
    {
        if (!File.Exists(FilePath))
        {
            Console.Write("file:{0} does not exist.", FilePath);
            return;
        }
        Console.WriteLine("Parsing file:'{0}'", FilePath);
        string[] Lines = File.ReadAllLines(FilePath);
        for(int i = 0; i < Lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(Lines[i])) continue;

            string[] Split = Lines[i].Split(' ');
            Instruction instruction = null;
            switch (Split[0])
            {
                case "CURSOR":
                    instruction = new CursorInstruction();
                    break;
                case "MOUSE":
                    instruction = new MouseInstruction();
                    break;
                case "SLEEP":
                    instruction = new SleepInstruction();
                    break;
            }
            if(instruction != null)
            {
                instruction.parameters = Split.Skip(1).ToArray();
                instruction.Parse();
                Instructions.Add(instruction);
            }
        }
    }
    public void Execute()
    {
        Console.WriteLine("Executing");
        foreach (Instruction instruction in Instructions)
            instruction.Execute();
    }
}