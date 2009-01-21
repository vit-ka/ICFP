import std.stdio;
import std.file;
import um.processor;
import um.memory;

void main(string[] args)
{
	 Processor processor = Processor.getInstance();
	 Memory memory = Memory.getInstance();
		
	 if (args.length < 2)
	 {
			writefln("Usage: um um_image.umz");
			return;
	 }
		
	 writefln("Reading file %s", args[1]);
		
	 byte[] data = cast(byte[])std.file.read(args[1]);
	 uint[] scroll = new uint[(data.length + 3) / 4];
		
	 for (int i = 0; i < data.length; i += 4)
	 {
			scroll[i / 4] = (0xFF & data[i]) << 24
				 | (0xFF & data[i + 1]) << 16
				 | (0xFF & data[i + 2]) << 8
				 | (0xFF & data[i + 3]);
	 }
		
		
	 writefln("Read %d bytes and %d instrustions", data.length, scroll.length);
		
	 memory.loadScroll(scroll);
		
	 writefln("Program loaded to memory. Interpretation started.");
		
	 processor.performProgram();
		
	 return 0;
}
