/*
 * MemoryManager.java
 *
 * Created on 25 Октябрь 2006 г., 13:37
 *
 * To change this template, choose Tools | Template Manager
 * and open the template in the editor.
 */

package um;

import java.util.ArrayList;
import java.util.List;

/**
 *
 * @author lattyf
 */
public class MemoryManager
{
    
    private static MemoryManager instance;
    private List<long[]> arrays;
    private int nextFreeArrayID = 1;
    
    /** Creates a new instance of MemoryManager */
    private MemoryManager()
    {
        arrays = new ArrayList<long[]>();
        arrays.add(null);
    }
    
    public static MemoryManager getInstance()
    {
        if (instance == null)
            instance = new MemoryManager();
        
        return instance;
    }
    
    public int allocate(int aCapasity)
    {
        long[] newArray = new long[aCapasity];
        
        for (int i = 0; i < aCapasity; ++i)
            newArray[i] = 0;
        
        int newArrayID = getFreeArrayID();
        arrays.set(newArrayID, newArray);
        return newArrayID;
    }
    
    public void abandon(int anArrayID)
    {
        arrays.set(anArrayID, null);
    }
    
    public List<long[]> getArrays()
    {
        return arrays;
    }
    
    private int getFreeArrayID()
    {
        int arraysSize = arrays.size();
        
        for (int i = nextFreeArrayID; i < arraysSize; ++i)
            if (arrays.get(i) == null)
            {
                nextFreeArrayID = i + 1;
                return i;
            }
        
        for (int i = 1; i < arraysSize; ++i)
            if (arrays.get(i) == null)
            {
                nextFreeArrayID = i + 1;
                return i;
            }
        
        for (int i = 0; i < 2000; ++i)
            arrays.add(null);
        
        nextFreeArrayID = arraysSize;
        
        return nextFreeArrayID;
    }
    
    public void copyToZeroArray(int anArrayID)
    {
        if (anArrayID == 0)
            return;
        
        if (arrays.get(0) != null)
        {
            arrays.set(0, null);
        }
        
        arrays.set(0, (long[]) arrays.get(anArrayID).clone());
    }
    
    public void loadScroll(long[] aScroll)
    {
        if (arrays.get(0) != null)
        {
            arrays.set(0, null);
        }
        
        arrays.set(0, (long[]) aScroll.clone());
    }
    
    
}
