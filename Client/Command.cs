using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileClient2
{
	class Command
	{
		string fullCommand;
		string command;
		Object[] args;

		public Command(string fullCommand)
		{
			this.fullCommand = fullCommand;
			command = fullCommand.Split(' ')[0];
			args = fullCommand.Split(' ').Skip(1).ToArray();
		}

		public Command() { }

		private Command(string fullCommand, string command, Object[] args)
		{
			this.fullCommand = fullCommand;
			this.command = command;
			this.args = args;
		}

		public static Command StringParse(string fullCommand)
		{
			string command = fullCommand.Split(' ')[0];
			string[] args = fullCommand.Split(' ').Skip(1).ToArray();
			return new Command(fullCommand, command, args);
		}

		public static Command ParseOneArgument(string fullCommand)
		{
			string command = fullCommand.Split(' ')[0];
			string[] args = new string[1] { fullCommand.Substring(command.Length + 1) };
			return new Command(fullCommand, command, args);
		}

		public static Command IntParse(string fullCommand)
		{
			Command stringCommand = StringParse(fullCommand);
			Command intCommand = new Command(stringCommand.fullCommand, stringCommand.command, new Object[stringCommand.args.Length]);

			for (int i = 0; i < stringCommand.args.Length; i++)
			{
				try
				{
					intCommand.args[i] = int.Parse(stringCommand.args[i].ToString());
				}
				catch (FormatException)
				{
					intCommand.args[i] = stringCommand.args[i];
				}
			}

			return intCommand;
		}

		public string GetFullCommand()
		{
			return fullCommand;
		}

		public string GetCommand() 
		{
			return command;
		}

		public Object[] GetArgs()
		{
			return args;
		}
	}
}
