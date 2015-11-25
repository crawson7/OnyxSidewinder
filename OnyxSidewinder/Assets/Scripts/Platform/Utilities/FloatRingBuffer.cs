using System;

public class FloatRingBuffer  : RingBuffer<float>{

    public FloatRingBuffer(int arraySize) : base(arraySize) { }


    public float GetAverage()
    {

        float sum = 0f;
        for (int i = 0; i < Size; i++)
        {
            sum = sum + GetAt(i); 
        }
        return sum / Size;
    }
        
}