using System;
using System.IO;

using UM.Command;
using UM.Memory;

namespace UM
{
	public class Program
	{
		static void Main(string[] args)
		{
            DateTime time = DateTime.Now;

			Processor.GetInstance();
			MemoryManager.GetInstance();

			if (args.Length < 1)
			{
				Console.WriteLine("Использование: UM.exe scroll_name");
				return;
			}

			Console.WriteLine("Загрузка программы...");

			FileStream scrollStream = new FileStream(args[0], FileMode.Open);

			byte[] buffer = new byte[4];
			uint[] scroll = new uint[scrollStream.Length / 4 + 1];
			
			int readed = scrollStream.Read(buffer, 0, 4);
			int scrollIndex = 0;
			while (readed != -1 && scrollIndex < scroll.Length)
			{
				scroll[scrollIndex] = 0;

				for (int i = 0; i < readed; i++)
					scroll[scrollIndex] =
						(scroll[scrollIndex] << 8) + buffer[i];
				
				scrollIndex++;
				readed = scrollStream.Read(buffer, 0, 4);
			}

			Console.WriteLine("Программа загружена, начато выполнение.");

			MemoryManager.GetInstance().LoadScroll(scroll);

			Processor processor = Processor.GetInstance();

			processor.PerformProgram(); 

            Console.WriteLine("Время выполнения: {0}.", (DateTime.Now - time));
		}
	}
}
