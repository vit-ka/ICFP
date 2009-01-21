/*
 * Processor.java
 *
 * Created on 25 Октябрь 2006 г., 13:37
 *
 * To change this template, choose Tools | Template Manager
 * and open the template in the editor.
 */

package um;

/**
 *
 * @author lattyf
 */
public class Processor
{
    
    private static Processor instance;
    private MemoryManager memory;
    private long[] registers;
    public final int REGISTERS_COUNT = 8;
    private boolean halted = false;
    
    private int currentOffset = 0;
    
    /** Creates a new instance of Processor */
    private Processor()
    {
        registers = new long[REGISTERS_COUNT];
        
        memory = MemoryManager.getInstance();
    }
    
    public static Processor getInstance()
    {
        if (instance == null)
            instance = new Processor();
        
        return instance;
    }
    
    public void moveCursorTo(int anOffset)
    {
        currentOffset = anOffset;
    }
    
    public void performProgram()
    {
        long instruction;
        int instructionType;
        int registerAIndex;
        int registerBIndex;
        int registerCIndex;
        
        while (!halted)
        {
            instruction = memory.getArrays().get(0)[currentOffset];
            
            currentOffset++;
            
            instructionType = (int) ((instruction & 0xF0000000L) >> 28);
            
            registerAIndex = (int) ((instruction & 0x000001C0L) >> 6);
            registerBIndex = (int) ((instruction & 0x00000038L) >> 3);
            registerCIndex = (int) (instruction & 0x00000007L);
            
        /*System.out.println(currentOffset + ":" + instructionType + "("
                + instruction + ")"
                + " [" + registerAIndex + " " +
                registerBIndex + " " + registerCIndex + "] "
                + Arrays.toString(registers));*/
            
            switch (instructionType) {
                //Conditional move
                case 0x00:
                {
                    if (registers[registerCIndex] != 0)
                        registers[registerAIndex] = registers[registerBIndex];
                    break;
                }
                
                //Array index
                case 0x01:
                {
                    registers[registerAIndex] =
                            memory.getArrays().get((int)registers[registerBIndex])[(int)registers[registerCIndex]];
                    
                    break;
                }
                
                //Array amendment
                case 0x02:
                {
                    memory.getArrays().get((int)registers[registerAIndex])[(int)registers[registerBIndex]] =
                            registers[registerCIndex];
                    break;
                }
                
                //Addition
                case 0x03:
                {
                    long operand1 = registers[registerBIndex];
                    long operand2 = registers[registerCIndex];
                    registers[registerAIndex] = 0xFFFFFFFFL & (operand1 + operand2);
                    break;
                }
                
                //Multiplication
                case 0x04:
                {
                    long operand1 = registers[registerBIndex];
                    long operand2 = registers[registerCIndex];
                    registers[registerAIndex] = 0xFFFFFFFFL & (operand1 * operand2);
                    break;
                }
                
                //Division
                case 0x05:
                {
                    long operand1 = registers[registerBIndex];
                    long operand2 = registers[registerCIndex];
                    registers[registerAIndex] =  0xFFFFFFFFL & (operand1 / operand2);
                    break;
                }
                
                //Not and
                case 0x06:
                {
                    int operand1 = (int) registers[registerBIndex];
                    int operand2 = (int) registers[registerCIndex];
                    registers[registerAIndex] = ~(operand1 & operand2) & 0xFFFFFFFFL;
                    break;
                }
                
                //Halt
                case 0x07:
                {
                    halted = true;
                    System.out.println("Программа остановлена.");
                    break;
                }
                
                //Allocation
                case 0x08:
                {
                    int newArrayIndex =
                            memory.allocate((int)registers[registerCIndex]);
                    registers[registerBIndex] = newArrayIndex;
                    
                    break;
                }
                
                //Abandonment
                case 0x09:
                {
                    memory.abandon((int)registers[registerCIndex]);
                    break;
                }
                
                //Output
                case 0x0a:
                {
                    System.out.print((char) registers[registerCIndex]);
                    break;
                }
                
                //Input
                case 0x0b:
                {
                    
                    byte value;
                    
                    try {
                        value = (byte) System.in.read();
                    } catch (Exception ex) {
                        value = (byte) '*';
                    }
                    
                    if (value <= 0) {
                        registers[registerCIndex] = 0xFFFFFFFF;
                    } else
                        registers[registerCIndex] = (int) value;
                    
                    break;
                }
                
                //Load program
                case 0x0c:
                {
                    memory.copyToZeroArray((int)registers[registerBIndex]);
                    moveCursorTo((int)registers[registerCIndex]);
                    break;
                }
                
                //Orphography
                case 0x0d:
                {
                    int registerIndex = (int) ((instruction & 0x0E000000L) >> 25);
                    long value = instruction & 0x01FFFFFFL;
                    
                    registers[registerIndex] = value;
                    
                    break;
                }
                
                default:
                {
                    throw new IllegalArgumentException("Illegal operation code " + instructionType);
                }
                
            }
            
        }
    }
}
