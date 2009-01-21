/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package endo.runner;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author lattyf
 */
public class BaseSeq {
    
    private String seq;
    
    public BaseSeq()
    {
        seq = "";
    }
    
    public BaseSeq(InputStream stream)
    {
        try
        {
            BufferedReader reader = new BufferedReader(new InputStreamReader(stream));

            seq = reader.readLine();
        } 
        catch (IOException ex)
        {
            Logger.getLogger(BaseSeq.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    public boolean isStartsWith(String prefix)
    {
        return seq.startsWith(prefix);
    }
    
    public void dropFromStart(int count)
    {
        seq = seq.substring(count);
    }
    
    public char charAt(int index)
    {
        return seq.charAt(index);
    }
}
