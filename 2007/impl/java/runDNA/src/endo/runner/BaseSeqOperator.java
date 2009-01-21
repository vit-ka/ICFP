/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package endo.runner;

/**
 *
 * @author lattyf
 */
public class BaseSeqOperator {
    private BaseSeq dna;
    
    public BaseSeqOperator(BaseSeq aDNA)
    {
        dna = aDNA;
    }
    
    public Pattern getPattern()
    {
        Pattern result = new Pattern();
        int level = 0;
        
        while (true)
        {
            if (dna.isStartsWith("C"))
            {
                dna.dropFromStart(1);
                result.append("I");
            }
            else if (dna.isStartsWith("F"))
            {
                dna.dropFromStart(1);
                result.append("C");
            }
            else if (dna.isStartsWith("P"))
            {
                dna.dropFromStart(1);
                result.append("F");
            }
            else if (dna.isStartsWith("IC"))
            {
                dna.dropFromStart(2);
                result.append("P");
            }
            else if (dna.isStartsWith("IP"))
            {
                dna.dropFromStart(2);
                int n = parseNumber();
                
                result.addSkip(n);
            } 
            else if (dna.isStartsWith("IF"))
            {
                dna.dropFromStart(3);
                
                String str = getConsts();
                
                result.addSearch(str);
            }
            else if (dna.isStartsWith("IIP"))
            {
                dna.dropFromStart(3);
                
                level++;
                
                result.incLevel();
            }
            else if (dna.isStartsWith("IIC") || dna.isStartsWith("IIF"))
            {
                dna.dropFromStart(3);
                
                if (level == 0)
                    return result;
                else
                {
                    level--;
                    result.decLevel();
                }
            }
            else if (dna.isStartsWith("III"))
            {
                // TODO: Добавить реализацию.
            }
            //TODO: Добавить выход
        }
    }

    private String getConsts()
    {
        throw new UnsupportedOperationException("Not yet implemented");
    }

    private int parseNumber()
    {
        throw new UnsupportedOperationException("Not yet implemented");
    }
}
