/*
 * Main.java
 *
 * Created on 25 Октябрь 2006 г., 13:24
 *
 * To change this template, choose Tools | Template Manager
 * and open the template in the editor.
 */

package um;

import java.io.BufferedInputStream;
import java.io.FileInputStream;

/**
 *
 * @author lattyf
 */
public class Main
{
    
    /** Creates a new instance of Main */
    public Main()
    {
    }
    
    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) throws Exception
    {
        // TODO code application logic here
        
        Processor.getInstance();
        MemoryManager.getInstance();
        
        if (args.length < 1)
        {
            System.err.println("Usage: java -jar Um.jar scroll_name");
            return;
        }
        
        System.out.println("Загрузка программы...");
        
        BufferedInputStream scrollStream = new BufferedInputStream(
                new FileInputStream(args[0]));
        
        byte[] buffer = new byte[4];
        long[] scroll = new long[scrollStream.available() / 4 + 1];
        
        long readed = scrollStream.read(buffer, 0, 4);
        int scrollIndex = 0;
        while (readed != -1 && scrollIndex < scroll.length)
        {
            scroll[scrollIndex] = 0L;
            
            for (int i = 0; i < readed; i++)
                scroll[scrollIndex] =
                        (scroll[scrollIndex] << 8) | (0xFFL & buffer[i]);
            
            scrollIndex++;
            readed = scrollStream.read(buffer, 0, 4);
        }
        
        System.out.println("Программа загружена, начато выполнение.");
        
        MemoryManager.getInstance().loadScroll(scroll);
        
        Processor processor = Processor.getInstance();
        
        processor.performProgram();
    }
    
}
