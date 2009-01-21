/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package endo;

import java.io.FileInputStream;
import endo.runner.BaseSeq;
import endo.runner.BaseSeqOperator;
import endo.runner.Pattern;
import java.io.FileNotFoundException;

/**
 *
 * @author lattyf
 */
public class Main {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) throws FileNotFoundException {
        if (args.length < 1) {
            System.err.println("Usage: java -jar endo.jar <dna-file>");

            return;
        }

        String dnaFileName = args[0];

        BaseSeq dna = new BaseSeq(new FileInputStream(dnaFileName));

        BaseSeqOperator operator = new BaseSeqOperator(dna);

        // Начинаем главный цикл
        //while(true)
        {
            Pattern p = operator.getPattern();
        //Template t = operator.getTemplate();
        }

    }
}
