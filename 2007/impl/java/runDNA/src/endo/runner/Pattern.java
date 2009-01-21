/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package endo.runner;

/**
 *
 * @author lattyf
 */
public class Pattern {
    
    private String pattern;
    
    public Pattern()
    {
        pattern = "";
    }

    void addSearch(String str)
    {
        throw new UnsupportedOperationException("Not yet implemented");
    }

    void addSkip(int n)
    {
        throw new UnsupportedOperationException("Not yet implemented");
    }

    void append(String prefix)
    {
        pattern = prefix + pattern;
    }

    void decLevel()
    {
        throw new UnsupportedOperationException("Not yet implemented");
    }

    void incLevel()
    {
        throw new UnsupportedOperationException("Not yet implemented");
    }
}
