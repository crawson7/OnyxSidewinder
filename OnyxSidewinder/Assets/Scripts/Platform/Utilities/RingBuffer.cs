using System;


public class RingBuffer <T>
{
    private T[] _rbArray;
    private int _index;
    private int _arraySize;

    public RingBuffer(int arraySize)
    {
        _arraySize = arraySize;
        _rbArray = new T[_arraySize];
        _index = 0;
    }

    internal void SetSize(int size) { _arraySize = size; }
    internal void SetArray() { _rbArray = new T[_arraySize]; }

    public int Size { get { return _arraySize; } }
    public int Index { get { return _index; } }

    public void addValue(T value) // index is always pointing at the next open spot. To fetch the latest, we'll need to decrement index first.
    {
        if (_index >= _arraySize)
            _index = 0;
        _rbArray[_index] = value;
        _index++;
    }

    public T GetLatest()
    {
        if (_index - 1 < 0)
            return _rbArray[_arraySize - 1];
        else
            return _rbArray[_index-1];
    }

    public T GetAt(int fetchIndex)
    {
        if (fetchIndex >= _arraySize) // user error. Return latest value instead.
            return GetLatest();
        if (_index - fetchIndex - 1 < 0)
            return _rbArray[_arraySize - fetchIndex - 1];
         //else, _index - fetchIndex is positive.
        return _rbArray[_index - fetchIndex - 1];
    }

    /*
    public T ReturnAverage()
    {
        Type thisType = typeof(T);
        if (thisType.ToString() == "float")
        {
            float sum = 0f;
            for(int i = 0;i< _arraySize;i++)
            {
                sum = sum + GetAt(i); //no idea how this needs to work.
            }
            return sum / <float>_arraySize;
        }
        else // Default case. Will expand to include more ReturnAverage functions for different types later.
            return GetLatest();

    }*/

    public string returnAllTest()
    {
        string returnString = "";
        for (int i = 0; i < _arraySize;i++)
        {
            returnString = returnString + GetAt(i);
        }
        return returnString;

    }

    public string testString()
    {
        return "RingBuffer Success!";
    }


}